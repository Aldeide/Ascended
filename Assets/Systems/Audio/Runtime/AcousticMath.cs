using UnityEngine;

namespace Systems.Audio
{
    public static class AcousticMath
    {
        /// <summary>
        /// Calculates the total transmission loss in decibels based on wall thickness and material attenuation.
        /// </summary>
        public static float CalculateTransmissionLossDb(float transmissionLossPerMeter, float thickness)
        {
            return Mathf.Max(0f, transmissionLossPerMeter * thickness);
        }

        /// <summary>
        /// Converts decibel (dB) attenuation to a linear volume scale multiplier.
        /// Formula: scale = 10^(-dB / 20)
        /// </summary>
        public static float CalculateVolumeScaleFromDb(float db)
        {
            if (db <= 0f) return 1f;
            return Mathf.Clamp01(Mathf.Pow(10f, -db / 20f));
        }

        /// <summary>
        /// Calculates the low-pass cutoff frequency in Hz based on wall thickness.
        /// </summary>
        public static float CalculateCutoffFrequency(float thickness, float maxThickness, float materialCutoff, float minCutoff, float maxCutoff)
        {
            if (maxThickness <= 0.001f) return minCutoff;
            float thicknessPercent = Mathf.Clamp01(thickness / maxThickness);
            float targetCutoff = Mathf.Max(materialCutoff, minCutoff);
            return Mathf.Lerp(maxCutoff, targetCutoff, thicknessPercent);
        }

        /// <summary>
        /// Calculates the reverberation decay time (RT60) in seconds using the Eyring formula.
        /// Formula: RT60 = (0.0537 * avgDistance) / -ln(1 - avgAbsorption)
        /// </summary>
        public static float CalculateEyringReverbDecay(float averageDistance, float averageAbsorption, float minDecay, float maxDecay)
        {
            if (averageDistance <= 0f) return minDecay;
            
            // Clamp absorption to avoid Log(0) or Log(Negative)
            float clampedAbsorption = Mathf.Clamp(averageAbsorption, 0.001f, 0.999f);
            float denominator = -Mathf.Log(1f - clampedAbsorption);
            float decay = (0.0537f * averageDistance) / denominator;

            return Mathf.Clamp(decay, minDecay, maxDecay);
        }

        /// <summary>
        /// Estimates the early reflection delay time in seconds based on speed of sound (approx 343 m/s).
        /// </summary>
        public static float CalculateReverbDelay(float averageDistance)
        {
            if (averageDistance <= 0f) return 0f;
            return Mathf.Clamp(averageDistance / 343f, 0f, 0.1f);
        }

        /// <summary>
        /// Maps average material cutoff frequency to Unity's roomHF reverb filter parameter.
        /// roomHF ranges from -10000 (fully muffled) to 0 (bright/no dampening).
        /// </summary>
        public static float CalculateRoomHF(float averageCutoff)
        {
            float percent = Mathf.InverseLerp(150f, 20000f, averageCutoff);
            return Mathf.Lerp(-5000f, 0f, percent);
        }
    }
}
