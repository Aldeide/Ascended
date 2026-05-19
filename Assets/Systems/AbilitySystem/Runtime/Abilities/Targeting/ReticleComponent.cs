using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.Targeting
{
    /// <summary>
    /// A visual component that represents the area of effect or targeting reticle.
    /// </summary>
    public class ReticleComponent : MonoBehaviour
    {
        public Renderer[] RenderersToTint;
        public Color ValidColor = Color.green;
        public Color InvalidColor = Color.red;

        public void SetValidTarget(bool isValid)
        {
            var color = isValid ? ValidColor : InvalidColor;
            
            if (RenderersToTint != null)
            {
                foreach (var r in RenderersToTint)
                {
                    r.material.color = color;
                }
            }
        }
    }
}
