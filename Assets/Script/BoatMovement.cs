using UnityEngine;

public class BoatMovement : MonoBehaviour
{

    public float speed = 5f;
    public float endZ = 400f;
    private float startZ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);


        if (transform.position.z >= endZ)
        {
            Vector3 pos = transform.position;
            pos.z = startZ;
            transform.position = pos;
        }
    }
}
