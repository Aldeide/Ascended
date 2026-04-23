using AbilitySystem.Runtime.Abilities;
using static AbilitySystem.Test.Utilities.AbilityUtilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using NUnit.Framework;
using AbilitySystem.Runtime.Core;
using System.Collections.Generic;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityPolicyTests
    {
        private IAbilitySystem _clientAbilitySystem;
        private IAbilitySystem _serverAbilitySystem;
        private AbilityDefinition _abilityDefinition;

        [SetUp]
        public void SetUp()
        {
            _clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            _serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            _abilityDefinition = CreateTestAbilityDefinition();
        }

        public struct NetTestCase
        {
            public AbilityNetworkPolicy NetPolicy;
            public AbilityNetworkSecurityPolicy SecurityPolicy;
            public bool RequestFromClient;
            public bool ExpectedActiveOnClient;
            public bool ExpectedActiveOnServer;
            public bool ExpectedRpcToServer;

            public override string ToString() => $"{NetPolicy}_{SecurityPolicy}_{(RequestFromClient ? "Cli" : "Svr")}";
        }

        public static IEnumerable<NetTestCase> ActivationCases()
        {
            foreach (var net in new[] { AbilityNetworkPolicy.ClientOnly, AbilityNetworkPolicy.ClientPredicted, AbilityNetworkPolicy.Server })
            {
                foreach (var sec in new[] { AbilityNetworkSecurityPolicy.ClientOrServer, AbilityNetworkSecurityPolicy.ServerOnlyExecution, AbilityNetworkSecurityPolicy.ServerOnlyTermination, AbilityNetworkSecurityPolicy.ServerOnly })
                {
                    foreach (var fromClient in new[] { true, false })
                    {
                        bool canClientStart = (sec == AbilityNetworkSecurityPolicy.ClientOrServer || sec == AbilityNetworkSecurityPolicy.ServerOnlyTermination);
                        
                        var tc = new NetTestCase { NetPolicy = net, SecurityPolicy = sec, RequestFromClient = fromClient };

                        if (fromClient)
                        {
                            if (net == AbilityNetworkPolicy.ClientOnly)
                            {
                                tc.ExpectedActiveOnClient = canClientStart;
                                tc.ExpectedActiveOnServer = false;
                                tc.ExpectedRpcToServer = false;
                            }
                            else if (net == AbilityNetworkPolicy.ClientPredicted)
                            {
                                tc.ExpectedActiveOnClient = canClientStart;
                                tc.ExpectedActiveOnServer = canClientStart; // Assuming RPC succeeds
                                tc.ExpectedRpcToServer = canClientStart;
                            }
                            else if (net == AbilityNetworkPolicy.Server)
                            {
                                tc.ExpectedActiveOnClient = false;
                                tc.ExpectedActiveOnServer = canClientStart;
                                tc.ExpectedRpcToServer = canClientStart;
                            }
                        }
                        else // From Server
                        {
                            // Server can always start anything except ClientOnly (which it tells client to start)
                            if (net == AbilityNetworkPolicy.ClientOnly)
                            {
                                tc.ExpectedActiveOnClient = true;
                                tc.ExpectedActiveOnServer = false;
                            }
                            else
                            {
                                tc.ExpectedActiveOnClient = false;
                                tc.ExpectedActiveOnServer = true;
                            }
                            tc.ExpectedRpcToServer = false;
                        }
                        yield return tc;
                    }
                }
            }
        }

        [Test, TestCaseSource(nameof(ActivationCases))]
        public void AbilityPolicy_Activation_Matrix(NetTestCase tc)
        {
            _abilityDefinition.NetworkPolicy = tc.NetPolicy;
            _abilityDefinition.NetworkSecurityPolicy = tc.SecurityPolicy;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var rpcStarted = false;
            _clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (name, key, data) =>
            {
                rpcStarted = true;
                // Simulate network bridge to server
                _serverAbilitySystem.AbilityManager.ServerTryActivateAbilityWithKey(name, key, data);
            };
            _clientAbilitySystem.AbilityManager.OnServerTryUnpredictedAbilityRequested += (name, data) =>
            {
                rpcStarted = true;
                _serverAbilitySystem.AbilityManager.TryActivateAbility(name, data);
            };
            _serverAbilitySystem.AbilityManager.OnNotifyClientActivateAbility += (name, data) =>
            {
                 _clientAbilitySystem.AbilityManager.ForceActivateAbility(name, data);
            };

            var initiator = tc.RequestFromClient ? _clientAbilitySystem : _serverAbilitySystem;
            initiator.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName);

            Assert.AreEqual(tc.ExpectedActiveOnClient, _clientAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Client Active Mismatch");
            Assert.AreEqual(tc.ExpectedActiveOnServer, _serverAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Server Active Mismatch");
            if (tc.RequestFromClient)
            {
                Assert.AreEqual(tc.ExpectedRpcToServer, rpcStarted, "RPC Dispatch Mismatch");
            }
        }

        public static IEnumerable<NetTestCase> TerminationCases()
        {
             foreach (var net in new[] { AbilityNetworkPolicy.ClientPredicted, AbilityNetworkPolicy.Server })
             {
                foreach (var sec in new[] { AbilityNetworkSecurityPolicy.ClientOrServer, AbilityNetworkSecurityPolicy.ServerOnlyExecution, AbilityNetworkSecurityPolicy.ServerOnlyTermination, AbilityNetworkSecurityPolicy.ServerOnly })
                {
                    foreach (var fromClient in new[] { true, false })
                    {
                        bool canClientEnd = (sec == AbilityNetworkSecurityPolicy.ClientOrServer || sec == AbilityNetworkSecurityPolicy.ServerOnlyExecution);
                        var tc = new NetTestCase { NetPolicy = net, SecurityPolicy = sec, RequestFromClient = fromClient };
                        
                        if (fromClient)
                        {
                            tc.ExpectedActiveOnClient = !canClientEnd;
                            tc.ExpectedActiveOnServer = !canClientEnd;
                        }
                        else
                        {
                            tc.ExpectedActiveOnClient = false;
                            tc.ExpectedActiveOnServer = false;
                        }
                        yield return tc;
                    }
                }
             }
        }

        [Test, TestCaseSource(nameof(TerminationCases))]
        public void AbilityPolicy_Termination_Matrix(NetTestCase tc)
        {
            _abilityDefinition.NetworkPolicy = tc.NetPolicy;
            _abilityDefinition.NetworkSecurityPolicy = tc.SecurityPolicy;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            // Setup: Both start it successfully (force it if needed to ignore activation security for this termination test)
            _serverAbilitySystem.AbilityManager.ForceActivateAbility(_abilityDefinition.UniqueName);
            _clientAbilitySystem.AbilityManager.ForceActivateAbility(_abilityDefinition.UniqueName);

            bool isProcessing = false;
            _clientAbilitySystem.AbilityManager.OnServerTryEndAbilityRequested += (name) =>
            {
                if (isProcessing) return;
                isProcessing = true;
                _serverAbilitySystem.AbilityManager.EndAbility(name);
                isProcessing = false;
            };
            _serverAbilitySystem.AbilityManager.OnNotifyClientEndAbility += (name) =>
            {
                if (isProcessing) return;
                isProcessing = true;
                _clientAbilitySystem.AbilityManager.ForceEndAbility(name);
                isProcessing = false;
            };

            var initiator = tc.RequestFromClient ? _clientAbilitySystem : _serverAbilitySystem;
            initiator.AbilityManager.EndAbility(_abilityDefinition.UniqueName);

            Assert.AreEqual(tc.ExpectedActiveOnClient, _clientAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Client Active Mismatch after End");
            Assert.AreEqual(tc.ExpectedActiveOnServer, _serverAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Server Active Mismatch after End");
        }

        [Test]
        public void AbilityPolicy_Host_DoesNotSendRpc()
        {
            var hostSystem = CreateMockServerAbilitySystem().Object; // Host is server
            // In our mock, we need to mark it as local client too for this test
            // Actually, we'll just check if RPC events are fired.
            
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            hostSystem.AbilityManager.GrantAbility(_abilityDefinition);

            bool rpcFired = false;
            hostSystem.AbilityManager.OnServerTryActivateAbilityRequested += (n, k, d) => rpcFired = true;

            hostSystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName);

            Assert.IsTrue(hostSystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive);
            Assert.IsFalse(rpcFired, "Host should not fire RPC events to itself.");
        }
    }
}