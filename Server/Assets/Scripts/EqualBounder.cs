using UnityEngine;

namespace Assets.Scripts
{
    public class EqualBounder : MonoBehaviour
    {
        private Rigidbody _rigidBody;
        private Vector3 _velocity = Vector3.zero;

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_velocity == Vector3.zero)
                _velocity = _rigidBody.velocity;
            else
                _rigidBody.velocity = _velocity;
        }
    }
}