using System;

using UnityEngine;

public class BasketController3D : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float xLimit = 8f; // How far left/right (in meters) you can go

    void Update()
    {
        // 1. Get Input
        float moveInput = Input.GetAxis("Horizontal"); // A/D or Arrow Keys

        // 2. Move along the X axis
        transform.Translate(Vector3.right * moveInput * moveSpeed * Time.deltaTime);

        // 3. Clamp Position (Keep player on screen)
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(currentPos.x, -xLimit, xLimit);
        transform.position = currentPos;
    }
}