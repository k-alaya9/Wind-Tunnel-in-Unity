using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure2 : MonoBehaviour
{
    // New public variable for rotation speed
    public float rotationSpeed = 100f;

    void Update()
    {
        // Rotate the fan GameObject
        transform.Rotate(Vector3.up, rotationSpeed);
    }
}
