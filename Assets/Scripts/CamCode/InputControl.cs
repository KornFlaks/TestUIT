using Unity.Mathematics;
using UnityEngine;

namespace CamCode
{
    public class InputControl : MonoBehaviour
    {
        [SerializeField] [Range(1f, 360f)] public float RotationSpeed = 40f;
        [SerializeField] [Range(0f, 10f)] public float MaxPolarAngle = 5f;
        [SerializeField] [Range(0.01f, 0.1f)] public float ZoomJump = 0.05f;
        [SerializeField] [Range(0.1f, 1f)] public float ZoomSpeed = 0.3f;
        public float MaxCameraZoom = 2.2f, MinCameraZoom = 1.1f;

        private float _positionTarget, _startTime;

        private void Start()
        {
            _positionTarget = math.length(transform.position);
            _startTime = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            // Get camera zoom info
            //transform.position += transform.forward * (MouseSpeed * Input.mouseScrollDelta.y);

            _positionTarget = math.clamp(
                math.pow(math.sqrt(_positionTarget - (MinCameraZoom - 0.05f))
                         - ZoomJump * Input.mouseScrollDelta.y, 2) + (MinCameraZoom - 0.05f),
                MinCameraZoom, MaxCameraZoom);

            if (math.abs(Input.mouseScrollDelta.y) > 0.01f)
                _startTime = Time.realtimeSinceStartup;

            var oldPosition = transform.position;
            transform.position = Vector3.Lerp(oldPosition, oldPosition.normalized * _positionTarget,
                (Time.realtimeSinceStartup - _startTime) / ZoomSpeed);

            // Get the keyboard input data
            int horizontal = 0, vertical = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                vertical++;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                vertical--;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                horizontal++;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                horizontal--;

            if (horizontal == 0 && vertical == 0)
                return;

            // Unlimited horizontal movement.
            transform.RotateAround(Vector3.zero, Vector3.up, RotationSpeed * Time.unscaledDeltaTime * horizontal);

            // Clamped vertical movement.
            var verticalAngle = Vector3.Angle(Vector3.up, transform.position);

            var direction = 1;
            if (verticalAngle > 90)
            {
                direction = -1;
                verticalAngle = 180 - verticalAngle;
            }

            if (verticalAngle < MaxPolarAngle && vertical == direction)
                return;

            transform.RotateAround(Vector3.zero, transform.right, RotationSpeed * Time.unscaledDeltaTime * vertical);
        }
    }
}