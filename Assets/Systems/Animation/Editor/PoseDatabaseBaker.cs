using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Ascended.Systems.Animation.Runtime;

namespace Ascended.Systems.Animation.Editor
{
    /// <summary>
    /// Offline baking tool for the Motion Matching Database.
    /// Evaluates AnimationClips at a specific sample rate to extract joint positions, 
    /// velocities, and trajectory paths.
    /// </summary>
    public class PoseDatabaseBaker : EditorWindow
    {
        private MotionMatchingDatabase _database;
        private Animator _targetAvatar;

        [MenuItem("Ascended/Animation/Pose Database Baker")]
        public static void ShowWindow()
        {
            GetWindow<PoseDatabaseBaker>("Pose Baker");
        }

        private void OnGUI()
        {
            GUILayout.Label("Motion Matching Database Baker", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Assign a database and an Animator in the scene to bake poses.", MessageType.Info);

            _database = (MotionMatchingDatabase)EditorGUILayout.ObjectField("Database", _database, typeof(MotionMatchingDatabase), false);
            _targetAvatar = (Animator)EditorGUILayout.ObjectField("Target Avatar", _targetAvatar, typeof(Animator), true);

            EditorGUILayout.Space();
            GUILayout.Label("Add Clips to Database", EditorStyles.boldLabel);

            // Drag and drop area
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 60.0f, GUILayout.ExpandWidth(true));
            GUIStyle dropBoxStyle = new GUIStyle(GUI.skin.box);
            dropBoxStyle.alignment = TextAnchor.MiddleCenter;
            dropBoxStyle.normal.textColor = Color.gray;
            GUI.Box(dropArea, "Drag & Drop Animation Clips Here to Add", dropBoxStyle);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        List<AnimationClip> clipsToAdd = new List<AnimationClip>();
                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is AnimationClip clip)
                            {
                                clipsToAdd.Add(clip);
                            }
                        }

