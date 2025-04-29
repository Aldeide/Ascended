using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using Systems.Interface;
using Unity.Netcode;

namespace Systems
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

        private void SetupAsc()
        {
            GetComponent<AbilitySystemComponent>().Initialise();
            GetComponent<AbilitySystemComponent>().AbilitySystem.AbilityManager
                .TryActivateAbility("EnergyRegenAbility");
        }
    }
}