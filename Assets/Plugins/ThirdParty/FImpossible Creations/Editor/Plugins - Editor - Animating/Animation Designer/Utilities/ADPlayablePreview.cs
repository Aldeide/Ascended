using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FIMSpace.AnimationTools
{
    public class ADPlayablePreview
    {
        // Playable - variables for animation clip preview
        public PlayableGraph graph;
        public AnimationClipPlayable clipPlayable;
        AnimationPlayableOutput baseOutput;
        AnimationLayerMixerPlayable mixer;

        // References to use 
        [NonSerialized] public AnimationClip previewClp = null;
        [NonSerialized] public Animator characterAnimator;
        bool wasCreated = false;

        public ADPlayablePreview(Animator animator)
        {
            characterAnimator = animator;
        }

        public void SetAnimationClipToPreview(AnimationClip clip)
        {
            if (previewClp == clip && clipPlayable.IsValid() && (clipPlayable.GetAnimationClip() == clip)) return;

            if (characterAnimator == null) { UnityEngine.Debug.Log("[Animation Preview] No Animator on the preview Character Model"); return; }
            if (!mixer.IsValid()) ResetPlayableGraph(characterAnimator);

            previewClp = clip;
            if (clip == null) return;

            graph.Play();
            CreatePlayableClip(clip);
        }

        public void SetTime(float time)
        {
            if (previewClp == null) return;

            clipPlayable.SetTime(time);
            graph.Evaluate(0f);
        }

        void ResetPlayableGraph(Animator animator)
        {
            Dispose();
            characterAnimator = animator;
            if (animator == null) return;

            graph = PlayableGraph.Create(animator.name + "-ADPreview");
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            baseOutput = AnimationPlayableOutput.Create(graph, "ADPreview", animator);

            mixer = AnimationLayerMixerPlayable.Create(graph, 1);
            baseOutput.SetSourcePlayable(mixer);
            wasCreated = true;
        }

        void CreatePlayableClip(AnimationClip clip)
        {
            if (clipPlayable.IsValid()) clipPlayable.Destroy();

            mixer.DisconnectInput(0);

            clipPlayable = AnimationClipPlayable.Create(graph, clip);
            clipPlayable.SetApplyFootIK(true);

            if (clip.isLooping == false) clipPlayable.SetDuration(clip.length);
            mixer.ConnectInput(0, clipPlayable, 0);
            mixer.SetInputWeight(0, 1f);
        }

        public void Dispose()
        {
            if (wasCreated == false) return;

            if (mixer.IsValid()) if (mixer.IsNull() == false) if (mixer.CanDestroy()) mixer.Destroy();
            if (clipPlayable.IsValid()) if (clipPlayable.IsNull() == false) if (clipPlayable.CanDestroy()) clipPlayable.Destroy();

            if (graph.IsValid())
            {
                graph.Stop();
                graph.Destroy();
            }
        }
    }
}