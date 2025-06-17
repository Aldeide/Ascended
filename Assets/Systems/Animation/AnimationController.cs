using UnityEngine;

namespace Systems.Controllers
{
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

        public void SetIsMoving(bool isMoving)
        {
            _animator.SetBool(IsMoving, isMoving);
        }

        public void SetMoveForward()
        {
            SetIsMoving(true);
            _animator.SetFloat(MovementX, 0);
            _animator.SetFloat(MovementY, 1);
        }

        public void SetMovement(float x, float y)
        {
            SetIsMoving(Mathf.Abs(x) + Mathf.Abs(y) >= 0.001);
            _animator.SetFloat(MovementX, x);
            _animator.SetFloat(MovementY, y);
        }

        public void StopMovement()
        {
            SetMovement(0, 0);
            SetIsMoving(false);
        }
    }
}