using NUnit.Framework;
using Systems.Audio;
using UnityEngine;

namespace Systems.Audio.Tests
{
    /// <summary>
    /// Unit tests verifying the correctness of static acoustic calculations in AcousticMath.
    /// </summary>
    public class AcousticMathTests
    {
        [Test]
        public void CalculateTransmissionLossDb_ValidInputs_ReturnsExpectedLoss()
        {
            // Loss = 10dB/m * 2.5m = 25dB
            float loss = AcousticMath.CalculateTransmissionLossDb(10f, 2.5f);
            Assert.AreEqual(25f, loss, 0.001f);
        }

        [Test]
        public void CalculateTransmissionLossDb_NegativeThickness_ReturnsZero()
        {
            float loss = AcousticMath.CalculateTransmissionLossDb(10f, -5f);
            Assert.AreEqual(0f, loss);
        }

        [Test]
        public void CalculateVolumeScaleFromDb_ZeroLoss_ReturnsOne()
        {
            float scale = AcousticMath.CalculateVolumeScaleFromDb(0f);
            Assert.AreEqual(1f, scale);
        }

        [Test]
        public void CalculateVolumeScaleFromDb_Attenuation_ReturnsCorrectScale()
        {
            // 20dB loss should attenuate volume to exactly 0.1 (10%)
            float scale20Db = AcousticMath.CalculateVolumeScaleFromDb(20f);
            Assert.AreEqual(0.1f, scale20Db, 0.001f);

            // 6dB loss should attenuate volume to approx 0.5 (50%)
            float scale6Db = AcousticMath.CalculateVolumeScaleFromDb(6.02f);
            Assert.AreEqual(0.5f, scale6Db, 0.01f);
        }

        [Test]
        public void CalculateVolumeScaleFromDb_NegativeDb_ReturnsOne()
        {
            float scale = AcousticMath.CalculateVolumeScaleFromDb(-10f);
            Assert.AreEqual(1f, scale);
        }

        [Test]
        public void CalculateCutoffFrequency_ZeroThickness_ReturnsMaxCutoff()
        {
            float cutoff = AcousticMath.CalculateCutoffFrequency(0f, 5f, 1000f, 150f, 22000f);
            Assert.AreEqual(22000f, cutoff);
        }

        [Test]
        public void CalculateCutoffFrequency_MaxThickness_ReturnsMaterialOrMinCutoff()
        {
            // Material cutoff (1000Hz) is higher than MinCutoff (150Hz) -> should return 1000Hz
            float cutoff1 = AcousticMath.CalculateCutoffFrequency(5f, 5f, 1000f, 150f, 22000f);
            Assert.AreEqual(1000f, cutoff1);

            // Material cutoff (50Hz) is lower than MinCutoff (150Hz) -> should clamp to 150Hz
            float cutoff2 = AcousticMath.CalculateCutoffFrequency(5f, 5f, 50f, 150f, 22000f);
            Assert.AreEqual(150f, cutoff2);
        }

        [Test]
        public void CalculateCutoffFrequency_PartialThickness_InterpolatesCorrectly()
        {
            // Halfway (2.5m out of 5.0m)
            float cutoff = AcousticMath.CalculateCutoffFrequency(2.5f, 5.0f, 2000f, 2000f, 22000f);
            // Lerps between 22000 and 2000 -> 12000
            Assert.AreEqual(12000f, cutoff, 0.1f);
        }

        [Test]
        public void CalculateEyringReverbDecay_TypicalRoom_ReturnsReasonableDecay()
        {
            // Concrete room: average distance = 10m, absorption = 0.1
            float decay = AcousticMath.CalculateEyringReverbDecay(10f, 0.1f, 0.1f, 10f);
            
            // Math: RT60 = (0.0537 * 10) / -ln(1 - 0.1)
            // -ln(0.9) approx 0.10536
            // RT60 = 0.537 / 0.10536 approx 5.09s
            Assert.AreEqual(5.097f, decay, 0.01f);
        }

        [Test]
        public void CalculateEyringReverbDecay_DeadOrOpenSpace_ClampsToMinDecay()
        {
            // Almost total absorption (0.99) and small size -> decay is tiny, clamps to MinDecay
            float decay = AcousticMath.CalculateEyringReverbDecay(1f, 0.99f, 0.2f, 10f);
            Assert.AreEqual(0.2f, decay);
        }

        [Test]
        public void CalculateReverbDelay_ValidDistances_ReturnsExpectedDelay()
        {
            // 34.3 meters should take exactly 0.1 seconds (100ms) to reflect
            float delay = AcousticMath.CalculateReverbDelay(34.3f);
            Assert.AreEqual(0.1f, delay, 0.001f);

            // 0 meters should have 0 delay
            float zeroDelay = AcousticMath.CalculateReverbDelay(0f);
            Assert.AreEqual(0f, zeroDelay);
        }

        [Test]
        public void CalculateRoomHF_CutoffBounds_MapsToReverbFilterDecibels()
        {
            // Max cutoff (20000Hz) should map to 0dB (brightest)
            float roomHFMax = AcousticMath.CalculateRoomHF(20000f);
            Assert.AreEqual(0f, roomHFMax);

            // Min cutoff (150Hz) should map to -5000dB (muffled)
            float roomHFMin = AcousticMath.CalculateRoomHF(150f);
            Assert.AreEqual(-5000f, roomHFMin);

            // Halfway (10075Hz) should map to -2500dB
            float roomHFMid = AcousticMath.CalculateRoomHF(10075f);
            Assert.AreEqual(-2500f, roomHFMid, 1f);
        }
    }
}
