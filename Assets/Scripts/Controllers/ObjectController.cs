using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public OnlineMapsMarker3DManager onlineMapsMarker3DManager;

    public void SetAssetsVisible(string assetsName, bool value = false)
    {
        List<OnlineMapsMarker3D> itemsList = onlineMapsMarker3DManager.items;
        foreach (OnlineMapsMarker3D item in itemsList)
        {
            if (item.label == assetsName)
            {
                item.enabled = value;
            }
        }
    }
}
