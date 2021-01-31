using System;

namespace UnityEngine
{
    public class HiraAnimator : MonoBehaviour
    {
        [SerializeField] private Transform target = null;
        [SerializeField] private HiraCreatureBody body = null;
        

        private Vector3 _lastPosition = default;
        private static readonly int speed_variable_hash = Animator.StringToHash("Speed");
        private static readonly int inactive_variable_hash = Animator.StringToHash("Inactive");

        private void Reset()
        {
            target = transform;
            body = GetComponentInChildren<HiraCreatureBody>();
        }

        private void OnValidate()
        {
            if (target == null) target = transform;
        }

        private void OnEnable()
        {
            _lastPosition = target.position;
            body.Animator.SetBool(inactive_variable_hash, false);
        }

        private void OnDisable()
        {
            body.Animator.SetBool(inactive_variable_hash, true);
        }

        private void Update()
        {
            var currentPosition = target.position;
            var speed = Vector3.Magnitude(currentPosition - _lastPosition);

            body.Animator.SetFloat(speed_variable_hash, speed);
        }
    }
}