                        if (clipsToAdd.Count > 0)
                        {
                            AddClipsToDatabase(clipsToAdd);
                        }
                    }
                    break;
            }

            if (GUILayout.Button("Add Selected Clips from Project View"))
            {
                List<AnimationClip> clipsToAdd = new List<AnimationClip>();
                foreach (UnityEngine.Object obj in Selection.objects)
                {
                    if (obj is AnimationClip clip)
                    {
                        clipsToAdd.Add(clip);
                    }
                }

                if (clipsToAdd.Count > 0)
                {
                    AddClipsToDatabase(clipsToAdd);
                }
                else
                {
                    Debug.LogWarning("No AnimationClips selected in the Project view.");
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Bake Database", GUILayout.Height(30)))
            {
                if (_database != null && _targetAvatar != null)
                {
                    Bake();
                }
                else
                {
                    Debug.LogWarning("Please assign a Database and a Target Avatar (in the scene) to bake.");
                }
            }
        }

        private void AddClipsToDatabase(List<AnimationClip> newClips)
        {
            if (_database == null)
            {
                Debug.LogWarning("Please assign a Database first.");
                return;
            }

            List<MotionMatchingClipEntry> existingList = new List<MotionMatchingClipEntry>();
            if (_database.SourceClips != null)
            {
                existingList.AddRange(_database.SourceClips);
            }

            int addedCount = 0;
            foreach (var clip in newClips)
            {
                if (clip != null)
                {
                    bool exists = false;
                    foreach (var entry in existingList)
                    {
                        if (entry.Clip == clip)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        float nominalSpeed = 0f;
                        string lowerName = clip.name.ToLower();
                        if (lowerName.Contains("walk")) nominalSpeed = 1.5f;
                        else if (lowerName.Contains("run") || lowerName.Contains("sprint")) nominalSpeed = 4.0f;
                        else if (lowerName.Contains("idle")) nominalSpeed = 0f;

                        existingList.Add(new MotionMatchingClipEntry { Clip = clip, ForwardSpeed = nominalSpeed });
                        addedCount++;
                    }
                }
            }

            if (addedCount > 0)
            {
                _database.SourceClips = existingList.ToArray();
                EditorUtility.SetDirty(_database);
                AssetDatabase.SaveAssets();
                Debug.Log($"[PoseBaker] Added {addedCount} new clips to database {_database.name}. Total clips: {_database.SourceClips.Length}");
            }
            else
            {
                Debug.Log("[PoseBaker] No new clips were added (all selected clips are already in the database).");
            }
        }

        private void Bake()
        {
            if (_database.SourceClips == null || _database.SourceClips.Length == 0)
            {
                Debug.LogWarning("No source clips assigned in the database.");
                return;
            }

            List<PoseData> bakedPoses = new List<PoseData>();
            float timeStep = 1f / _database.SampleRate;

            Transform hips = _targetAvatar.GetBoneTransform(HumanBodyBones.Hips);
            Transform leftFoot = _targetAvatar.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rightFoot = _targetAvatar.GetBoneTransform(HumanBodyBones.RightFoot);

            if (hips == null || leftFoot == null || rightFoot == null)
            {
                Debug.LogError("Target Avatar is missing required humanoid bones (Hips, LeftFoot, RightFoot).");
                return;
            }

            // Create a PlayableGraph to evaluate animations offline
            PlayableGraph graph = PlayableGraph.Create("BakingGraph");
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            var playableOutput = AnimationPlayableOutput.Create(graph, "Animation", _targetAvatar);

            for (int c = 0; c < _database.SourceClips.Length; c++)
            {
                AnimationClip clip = _database.SourceClips[c].Clip;
                float forwardSpeed = _database.SourceClips[c].ForwardSpeed;
                if (clip == null) continue;

                var clipPlayable = AnimationClipPlayable.Create(graph, clip);
                playableOutput.SetSourcePlayable(clipPlayable);

                int numFrames = Mathf.FloorToInt(clip.length / timeStep);
                
                // Track previous frame positions to calculate velocity
                float3 prevHipsPos = float3.zero;
                float3 prevLeftFootPos = float3.zero;
                float3 prevRightFootPos = float3.zero;

                float3 displacement = float3.zero;
                float displacementAngle = 0f;

                if (forwardSpeed > 0f)
                {
                    displacement = new float3(0f, 0f, forwardSpeed * clip.length);
                }
                else
                {
                    // Precompute total clip displacement for looping clips
                    clipPlayable.SetTime(0f);
                    graph.Evaluate();
                    Vector3 startHipsPos = hips.position;
                    Vector3 startHipsForward = hips.forward;
                    startHipsForward.y = 0f;
                    float startAngle = Vector3.SignedAngle(Vector3.forward, startHipsForward.normalized, Vector3.up);

                    clipPlayable.SetTime(clip.length);
                    graph.Evaluate();
                    Vector3 endHipsPos = hips.position;
                    Vector3 endHipsForward = hips.forward;
                    endHipsForward.y = 0f;
                    float endAngle = Vector3.SignedAngle(Vector3.forward, endHipsForward.normalized, Vector3.up);

                    displacement = _targetAvatar.transform.InverseTransformPoint(endHipsPos) - _targetAvatar.transform.InverseTransformPoint(startHipsPos);
                    displacementAngle = Mathf.DeltaAngle(startAngle, endAngle);
                }

                for (int f = 0; f < numFrames; f++)
                {
                    float time = f * timeStep;
                    
                    // Evaluate the graph at the specific time
                    clipPlayable.SetTime(time);
                    graph.Evaluate();

                    // Extract data in local space relative to the character root
                    float3 currentHipsPos = _targetAvatar.transform.InverseTransformPoint(hips.position);
                    float3 currentLeftFootPos = _targetAvatar.transform.InverseTransformPoint(leftFoot.position);
                    float3 currentRightFootPos = _targetAvatar.transform.InverseTransformPoint(rightFoot.position);

                    // Calculate velocities (skip first frame as we don't have a previous position)
                    float3 hipsVel = f == 0 ? float3.zero : (currentHipsPos - prevHipsPos) / timeStep;
                    float3 leftFootVel = f == 0 ? float3.zero : (currentLeftFootPos - prevLeftFootPos) / timeStep;
                    float3 rightFootVel = f == 0 ? float3.zero : (currentRightFootPos - prevRightFootPos) / timeStep;

                    // Calculate local root frame for trajectory delta projection
                    float3 currentHipsRootPos;
                    float currentFacingAngle;

                    if (forwardSpeed > 0f)
                    {
                        currentHipsRootPos = new float3(0f, 0f, forwardSpeed * time);
                        currentFacingAngle = 0f;
                    }
                    else
                    {
                        currentHipsRootPos = new float3(currentHipsPos.x, 0f, currentHipsPos.z);
                        Vector3 currentHipsForward = hips.forward;
                        currentHipsForward.y = 0f;
                        if (currentHipsForward.sqrMagnitude < 0.001f) currentHipsForward = hips.up;
                        currentHipsForward.y = 0f;
                        currentFacingAngle = Vector3.SignedAngle(Vector3.forward, currentHipsForward.normalized, Vector3.up);
                    }
                    
                    quaternion currentRootRot = quaternion.Euler(0, math.radians(currentFacingAngle), 0);
                    quaternion invRootRot = math.inverse(currentRootRot);

                    // Evaluate future trajectory points
                    GetHipsPoseAtTime(graph, clipPlayable, time + 0.33f, clip, displacement, displacementAngle, forwardSpeed, out float3 p0, out float3 v0, out float facing0);
                    GetHipsPoseAtTime(graph, clipPlayable, time + 0.66f, clip, displacement, displacementAngle, forwardSpeed, out float3 p1, out float3 v1, out float facing1);
                    GetHipsPoseAtTime(graph, clipPlayable, time + 1.00f, clip, displacement, displacementAngle, forwardSpeed, out float3 p2, out float3 v2, out float facing2);

                    PoseData pose = new PoseData
                    {
                        ClipIndex = c,
                        Time = time,
                        
                        HipsPosition = currentHipsPos,
                        LeftFootPosition = currentLeftFootPos,
                        RightFootPosition = currentRightFootPos,
                        
                        HipsVelocity = hipsVel,
                        LeftFootVelocity = leftFootVel,
                        RightFootVelocity = rightFootVel,
                        
                        Trajectory0 = new TrajectoryPointData
                        {
                            Position = math.mul(invRootRot, p0 - currentHipsRootPos),
                            Velocity = math.mul(invRootRot, v0),
                            FacingAngle = Mathf.DeltaAngle(currentFacingAngle, facing0)
                        },
                        Trajectory1 = new TrajectoryPointData
                        {
                            Position = math.mul(invRootRot, p1 - currentHipsRootPos),
                            Velocity = math.mul(invRootRot, v1),
                            FacingAngle = Mathf.DeltaAngle(currentFacingAngle, facing1)
                        },
                        Trajectory2 = new TrajectoryPointData
                        {
                            Position = math.mul(invRootRot, p2 - currentHipsRootPos),
                            Velocity = math.mul(invRootRot, v2),
                            FacingAngle = Mathf.DeltaAngle(currentFacingAngle, facing2)
                        }
                    };

                    bakedPoses.Add(pose);

                    prevHipsPos = currentHipsPos;
                    prevLeftFootPos = currentLeftFootPos;
                    prevRightFootPos = currentRightFootPos;
                }
            }

            // Cleanup PlayableGraph
            if (graph.IsValid())
            {
                graph.Destroy();
            }

            // Save the data
            _database.Poses = bakedPoses.ToArray();
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            Debug.Log($"[PoseBaker] Baked {bakedPoses.Count} poses successfully into {_database.name}.");
        }

        private void GetHipsPoseAtTime(PlayableGraph graph, AnimationClipPlayable clipPlayable, float time, AnimationClip clip, float3 displacement, float displacementAngle, float forwardSpeed, out float3 position, out float3 velocity, out float facingAngle)
        {
            float length = clip.length;
            int wraps = 0;
            float sampleTime = time;

            if (clip.isLooping)
            {
                if (sampleTime > length)
                {
                    wraps = Mathf.FloorToInt(sampleTime / length);
                    sampleTime = sampleTime % length;
                }
            }
            else
            {
                sampleTime = Mathf.Clamp(sampleTime, 0f, length);
            }

            clipPlayable.SetTime(sampleTime);
            graph.Evaluate();

            Transform hips = _targetAvatar.GetBoneTransform(HumanBodyBones.Hips);
            float3 localHipsPos = _targetAvatar.transform.InverseTransformPoint(hips.position);
            
            float3 localRootPos = new float3(localHipsPos.x, 0f, localHipsPos.z);
            float3 flatDisplacement = new float3(displacement.x, 0f, displacement.z);

            if (forwardSpeed > 0f)
            {
                position = new float3(0f, 0f, forwardSpeed * time);
                velocity = new float3(0f, 0f, forwardSpeed);
                facingAngle = 0f;
            }
            else
            {
                position = localRootPos + wraps * flatDisplacement;

                Vector3 hipsForward = hips.forward;
                hipsForward.y = 0f;
                if (hipsForward.sqrMagnitude < 0.001f)
                {
                    hipsForward = hips.up;
                    hipsForward.y = 0f;
                }
                float rawAngle = Vector3.SignedAngle(Vector3.forward, hipsForward.normalized, Vector3.up);
                facingAngle = rawAngle + wraps * displacementAngle;

                // Approximate velocity by looking at a tiny step forward
                float epsilon = 0.03f;
                float nextTime = sampleTime + epsilon;
                int nextWraps = wraps;
                if (clip.isLooping)
                {
                    if (nextTime > length)
                    {
                        nextWraps = Mathf.FloorToInt(nextTime / length);
                        nextTime = nextTime % length;
                    }
                }
                else
                {
                    nextTime = Mathf.Clamp(nextTime, 0f, length);
                }

                clipPlayable.SetTime(nextTime);
                graph.Evaluate();

                float3 nextLocalHipsPos = _targetAvatar.transform.InverseTransformPoint(hips.position);
                float3 nextLocalRootPos = new float3(nextLocalHipsPos.x, 0f, nextLocalHipsPos.z);
                float3 nextPosition = nextLocalRootPos + nextWraps * flatDisplacement;
                velocity = (nextPosition - position) / epsilon;
            }
        }
    }
}
