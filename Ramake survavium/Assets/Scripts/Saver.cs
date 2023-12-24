using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.position = new Vector3(35.0f, 0.09f, 29.0f);
    }
    
}
