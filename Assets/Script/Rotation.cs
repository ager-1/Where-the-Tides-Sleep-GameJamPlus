
using UnityEngine;

public class RotatingHalo : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}