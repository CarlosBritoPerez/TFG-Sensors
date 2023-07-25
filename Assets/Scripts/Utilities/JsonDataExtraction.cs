using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class SensorsList
{
    public string date;
    public Sensor[] sensor;
}

[System.Serializable]
public class Sensor
{
    public int id;
    public Location location;
    public SensorData data;
}

[System.Serializable]
public class SensorData
{
    public float temperature;
    public float humidity;
    public float ambientLight;
    public float pressure;
    public float soundNoise;
    public float eTVOC;
    public float eCO2;
    public float discomfort;
    public float heatStroke;
    public float vibration;
    public float siValue;
    public float pga;
    public float seismicIntensity;
}

[System.Serializable]
public class Location
{
    public float[] coordinates;
}

public class JsonDataExtraction : MonoBehaviour
{
    [SerializeField] SensorController sensorController;
    [SerializeField] GridMap gridMap;
    [SerializeField] HeatMapController heatMapController;

    public TextAsset jsonFileStart;
    public TextAsset jsonFileEnd;
    public bool InterpolateJsons = false;

    private SensorsList sensorList = new SensorsList();
    private SensorsList sensorListStart = new SensorsList();
    private SensorsList sensorListEnd = new SensorsList();

    private int numberOfSensors;

    

    void Start()
    {
        ReadJsonFile();
        
        //Counting the number of sensors active
        foreach (Sensor sensor in sensorList.sensor)
        {
            numberOfSensors++;
        }
    }

    public void StartUpdatingJsonData()
    {
        InvokeRepeating("UpdateJsonData", 0f, 10f);
    }

    public void UpdateJsonData()
    {
        GenerateNewJsonData();

        sensorController.UpdateSensorData();
        gridMap.UpdateGrid();
        heatMapController.UpdateHeatMap();
    }
        
    public string GetJsonData()
    {
        return JsonUtility.ToJson(this.sensorList);
    }

    public void GenerateNewJsonData()
    {
        foreach (Sensor sensor in sensorList.sensor)
        {
            //                                                                              Range of normal values
            sensor.data.temperature = sensor.data.temperature - Random.Range(-2.0f,2.0f);           //10-30
            sensor.data.humidity = sensor.data.humidity - Random.Range(-3.0f, 3.0f);                //30-60
            sensor.data.ambientLight = sensor.data.ambientLight - Random.Range(-1000.0f, 1000.0f);  //1000-100000 depending of clouds and time of the day
            sensor.data.pressure = sensor.data.pressure - Random.Range(-2.0f, 2.0f);                //80-100
            sensor.data.soundNoise = sensor.data.soundNoise - Random.Range(-10.0f, 10.0f);          //20-130
            sensor.data.eTVOC = sensor.data.eTVOC - Random.Range(0.0f, 0.0f);                       //
            sensor.data.eCO2 = sensor.data.eCO2 - Random.Range(0.0f, 0.0f);                         //
            sensor.data.discomfort = sensor.data.discomfort - Random.Range(0.0f, 0.0f);             //
            sensor.data.heatStroke = sensor.data.heatStroke - Random.Range(0.0f, 0.0f);             //
            sensor.data.vibration = sensor.data.vibration - Random.Range(0.0f, 0.0f);               //
            sensor.data.siValue = sensor.data.siValue - Random.Range(0.0f, 0.0f);                   //
            sensor.data.pga = sensor.data.pga - Random.Range(0.0f, 0.0f);                           //
            sensor.data.seismicIntensity = sensor.data.seismicIntensity - Random.Range(0.0f, 0.0f); //
        }
    }

    public int GetNumberOfSensors()
    {
        return numberOfSensors;
    }

    public void ReadJsonFile()
    {
        if (InterpolateJsons)
        {
            sensorListStart = JsonUtility.FromJson<SensorsList>(jsonFileStart.text);
            sensorListEnd = JsonUtility.FromJson<SensorsList>(jsonFileEnd.text);
        }
        else
        {
            sensorList = JsonUtility.FromJson<SensorsList>(jsonFileStart.text);
        }        
    }

    //private void CreateNewJsons()
    //{

    //}

    //private void InterpolateValues(SensorsList sensorList, int numberOfNewJsons)
    //{
    //    for (int i = 0; i < numberOfNewJsons; i++)
    //    {
    //        foreach (Sensor sensor in sensorList.sensor)
    //        {
    //            sensor.data.temperature += 0;
    //            sensor.data.humidity += 0;
    //            sensor.data.ambientLight += 0;
    //            sensor.data.pressure += 0;
    //            sensor.data.soundNoise += 0;
    //            sensor.data.eTVOC += 0;
    //            sensor.data.eCO2 += 0;
    //            sensor.data.discomfort += 0;
    //            sensor.data.heatStroke += 0;
    //            sensor.data.vibration += 0;
    //            sensor.data.siValue += 0;
    //            sensor.data.pga += 0;
    //            sensor.data.seismicIntensity += 0;
    //        }
    //        newPoint.value = GetInterpolatedValue(newPoint, closestPoints);
    //        gridArray[i, j] = newPoint.value;
    //    }
    //}

    //private float GetInterpolatedValue(Point point, Point[] closestPoints)
    //{
    //    float numerator = 0;
    //    float denominator = 0;
    //    foreach (Point closePoint in closestPoints)
    //    {
    //        numerator += (float)(closePoint.value / closePoint.distance);
    //        denominator += (float)(1 / closePoint.distance);
    //    }
    //    return numerator / denominator;
    //}

}
