using UnityEngine;

namespace Systems.Animation
{
    /// <summary>
    /// A class for controlling animation states using Unity's Animator component.
    /// </summary>
    /// <remarks>
    /// This class provides methods to control character movement animations. It relies on the Animator component
    /// attached to the same GameObject. Ensure that an Animator component is present to avoid runtime errors.
    /// </remarks>
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private Animator _animator;
        
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int MovementX = Animator.StringToHash("MovementX");
        private static readonly int MovementY = Animator.StringToHash("MovementY");
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Sets the movement state in the Animator component.
        /// </summary>
        /// <param name="isMoving">A boolean indicating whether the character is moving (true) or stationary (false).</param>
        public void SetIsMoving(bool isMoving)
        {
            _animator.SetBool(IsMoving, isMoving);
        }

        /// <summary>
        /// Transitions the Animator component to a forward movement state.
        /// </summary>
        /// <remarks>
        /// This method sets the animation parameters to indicate forward movement,
        /// with no horizontal movement (MovementX set to 0) and vertical movement
        /// (MovementY set to 1). It also updates the Animator to reflect that the character
        /// is in motion.
        /// </remarks>
        public void SetMoveForward()
        {
            SetIsMoving(true);
            _animator.SetFloat(MovementX, 0);
            _animator.SetFloat(MovementY, 1);
        }

        /// <summary>
        /// Updates the movement parameters in the Animator component based on the specified X and Y values.
        /// </summary>
        /// <param name="x">The horizontal movement input value.</param>
        /// <param name="y">The vertical movement input value.</param>
        public void SetMovement(float x, float y)
        {
            SetIsMoving(Mathf.Abs(x) + Mathf.Abs(y) >= 0.001);
            _animator.SetFloat(MovementX, x);
            _animator.SetFloat(MovementY, y);
        }

        /// <summary>
        /// Stops all movement animations in the Animator component by resetting movement parameters and setting the movement state to stationary.
        /// </summary>
        public void StopMovement()
        {
            SetMovement(0, 0);
            SetIsMoving(false);
        }
    }
}