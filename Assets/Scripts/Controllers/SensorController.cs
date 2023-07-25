using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorController : MonoBehaviour
{
    [SerializeField] private GameObject mapBoard;
    [SerializeField] private GameObject sensorPrefab;
    [SerializeField] private GameObject heatMapPrefab;
    [SerializeField] private Material heatMapMaterial;
    [SerializeField] private GameObject heatMapController;
    [SerializeField] private GridMap gridMap;
    [SerializeField] private JsonDataExtraction jsonDataExtraction;

    private OnlineMapsMarker3D sensorMapMaker3D;
    private OnlineMapsMarker3D wayPointsActive;
    private OnlineMapsMarker3D heatMapMaker3D;

    private ObjectController objectController;

    private SensorsList sensorList = new SensorsList();

    private bool dataUpdated = false;

    private string lastSensorActiveId;


    Dictionary<string, string> dictSensors = new Dictionary<string, string>();
    Dictionary<string, OnlineMapsMarker3D> dictSensorsPreview = new Dictionary<string, OnlineMapsMarker3D>();

    public static SensorController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void LoopShowSensorData()
    {
        InvokeRepeating("ShowSensorData", 0f, 20f);
    }

    public void ShowSensorData()
    {
        GetSensorData();
        AddSensors();
        //RemoveSensors();
        InstantiateSensors();
    }

    public void GetSensorData()
    {
        sensorList = JsonUtility.FromJson<SensorsList>(jsonDataExtraction.GetJsonData());
    }

    public void AddSensors()
    {
        dictSensors.Clear();
        dictSensorsPreview.Clear();
        foreach (Sensor sensor in sensorList.sensor)
        {
            dictSensors.Add(sensor.id.ToString(), sensor.id.ToString());
        }
    }

    public void ShowInfo(string idSensor)
    {
        int index = 0;
        int indexAux = 0;
        if (!dictSensorsPreview.ContainsKey(idSensor))
        {
            return;
        }

        foreach (Sensor sensor in sensorList.sensor)
        {
            if (sensor.id.ToString() == idSensor)
            {
                index = indexAux;
                break;
            }
            indexAux++;
        }
        wayPointsActive = dictSensorsPreview[idSensor];
    }

    public void NoShowInfo(string idSensor)
    {
        if (dictSensorsPreview.ContainsKey(idSensor))
        {
            wayPointsActive = null;
        }
    }

    public void SensorActive(string idSensor)
    {
        if (lastSensorActiveId == idSensor)
        {
            lastSensorActiveId = null;
        }
        else
        {
            TurnOnLastSensorInfoPanel(false);
            lastSensorActiveId = idSensor;
        }
    }

    public void InstantiateSensors()
    {

        if (sensorList.sensor.Length == 0)
        {
            return;
        }

        foreach (Sensor sensor in sensorList.sensor)
        {
            if (dictSensorsPreview.ContainsKey(sensor.id.ToString()))
            {
                double lat = sensor.location.coordinates[0];
                double lon = sensor.location.coordinates[1];

                dictSensorsPreview[sensor.id.ToString()].SetPosition(lat, lon);
                return;
            }

            sensorPrefab.GetComponentInChildren<ShowMarkerInfo>().id = sensor.id.ToString();

            SetInfoPanel(sensor, sensorPrefab);
          
            sensorMapMaker3D = OnlineMapsMarker3DManager.CreateItem(new Vector2(sensor.location.coordinates[0], sensor.location.coordinates[1]), sensorPrefab);
            sensorMapMaker3D.scale = 0.015f;
            dictSensorsPreview.Add(sensor.id.ToString(), sensorMapMaker3D);
        } 
    }

    private void SetInfoPanel(Sensor sensor, GameObject sensorModel)
    {
        sensorModel.GetComponentInChildren<ShowMarkerInfo>().titleText.text = "ID: " + sensor.id.ToString();

        string temperatureInfoPanel = "Temperature: " + sensor.data.temperature.ToString("0.0") + " ºC" + "\n";
        string humidityInfoPanel = "Humidity: " + sensor.data.humidity.ToString("0.0") + " %RH" + "\n";
        string ambientLightInfoPanel = "AmbientLight: " + sensor.data.ambientLight.ToString("0") + " lx" + "\n";
        string pressureInfoPanel = "Pressure: " + sensor.data.pressure.ToString("0.0") + " hPa" + "\n";
        string soundNoiseInfoPanel = "SoundNoise: " + sensor.data.soundNoise.ToString("0") + " dB" + "\n";
        string eTVOCInfoPanel = "eTVOC: " + sensor.data.eTVOC.ToString("0.0") + " ppb" + "\n";
        string eCO2InfoPanel = "eCO2: " + sensor.data.eCO2.ToString("0.0") + " ppm" + "\n";
        //string discomfortInfoPanel = "Discomfort: " + sensor.data.discomfort.ToString() + "" + "\n";
        //string heatStrokeInfoPanel = "HeatStroke: " + sensor.data.heatStroke.ToString() + " degC" + "\n";
        //string vibrationInfoPanel = "Vibration: " + sensor.data.vibration.ToString() + "" + "\n";
        //string siValueInfoPanel = "SIvalue: " + sensor.data.siValue.ToString() + " kine" + "\n";
        //string pgaInfoPanel = "PGA: " + sensor.data.pga.ToString() + " gal" + "\n";
        //string seismicIntensityInfoPanel = "SeismicIntensity: " + sensor.data.seismicIntensity.ToString() + "" + "\n";

        sensorModel.GetComponentInChildren<ShowMarkerInfo>().mainInfoText.text = ""
        + temperatureInfoPanel
        + humidityInfoPanel
        + ambientLightInfoPanel
        + pressureInfoPanel
        + soundNoiseInfoPanel
        + eTVOCInfoPanel
        + eCO2InfoPanel;
        //+ discomfortInfoPanel
        //+ heatStrokeInfoPanel
        //+ vibrationInfoPanel
        //+ siValueInfoPanel
        //+ pgaInfoPanel
        //+ seismicIntensityInfoPanel;
    }

    private void TurnOnLastSensorInfoPanel(bool state)
    {
        if (lastSensorActiveId != null)
        {
            GameObject listOfSensors = GameObject.Find("SharedPlayground/TableAnchor/GameObject/Map/3D Markers");
            GameObject sensorModel = listOfSensors.transform.GetChild(Int32.Parse(lastSensorActiveId) - 1).gameObject;
            sensorModel.transform.GetChild(0).GetChild(0).gameObject.SetActive(state);
        }        
    }

    public void UpdateSensorData()
    {
        GetSensorData();

        GameObject listOfSensors = GameObject.Find("SharedPlayground/TableAnchor/GameObject/Map/3D Markers");
        int sensorCounter = 0;
        foreach (Sensor sensor in sensorList.sensor)
        {
            GameObject sensorModel = listOfSensors.transform.GetChild(sensorCounter).gameObject;
            sensorModel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            SetInfoPanel(sensor, sensorModel);
            sensorModel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            sensorCounter++;
        }

        TurnOnLastSensorInfoPanel(true);
    }
}
