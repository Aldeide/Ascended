using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Utilities
{
    /// <summary>
    /// Base class for unit tests that require a mocked Ability System environment.
    /// Provides pre-configured source and target systems with TestAttributeSet registered.
    /// </summary>
    public abstract class AbilitySystemTestBase
    {
        protected Mock<IAbilitySystem> SourceMock;
        protected Mock<IAbilitySystem> TargetMock;
        
        protected IAbilitySystem Source => SourceMock.Object;
        protected IAbilitySystem Target => TargetMock.Object;
        
        protected TestAttributeSet SourceAttributes;
        protected TestAttributeSet TargetAttributes;

        protected virtual bool AddDefaultAttributes => true;
        
        [SetUp]
        public virtual void SetUp()
        {
            InitializeMocks();
            InitializeAttributes();
        }

        protected virtual void InitializeMocks()
        {
            if (SourceMock == null) SourceMock = AbilitySystemUtilities.CreateMockAbilitySystem(AddDefaultAttributes);
            if (TargetMock == null) TargetMock = AbilitySystemUtilities.CreateMockAbilitySystem(AddDefaultAttributes);
        }

        protected virtual void InitializeAttributes()
        {
            if (AddDefaultAttributes)
            {
                SourceAttributes = Source.AttributeSetManager.GetAttributeSet<TestAttributeSet>();
                TargetAttributes = Target.AttributeSetManager.GetAttributeSet<TestAttributeSet>();
            }
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (SourceMock != null && Source.AttributeSetManager != null)
            {
                Source.AttributeSetManager.Dispose();
            }
            if (TargetMock != null && Target.AttributeSetManager != null)
            {
                Target.AttributeSetManager.Dispose();
            }
            SourceMock = null;
            TargetMock = null;
        }
    }
}
