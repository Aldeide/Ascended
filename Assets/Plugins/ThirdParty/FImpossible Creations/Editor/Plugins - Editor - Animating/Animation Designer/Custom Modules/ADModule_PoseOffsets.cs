using FIMSpace.FEditor;
using FIMSpace.Generating;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools.CustomModules
{

    public class ADModule_PoseOffsets : ADCustomModuleBase
    {
        public override string ModuleTitleName { get { return "Utilities/Bones Pose Fix Offsets"; } }
        public override bool GUIFoldable { get { return false; } }
        public override bool SupportBlending { get { return true; } }

        public List<BoneOffset> BoneOffsets = new List<BoneOffset>();

        [System.Serializable]
        public class BoneOffset
        {
            public string BoneID;
            public Vector3 RotationOffset;
            internal Transform transform;
            public float Blend = 1f;
            [HideInInspector] public bool Foldout = true;
        }

        ADClipSettings_Modificators.ModificatorSet.EOrder _updateOrder = ADClipSettings_Modificators.ModificatorSet.EOrder.InheritElasticity;
        public override void OnResetState( ADClipSettings_CustomModules.CustomModuleSet customModuleSet )
        {
            base.OnResetState( customModuleSet );

            var updateOrder = GetVariable( "Update Order:", customModuleSet, 0 );
            _updateOrder = (ADClipSettings_Modificators.ModificatorSet.EOrder)updateOrder.GetIntValue();
        }

        public override void OnBeforeIKUpdate( float animationProgress, float deltaTime, AnimationDesignerSave s, ADClipSettings_Main anim_MainSet, ADClipSettings_CustomModules customModules, ADClipSettings_CustomModules.CustomModuleSet set )
        {
            base.OnBeforeIKUpdate( animationProgress, deltaTime, s, anim_MainSet, customModules, set );
            if( _updateOrder != ADClipSettings_Modificators.ModificatorSet.EOrder.BeforeEverything ) return;
            UpdatePose( set, s, anim_MainSet, animationProgress );
        }

        public override void OnInheritElasticnessUpdate( float animationProgress, float deltaTime, AnimationDesignerSave s, ADClipSettings_Main anim_MainSet, ADClipSettings_CustomModules customModules, ADClipSettings_CustomModules.CustomModuleSet set )
        {
            base.OnInheritElasticnessUpdate( animationProgress, deltaTime, s, anim_MainSet, customModules, set );
            if( _updateOrder != ADClipSettings_Modificators.ModificatorSet.EOrder.InheritElasticity ) return;
            UpdatePose( set, s, anim_MainSet, animationProgress );
        }

        public override void OnInfluenceIKUpdate( float animationProgress, float deltaTime, AnimationDesignerSave s, ADClipSettings_Main anim_MainSet, ADClipSettings_CustomModules customModules, ADClipSettings_CustomModules.CustomModuleSet set )
        {
            base.OnInfluenceIKUpdate( animationProgress, deltaTime, s, anim_MainSet, customModules, set );
            if( _updateOrder != ADClipSettings_Modificators.ModificatorSet.EOrder.AffectIK ) return;
            UpdatePose( set, s, anim_MainSet, animationProgress );
        }

        public override void OnLastUpdate( float animationProgress, float deltaTime, AnimationDesignerSave s, ADClipSettings_Main anim_MainSet, ADClipSettings_CustomModules customModules, ADClipSettings_CustomModules.CustomModuleSet set )
        {
            base.OnLastUpdate( animationProgress, deltaTime, s, anim_MainSet, customModules, set );
            if( _updateOrder != ADClipSettings_Modificators.ModificatorSet.EOrder.Last_Override ) return;
            UpdatePose( set, s, anim_MainSet, animationProgress );
        }

        void UpdatePose( ADClipSettings_CustomModules.CustomModuleSet set, AnimationDesignerSave s, ADClipSettings_Main anim_MainSet, float animProgress )
        {
            RefreshReferences( s );

            float blend = GetEvaluatedBlend( set, animProgress );

            for( int i = 0; i < BoneOffsets.Count; i++ )
            {
                var bone = BoneOffsets[i];
                if( bone.transform == null ) continue;

                bone.transform.localRotation *= Quaternion.Euler( bone.RotationOffset * blend * bone.Blend );
            }
        }

        int toRemove = -1;
        public override void InspectorGUI_ModuleBody( float clipProgress, ADClipSettings_Main _anim_MainSet, AnimationDesignerSave s, ADClipSettings_CustomModules cModule, ADClipSettings_CustomModules.CustomModuleSet set )
        {
            EditorGUILayout.HelpBox( "Module which will apply rotation offsets to the bones in order to fix it's pose.", UnityEditor.MessageType.None );
            EditorGUILayout.HelpBox( "Settings of this module are saved in the file, like preset settings.\nGenerate another instance of this module to apply different offsets per animation clip.", UnityEditor.MessageType.Info );

            GUILayout.Space( 4 );

            #region Update Order

            var updateOrder = GetVariable( "Update Order:", set, 0 );
            updateOrder.HideFlag = true;

            ADClipSettings_Modificators.ModificatorSet.EOrder order = (ADClipSettings_Modificators.ModificatorSet.EOrder)
                updateOrder.GetIntValue();

            order = (ADClipSettings_Modificators.ModificatorSet.EOrder)
                EditorGUILayout.EnumPopup( "Update Order: ", order );

            updateOrder.SetValue( (int)order );

            #endregion

            _updateOrder = order;

            GUILayout.Space( 6 );

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField( "Bones To Adjust:", EditorStyles.boldLabel );

            GUILayout.FlexibleSpace();

            if( GUILayout.Button( "+", GUILayout.Width( 24 ) ) )
            {
                BoneOffsets.Add( new BoneOffset() );
                EditorUtility.SetDirty( this );
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space( 3 );

            EditorGUI.BeginChangeCheck();

            for( int i = 0; i < BoneOffsets.Count; i++ )
            {
                DisplayBoneGUI( BoneOffsets[i], s, i );
                GUILayout.Space( 3 );
            }

            if( EditorGUI.EndChangeCheck() ) EditorUtility.SetDirty( this );

            if( toRemove != -1 )
            {
                BoneOffsets.RemoveAt( toRemove );
                EditorUtility.SetDirty( this );
                toRemove = -1;
            }
        }

        void DisplayBoneGUI( BoneOffset b, AnimationDesignerSave s, int index )
        {
            EditorGUILayout.BeginVertical( FGUI_Resources.BGInBoxStyle );

            EditorGUILayout.BeginHorizontal();
            if( GUILayout.Button( FGUI_Resources.GetFoldSimbol( b.Foldout, true ), EditorStyles.label, GUILayout.Width( 20 ) ) ) { b.Foldout = !b.Foldout; }
            GUILayout.Space( 4 );
            ArmatureTransformField( ref b.transform, ref b.BoneID, s, "To Adjust Bone:", index.ToString() );

            GUILayout.Space( 6 );
            FGUI_Inspector.RedGUIBackground();
            if( GUILayout.Button( FGUI_Resources.GUIC_Remove, FGUI_Resources.ButtonStyle, GUILayout.Width( 22 ), GUILayout.Height(18) ) ) { toRemove = index; }
            FGUI_Inspector.RestoreGUIBackground();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 2 );

            if( b.Foldout )
            {
                b.RotationOffset = EditorGUILayout.Vector3Field( "Rotation Offset:", b.RotationOffset );
                b.Blend = EditorGUILayout.Slider( "Blend:", b.Blend, 0f, 1f );
            }

            EditorGUILayout.EndVertical();
        }

        string _vselectorHelperID = "";

        protected Transform ArmatureTransformField( ref Transform transformVar, ref string transformNameVar, AnimationDesignerSave s, string title, string fieldID = "1" )
        {
            Transform preT = transformVar;

            bool searchableWasSet = Searchable.IsSetted;

            if( _vselectorHelperID == fieldID )
            {
                var getTr = Searchable.Get<Transform>();
                if( searchableWasSet ) if( getTr != preT ) { transformVar = getTr; if( getTr != null ) transformNameVar = getTr.name; else transformNameVar = ""; }
            }

            if( preT == null )
            {
                if( !string.IsNullOrEmpty( transformNameVar ) )
                {
                    preT = s.GetBoneByName( transformNameVar );
                    transformVar = preT;
                }
            }

            EditorGUILayout.BeginHorizontal();

            Transform newT = (Transform)EditorGUILayout.ObjectField( title, preT, typeof( Transform ), true );
            if( preT != newT ) { transformVar = newT; if( newT != null ) transformNameVar = newT.name; }

            if( GUILayout.Button( new GUIContent( FGUI_Resources.Tex_DownFold, "Display quick selection menu for available bone transform to choose" ), EditorStyles.label, GUILayout.Width( 16 ), GUILayout.Height( 16 ) ) )
            {
                _vselectorHelperID = fieldID;
                AnimationDesignerWindow.ShowBonesSelector( "Choose Your Character Model Bone", s.GetAllArmatureBonesList, AnimationDesignerWindow.GetMenuDropdownRect(), true );
            }

            EditorGUILayout.EndHorizontal();

            return newT;
        }

        bool wasRefreshed = false;
        void RefreshReferences( AnimationDesignerSave save )
        {
            if( wasRefreshed ) return;

            for( int b = 0; b < BoneOffsets.Count; b++ )
            {
                if( BoneOffsets[b].transform != null ) continue;
                ReadTransformForIDVariables( ref BoneOffsets[b].transform, BoneOffsets[b].BoneID, save );
            }

            wasRefreshed = true;
        }

        protected void ReadTransformForIDVariables( ref Transform transformVar, string transformNameVar, AnimationDesignerSave s )
        {
            Transform preT = transformVar;

            if( preT == null )
            {
                if( !string.IsNullOrEmpty( transformNameVar ) )
                {
                    preT = s.GetBoneByName( transformNameVar );
                    transformVar = preT;
                }
            }
        }

    }
}