using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Exhaustive matrix tests for Ability Network and Security policies, verifying correct behavior across client-server interactions.
    /// </summary>
    public class AbilityPolicyTests : AbilitySystemTestBase
    {
        private AbilityDefinition _abilityDefinition;
        private MockReplicationManager _clientRep;
        private MockReplicationManager _serverRep;

        [SetUp]
        public override void SetUp()
        {
            // We need explicit client/server roles for these tests
            SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();


            base.SetUp();

            _clientRep = (MockReplicationManager)Source.ReplicationManager;
            _serverRep = (MockReplicationManager)Target.ReplicationManager;

            _abilityDefinition = AbilityUtilities.CreateTestAbilityDefinition();

            // Link them for automatic communication

            AbilitySystemUtilities.LinkAbilitySystems(SourceMock, TargetMock);
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
                                tc.ExpectedActiveOnServer = canClientStart;
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
                            if (net == AbilityNetworkPolicy.ClientOnly)
                            {
                                tc.ExpectedActiveOnClient = true;
                                tc.ExpectedActiveOnServer = false;
                            }
                            else
                            {
                                tc.ExpectedActiveOnClient = (net != AbilityNetworkPolicy.Server);
                                tc.ExpectedActiveOnServer = true;
                            }
                            tc.ExpectedRpcToServer = false;
                        }
                        yield return tc;
                    }
                }
            }
        }

        /// <summary>
        /// Verifies the activation matrix for all combinations of NetworkPolicy, SecurityPolicy, and Initiator role.
        /// </summary>
        [Test, TestCaseSource(nameof(ActivationCases))]
        public void AbilityPolicyTests_ActivationMatrix_MatchesExpectedState(NetTestCase tc)
        {
            _abilityDefinition.NetworkPolicy = tc.NetPolicy;
            _abilityDefinition.NetworkSecurityPolicy = tc.SecurityPolicy;
            Source.AbilityManager.GrantAbility(_abilityDefinition);
            Target.AbilityManager.GrantAbility(_abilityDefinition);

            var rpcStarted = false;
            _clientRep.OnServerAbilityActivationRequested += (name, key, data) => rpcStarted = true;
            _clientRep.OnServerAbilityUnpredictedActivationRequested += (name, data) => rpcStarted = true;

            var initiator = tc.RequestFromClient ? Source : Target;
            initiator.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName);

            Assert.AreEqual(tc.ExpectedActiveOnClient, Source.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Client Active Mismatch");
            Assert.AreEqual(tc.ExpectedActiveOnServer, Target.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Server Active Mismatch");
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

        /// <summary>
        /// Verifies the termination matrix for all combinations of policies and initiator roles.
        /// </summary>
        [Test, TestCaseSource(nameof(TerminationCases))]
        public void AbilityPolicyTests_TerminationMatrix_MatchesExpectedState(NetTestCase tc)
        {
            _abilityDefinition.NetworkPolicy = tc.NetPolicy;
            _abilityDefinition.NetworkSecurityPolicy = tc.SecurityPolicy;
            Source.AbilityManager.GrantAbility(_abilityDefinition);
            Target.AbilityManager.GrantAbility(_abilityDefinition);

            // Force activation on both to test termination logic specifically
            Target.AbilityManager.ForceActivateAbility(_abilityDefinition.UniqueName);
            if (tc.NetPolicy != AbilityNetworkPolicy.Server)
            {
                Source.AbilityManager.ForceActivateAbility(_abilityDefinition.UniqueName);
            }

            var initiator = tc.RequestFromClient ? Source : Target;
            initiator.AbilityManager.EndAbility(_abilityDefinition.UniqueName);

            if (tc.NetPolicy != AbilityNetworkPolicy.Server)
            {
                Assert.AreEqual(tc.ExpectedActiveOnClient, Source.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Client Active Mismatch after End");
            }
            else
            {
                Assert.IsFalse(Source.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Client should never have active server-only ability");
            }
            Assert.AreEqual(tc.ExpectedActiveOnServer, Target.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive, "Server Active Mismatch after End");
        }

        /// <summary>
        /// Verifies that the host (Server + Local Client) does not attempt to send network RPCs to itself.
        /// </summary>
        [Test]
        public void AbilityPolicyTests_HostActivation_DoesNotSendRpcToSelf()
        {
            var hostSystemMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            var hostSystem = hostSystemMock.Object;
            hostSystemMock.Setup(x => x.IsLocalClient()).Returns(true);


            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            hostSystem.AbilityManager.GrantAbility(_abilityDefinition);

            bool rpcFired = false;
            hostSystem.ReplicationManager.OnServerAbilityActivationRequested += (n, k, d) => rpcFired = true;

            hostSystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName);

            Assert.IsTrue(hostSystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive);
            Assert.IsFalse(rpcFired, "Host should not fire RPC events to itself.");
        }
    }
}