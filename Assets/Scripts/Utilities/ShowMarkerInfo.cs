using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;


public class ShowMarkerInfo : MonoBehaviour
{
    [SerializeField] public string title;
    [SerializeField] public string mainInfo;
    [SerializeField] public string id;
    [SerializeField] public Vector2 position;

    [SerializeField] public TextMeshPro titleText;
    [SerializeField] public TextMeshPro mainInfoText;

    private void Start()
    {
        titleText.text = title;
        mainInfoText.text = mainInfo;
        gameObject.SetActive(false);
    }

    public void ShowSensorInfo()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        SensorController.instance.SensorActive(id);
    }

    public void ShowHeatMapInfo()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        HeatMapController.instance.HeatMapTileActive(position);
    }
}
