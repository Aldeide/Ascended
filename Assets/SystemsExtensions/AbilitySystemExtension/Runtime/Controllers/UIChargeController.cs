using UnityEngine;
using UnityEngine.UI;

namespace SystemsExtensions.AbilitySystemExtension.Runtime.Controllers
{
    public class UIChargeController : MonoBehaviour
    {
        public GameObject chargeBar;

        private Image _fillImage;
        
        private void Awake()
        {
            _fillImage = chargeBar.GetComponent<Image>();
        }

        public void SetFill(float fillAmount)
        {
            if (!_fillImage)
            {
                Debug.Log("UIChargeController: _fillImage is null!");
                return;
            }
            _fillImage.fillAmount = fillAmount;
        }
    }
}