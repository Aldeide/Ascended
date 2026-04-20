using Sirenix.OdinInspector;
using UnityEngine;

namespace Systems.Controllers
{
    public class WeaponController : MonoBehaviour
    {
        public GameObject weapon;
        [ShowInInspector] private Transform _muzzle;

        public void Start()
        {
            _muzzle = weapon.transform.Find("Muzzle");
        }

        public Vector3 GetMuzzlePosition()
        {
            return _muzzle.position;
        }
    }
}