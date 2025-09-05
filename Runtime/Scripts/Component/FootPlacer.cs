using UnityEngine;

namespace HRYooba.Kinect
{
    [RequireComponent(typeof(Animator))]
    public class FootPlacer : MonoBehaviour
    {
        private const float MaxFootDistanceGround = 0.02f;  // maximum distance from lower foot to the ground
        private const float MaxFootDistanceTime = 0.2f; // 1.0f;  // maximum allowed time, the lower foot to be distant from the ground

        [SerializeField] private float _smoothFactor = 10f;

        private Transform _leftFoot;
        private Transform _rightFoot;
        private Vector3 _leftFootInitPos;
        private Vector3 _rightFootInitPos;
        private Vector3 _initialUpVector;

        private Vector3 _footCorrection = Vector3.zero;
        private float _footDistance = 0f;
        private float _footDistanceTime = 0f;

        private void Awake()
        {
            var animator = GetComponent<Animator>();

            _initialUpVector = transform.up;
            _leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftToes);
            _rightFoot = animator.GetBoneTransform(HumanBodyBones.RightToes);
            _leftFootInitPos = _leftFoot ? _leftFoot.position : Vector3.zero;
            _rightFootInitPos = _rightFoot ? _rightFoot.position : Vector3.zero;
        }

        private void Update()
        {
            var targetPos = Vector3.zero;
            var newDistance = GetCorrDistanceToGround();
            var newDistanceTime = Time.time;

            if (Mathf.Abs(newDistance) >= MaxFootDistanceGround && Mathf.Abs(_footDistance + newDistance) < 1f)  // limit the correction to 1 meter
            {
                if ((newDistanceTime - _footDistanceTime) >= MaxFootDistanceTime)
                {
                    _footDistance += newDistance;
                    _footDistanceTime = newDistanceTime;

                    _footCorrection = _initialUpVector * _footDistance;
                }
            }
            else
            {
                _footDistanceTime = newDistanceTime;
            }

            targetPos += _footCorrection;

            transform.position = Vector3.Lerp(transform.position, targetPos, _smoothFactor * Time.deltaTime);
        }

        private float GetCorrDistanceToGround(Transform targetTransform, Vector3 initialPos)
        {
            Vector3 deltaDir = targetTransform.position - initialPos;
            Vector3 vTrans = new Vector3(deltaDir.x * _initialUpVector.x, deltaDir.y * _initialUpVector.y, deltaDir.z * _initialUpVector.z);

            float fSign = Vector3.Dot(deltaDir, _initialUpVector) < 0f ? 1f : -1f;  // change the sign, because it's a correction
            float deltaDist = fSign * vTrans.magnitude;

            return deltaDist;
        }

        private float GetCorrDistanceToGround()
        {
            float fDistMin = 1000f;
            float fDistLeft = _leftFoot ? GetCorrDistanceToGround(_leftFoot, _leftFootInitPos) : fDistMin;
            float fDistRight = _rightFoot ? GetCorrDistanceToGround(_rightFoot, _rightFootInitPos) : fDistMin;
            fDistMin = Mathf.Abs(fDistLeft) < Mathf.Abs(fDistRight) ? fDistLeft : fDistRight;

            if (fDistMin == 1000f)
            {
                fDistMin = 0f;
            }

            return fDistMin;
        }
    }
}