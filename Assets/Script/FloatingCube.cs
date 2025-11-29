using System;
using UnityEngine;

public class FloatingIndicator : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.5f; 
    [SerializeField] private float floatSpeed = 2f; 

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }
}