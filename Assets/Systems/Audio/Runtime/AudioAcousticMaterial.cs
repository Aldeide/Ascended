using UnityEngine;

namespace Systems.Audio
{
    [CreateAssetMenu(fileName = "AudioAcousticMaterial", menuName = "Systems/Audio/Acoustic Material")]
    public class AudioAcousticMaterial : ScriptableObject
    {
        [Tooltip("The fraction of sound energy absorbed by the surface. Range [0.0, 1.0]. (e.g. Carpet = 0.6, Concrete = 0.05)")]
        [Range(0f, 1f)]
        public float AbsorptionCoefficient = 0.1f;

        [Tooltip("Attenuation in decibels (dB) per unit of thickness when sound passes through this material. (e.g. Concrete = 30dB, Wood = 10dB)")]
        [Min(0f)]
        public float TransmissionLossDb = 10f;

        [Tooltip("The maximum cutoff frequency (Hz) allowed to pass through the material. Sound will be low-pass filtered to this cutoff. Range [10, 22000].")]
        [Range(10f, 22000f)]
        public float LowPassCutoffHz = 1000f;
    }
}
