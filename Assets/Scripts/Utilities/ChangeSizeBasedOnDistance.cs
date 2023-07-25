using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSizeBasedOnDistance : MonoBehaviour
{
    float max = 10;
    float min = 1.5f;

    void Update()
    {
        GameObject camera = GameObject.Find("/MixedRealityPlayspace/Main Camera");

        float distance = Vector3.Distance(transform.position, camera.transform.position);
        Vector3 newScale = new Vector3(1, 1, 1);
        newScale *= Mathf.Exp(distance/5);
        if (newScale.x > max)
        {
            newScale = new Vector3(max, max, max);
        }
        else if (newScale.x < min)
        {
            newScale = new Vector3(min, min, min);
        }
        transform.localScale = newScale;
    }
}
