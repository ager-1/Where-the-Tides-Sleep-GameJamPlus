
using UnityEngine;

public class FallingWood3D : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float catchDistance = 1.5f; // How close to the basket to count as a "Catch"

    // Internal references
    private WoodGameManager3D manager;
    private WoodData myData;
    private Transform basketTransform;

    public void Setup(WoodGameManager3D gm, WoodData data, Transform basket)
    {
        manager = gm;
        myData = data;
        basketTransform = basket;
    }

    void Update()
    {
        // 1. Fall Down (World Space Y)
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        // 2. Check Distance to Basket (Simple Collision)
        float dist = Vector3.Distance(transform.position, basketTransform.position);
        if (dist < catchDistance)
        {
            manager.CatchWood(myData);
            Destroy(gameObject);
        }

        // 3. Miss Logic (If it falls below the player)
        // Adjust -5f depending on where your floor is
        if (transform.position.y < basketTransform.position.y - 2f)
        {
            manager.MissWood();
            Destroy(gameObject);
        }
    }
}