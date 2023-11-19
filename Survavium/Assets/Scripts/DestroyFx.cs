using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFx : MonoBehaviour
{
    private float timeLeft = 4;
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}