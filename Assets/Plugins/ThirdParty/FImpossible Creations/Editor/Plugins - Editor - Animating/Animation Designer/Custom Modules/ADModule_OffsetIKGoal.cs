using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools.CustomModules
{
    public class ADModule_OffsetIKGoal : ADCustomModuleBase
    {
        public override string ModuleTitleName { get { return "IK/ffset IK Goal"; } }
        public override bool GUIFoldable { get { return false; } }
        public override bool SupportBlending { get { return true; } }

        // Here GUI code which defines variables to tweak and displaying them
        public override void InspectorGUI_ModuleBody(float optionalBlendGhost, ADClipSettings_Main _anim_MainSet, AnimationDesignerSave s, ADClipSettings_CustomModules cModule, ADClipSettings_CustomModules.CustomModuleSet set)
        {
            // Select limb by ID
            var selectedLimb = GetVariable("Limb", null, 0);
            selectedLimb.SetRangeHelperValue(new Vector2(0, s.Limbs.Count-1)); // Limbs count slider
            selectedLimb.GUISpacing = new Vector2(0, 6); // Spacing
            selectedLimb.HideFlag = true;

            // Define vector parameter
            var myVar = GetVariable("Off", null, Vector3.zero);
            myVar.DisplayName = "Offset IK";
            myVar.Tooltip = "Offset IK point by provided vector";

            var myVarB = GetVariable( "OffBlend", null, AnimationCurve.EaseInOut(0f,1f,1f,1f ) );
            myVarB.DisplayName = "Blend Offset";
            myVarB.SetRangeHelperValue( new Vector4( 0f, 0f, 1f, 1f ) );

            var limb = GetLimbByID(s, selectedLimb.GetIntValue());

            string selectedName = "None";
            if( limb  != null) selectedName = limb.GetName;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Use On:", GUILayout.MaxWidth( 64 ) );
            if( GUILayout.Button( selectedName, EditorStyles.popup ) ) DisplayGenericMenuLimb(s);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Display variables
            base.InspectorGUI_ModuleBody(optionalBlendGhost, _anim_MainSet, s, cModule, set);

            #region Accessing Limb's IK (here just to display "IK is disabled!" info)

            if (limb == null) EditorGUILayout.HelpBox("No Limb with ID = " + selectedLimb.IntV, MessageType.Warning);
            else
            {
                var ik = GetIKClipSettings(s, _anim_MainSet);
                if (ik != null)
                {
                    var limbIK = ik.GetIKSettingsForLimb(limb, s);
                    if (limbIK != null) if (limbIK.Enabled == false) EditorGUILayout.HelpBox("IK for limb '" + limbIK.GetName + "' Is Disabled!", MessageType.Warning);
                }
            }

            #endregion

        }

        void DisplayGenericMenuLimb( AnimationDesignerSave s )
        {
            GenericMenu menu = new GenericMenu();

            var vSel = GetVariable( "Limb", null, 0 );
            int sel = vSel.GetIntValue();

            for( int i = 0; i < s.Limbs.Count; i++ )
            {
                int id = i;
                var limb = GetLimbByID( s, id );
                if ( limb == null ) continue;
                menu.AddItem( new GUIContent( limb.GetName ), sel == id, () => { vSel.SetValue( id ); } );
            }

            menu.ShowAsContext();
        }


        // Here 'Update()' execution
        public override void OnInfluenceIKUpdate(float animationProgress, float deltaTime, AnimationDesignerSave s, ADClipSettings_Main anim_MainSet, ADClipSettings_CustomModules customModules, ADClipSettings_CustomModules.CustomModuleSet set)
        {
            Transform animator = s.LatestAnimator;
            if (animator == null) return; // No Animator - No Algorithm

            base.OnInfluenceIKUpdate(animationProgress, deltaTime, s, anim_MainSet, customModules, set);

            // Read variables defined inside 'InspectorGUI_ModuleBody()' above
            var selectedLimb = GetVariable("Limb", null, 0);
            int limbId = selectedLimb.GetIntValue();

            // Access selected limb IK to modify it
            var limb = GetLimbByID(s, limbId);

            #region Protection Checks to prevent console log errors in exception cases

            if (limb == null) return; // Limb not exists! Don't do anything then to prevent errors

            var ik = GetIKClipSettings(s, anim_MainSet);
            if (ik == null) return; // IK Setup not exists! Don't do anything then to prevent errors

            var limbIK = ik.GetIKSettingsForLimb(limb, s);
            if (limbIK == null) return; // No IK for the selected limb!

            if (limbIK.Enabled == false) return;

            #endregion

            var myVar = GetVariable("Off", null, Vector3.zero);
            var myVarB = GetVariable( "OffBlend", null, AnimationCurve.EaseInOut( 0f, 1f, 1f, 1f ) );
            Vector3 offset = myVar.GetVector3Value();

            float blend = GetEvaluatedBlend(set, animationProgress);
            offset *= blend * myVarB.GetCurve().Evaluate(animationProgress);

            if( s == null ) return;
            Transform characterSpace = s.LatestAnimator;
            if( characterSpace == null ) return;
            if( limb.IKLegProcessor == null ) return;

            if (limbIK.IKType == ADClipSettings_IK.IKSet.EIKType.FootIK)
            {
                var ikProc = limb.IKLegProcessor;
                ikProc.IKTargetPosition += characterSpace.TransformDirection( offset );
            }
            else if (limbIK.IKType == ADClipSettings_IK.IKSet.EIKType.ArmIK)
            {
                var ikProc = limb.IKArmProcessor;
                ikProc.IKTargetPosition += characterSpace.TransformDirection( offset );
            }

        }
    }
}