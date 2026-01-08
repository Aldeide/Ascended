using Sirenix.OdinInspector.Editor;
using Unity.Netcode;
using UnityEditor;

namespace EternityCommon.Editor
{
    // Custom Editor class to use Odin Inspector for NetworkBehaviour.
    [CustomEditor(typeof(NetworkBehaviour), true)]
    public class OdinNetworkBehaviourEditor : OdinEditor {}
}