using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class HololensButtonToggleColor : MonoBehaviour
{
    public bool isOn = false;
    public Interactable interactable;
    // Start is called before the first frame update
    void Start()
    {
        interactable.GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn) { 
            //interactable.pro
        }
        
    }
}
