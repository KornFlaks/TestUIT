using CamCode;
using UnityEngine;

namespace SunLighting
{
    public class MatchCamera : MonoBehaviour
    {
        private float _rotationSpeed;

        private void Start()
        {
            if (Camera.main is { })
                _rotationSpeed = Camera.main.GetComponent<InputControl>().RotationSpeed;
        }

        // Update is called once per frame
        private void Update()
        {
            var horizontal = 0;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                horizontal++;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                horizontal--;
            transform.RotateAround(Vector3.zero, Vector3.up, _rotationSpeed * Time.unscaledDeltaTime * horizontal);
        }
    }
}