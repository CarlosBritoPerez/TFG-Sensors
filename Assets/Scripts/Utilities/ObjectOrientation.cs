using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOrientation : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;

    [SerializeField] Transform TargetTransform;


    private void OnEnable()
    {
        if (TargetTransform == null)
        {
            GameObject camera = GameObject.Find("/MixedRealityPlayspace/Main Camera");
            TargetTransform = camera.transform;
        }

        if (GetComponent<Renderer>())
        {
            GetComponent<Renderer>().material.renderQueue = 4000;
        }

        Update();
    }


    private void Update()
    {
        if (TargetTransform == null)
        {
            return;
        }

        Vector3 objectOrientation;
        objectOrientation.x = TargetTransform.position.x - transform.position.x;
        objectOrientation.y = 0;
        objectOrientation.z = TargetTransform.position.z - transform.position.z;

        transform.rotation = Quaternion.LookRotation(-objectOrientation);
        
    }
}
