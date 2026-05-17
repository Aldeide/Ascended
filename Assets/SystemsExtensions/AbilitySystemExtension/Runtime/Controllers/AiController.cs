using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using Unity.Netcode;

namespace Systems.Controllers
{
    public class AiController : NetworkBehaviour
    {
        private InterfaceController _interfaceController;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SetupAsc();
            var nameplateController = GetComponentInChildren<NameplateController>();
            nameplateController.Initialise(
                GetComponent<AbilitySystemComponent>().AbilitySystem as AbilitySystemManager);
        }

        public void Start()
        {
            SetupAsc();
            var nameplateController = GetComponentInChildren<NameplateController>();
            nameplateController.Initialise(
                GetComponent<AbilitySystemComponent>().AbilitySystem as AbilitySystemManager);
        }

        private bool _isAscSetup = false;

        private void SetupAsc()
        {
            if (_isAscSetup) return;
            _isAscSetup = true;
            GetComponent<AbilitySystemComponent>().Initialise();
            GetComponent<AbilitySystemComponent>().AbilitySystem.AbilityManager
                .TryActivateAbility("EnergyRegenAbility");
        }
    }
}