using UnityEditor;
using UnityEditor.Callbacks;

namespace NewGraph
{
    namespace NewGraph {
        [CustomEditor(typeof(AbilityGraphModel))]
        public class AbilityGraphModelEditor : GraphModelEditorBase {

            [OnOpenAsset]
            public static bool OnOpenAsset(int instanceID, int line) {
                IGraphModelData baseGraphModel = EditorUtility.InstanceIDToObject(instanceID) as IGraphModelData;
                if (baseGraphModel != null) {
                    baseGraphModel.CreateSerializedObject();
                    OpenGraph(baseGraphModel);
                    return true;
                }
                return false;
            }
        }
    }
}