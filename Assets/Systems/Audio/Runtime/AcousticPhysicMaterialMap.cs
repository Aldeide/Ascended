using System;
using UnityEngine;

namespace Systems.Audio
{
    [CreateAssetMenu(fileName = "AcousticPhysicMaterialMap", menuName = "Systems/Audio/Physic Material Map")]
    public class AcousticPhysicMaterialMap : ScriptableObject
    {
        [Serializable]
        public struct MaterialMapping
        {
            public PhysicsMaterial PhysicsMaterial;
            public AudioAcousticMaterial AcousticMaterial;
        }

        [Tooltip("The map linking PhysicsMaterials to their acoustic properties.")]
        public MaterialMapping[] Mappings;

        [Tooltip("Default material used when no mapping exists for a hit surface.")]
        public AudioAcousticMaterial DefaultMaterial;

        /// <summary>
        /// Retrieves the acoustic material properties corresponding to a PhysicsMaterial.
        /// </summary>
        public AudioAcousticMaterial GetAcousticMaterial(PhysicsMaterial physicsMaterial)
        {
            if (physicsMaterial == null)
            {
                return DefaultMaterial;
            }

            if (Mappings != null)
            {
                for (int i = 0; i < Mappings.Length; i++)
                {
                    if (Mappings[i].PhysicsMaterial == physicsMaterial)
                    {
                        return Mappings[i].AcousticMaterial;
                    }
                }
            }

            return DefaultMaterial;
        }
    }
}
