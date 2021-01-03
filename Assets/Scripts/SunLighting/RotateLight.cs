using UnityEngine;

namespace SunLighting
{
    public class RotateLight : MonoBehaviour
    {
        [SerializeField] [Range(30, 90)] public int RotationSpeed = 30;

        private void Update()
        {
            transform.RotateAround(Vector3.zero, Vector3.up, RotationSpeed * Time.deltaTime);
        }
    }
}