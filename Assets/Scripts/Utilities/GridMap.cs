using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    private int height;
    private int width;
    private float cellSize;
    private float[,] gridArray;
    private string sensorDataType = "temperature";
    private Point[] baseSensors;
    private Vector2[] listOfSensorsInScene;
    private Vector2[] convexHullFigure;


    [SerializeField] private GameObject heatMapController;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject heatMapFloor;
    [SerializeField] JsonDataExtraction jsonDataExtraction;

    private Vector3 mapTopLeftCorner = new Vector3(1.85f, -1.5f, 7f);
    private Vector3 mapBottomRightCorner = new Vector3(-2f, -1.5f, 3.15f);

    ///////////////////////////////////
    //May change this to the JsonDataExtraction Class

    private SensorsList sensorList = new SensorsList();

    [System.Serializable]
    private class Point
    {
        public int id;
        public int x;
        public int y;
        public float value;
        public float distance = 0;
    }

    public void ShowHeatMapData()
    {
        if (heatMapFloor.transform.childCount == 0)
        {
            CreateGridMap();
        }
    }

    public void CreateGridMap(float cellSize = 0.2f)
    {
        this.cellSize = cellSize;

        Vector2 gridDimensions = CalculateGrid();

        this.height = (int)gridDimensions.x;
        this.width = (int)gridDimensions.y;
        this.gridArray = new float[width, height];

        DrawGrid(Color.green);

        GetSensorData();

        SetBaseSensorsData();
        CalculateConvexHull();
        SetSensorsOnGrid();
    }

    private void SetBaseSensorsData()
    {
        GameObject markers = GameObject.Find("SharedPlayground/TableAnchor/GameObject/Map/3D Markers");

        int sensorLooper = 0;
        int numberOfSensors = markers.transform.childCount;

        listOfSensorsInScene = new Vector2[numberOfSensors];
        baseSensors = new Point[numberOfSensors];

        foreach (Sensor sensor in sensorList.sensor)
        {
            GameObject sensorInScene = markers.transform.GetChild(sensorLooper).gameObject;
            Vector2 sensorInScenePosition = GetWorldPosition(sensorInScene);

            listOfSensorsInScene[sensorLooper] = sensorInScenePosition;

            Point sensorInHeatMap = new Point();
            sensorInHeatMap.id = sensor.id;

            SetSensorValue(sensorInHeatMap, sensor);

            sensorInHeatMap.x = (int)sensorInScenePosition.x;
            sensorInHeatMap.y = (int)sensorInScenePosition.y;

            baseSensors[sensorLooper] = sensorInHeatMap;
            sensorLooper++;
        }
    }

    private void SetSensorsOnGrid()
    {
        SetBaseValues(baseSensors);
        InterpolateValues(baseSensors);
        ShowGridMatrix();

        //Creates the HeatMap
        heatMapController.GetComponent<HeatMapController>().SetParameters(width, height, cellSize, gridArray, sensorDataType);
    }

    private Vector2 GetWorldPosition(GameObject sensorPostion, float xOffset = 1.95f, float yOffset = 7f)
    {
        float x = sensorPostion.transform.position.x;
        float y = sensorPostion.transform.position.z;

        x = (xOffset - x) / cellSize;
        y = (yOffset - y) / cellSize;

        return new Vector2(y, x);
    }

    private void SetBaseValues(Point[] basePoints)
    {
        for (int i = 0; i < basePoints.Length; i++)
        {
            gridArray[basePoints[i].x, basePoints[i].y] = basePoints[i].value;
        }
    }

    private void InterpolateValues(Point[] basePoints, int numberOfPoints = 3)
    {
        for (int i = 0; i < width; i ++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!IsBasePoint(i, j, basePoints) && IsPointInsideConvexHull(new Vector2(i,j)))
                {
                    Point newPoint = new Point();
                    newPoint.x = i;
                    newPoint.y = j;

                    Point[] closestPoints = GetClosestPoints(newPoint, basePoints, numberOfPoints);

                    newPoint.value = GetInterpolatedValue(newPoint, closestPoints);
                    gridArray[i, j] = newPoint.value;
                }
            }
        }
    }

    private float GetInterpolatedValue(Point point, Point[] closestPoints)
    {
        float numerator = 0;
        float denominator = 0;

        foreach (Point closePoint in closestPoints)
        {
            numerator += (float)(closePoint.value / closePoint.distance);
            denominator += (float)(1 / closePoint.distance);
        }

        return numerator / denominator;
    }

    private Boolean IsBasePoint(int x, int y, Point[] basePoints)
    {
        foreach (Point point in basePoints)
        {
            if(point.x == x && point.y == y)
            {
                return true;
            }
        }
        return false;
    }

    private Point[] GetClosestPoints(Point pointToCalculate, Point[] basePoints, int numberOfPoints)
    {
        int filledSpots = 0;
        int idCurrentBiggest = -1;
        Point[] closestPoints = new Point[numberOfPoints];

        foreach (Point point in basePoints)
        {
            point.distance = CalculateEuclidianDistance(pointToCalculate, point);

            if (filledSpots < numberOfPoints)
            {
                closestPoints[filledSpots] = point;
                filledSpots++;
                continue;
            }

            for (int i = 0; i < numberOfPoints; i++)
            {
                if (idCurrentBiggest < 0)
                {
                    if (closestPoints[i].distance > point.distance)
                    {
                        idCurrentBiggest = i;
                    }
                }
                else
                {
                    if (closestPoints[i].distance > closestPoints[idCurrentBiggest].distance)
                    {
                        idCurrentBiggest = i;
                    }
                }                
            }

            if (idCurrentBiggest >= 0)
            {
                closestPoints[idCurrentBiggest] = point;
            }

            idCurrentBiggest = -1;
        }
        return closestPoints;
    }

    private float CalculateEuclidianDistance(Point point1, Point point2)
    {
        double x = Math.Pow(Math.Abs(point1.x - point2.x), 2);
        double y = Math.Pow(Math.Abs(point1.y - point2.y), 2);
        return (float)Math.Sqrt(x + y);
    }

    private void DrawGrid(Color color)
    {
        Vector3 baseStartX = new Vector3(1.85f, -1.5f, 7f);
        Vector3 baseEndX = new Vector3(1.85f + cellSize, -1.5f, 7f);
        Vector3 baseStartZ = new Vector3(1.85f, -1.5f, 7f);
        Vector3 baseEndZ = new Vector3(1.85f, -1.5f, 7f + cellSize);

        for (int i = 1; i <= width; i++)
        {
            for (int j = 1; j <= height; j++)
            {
                float xOffset = - cellSize * j;
                float zOffset = - cellSize * i;
                Vector3 xStart = new Vector3(baseStartX.x + xOffset , baseStartX.y, baseStartX.z + zOffset);
                Vector3 xEnd = new Vector3(baseEndX.x + xOffset, baseEndX.y, baseEndX.z + zOffset);
                Vector3 zStart = new Vector3(baseStartZ.x + xOffset, baseStartZ.y, baseStartZ.z + zOffset);
                Vector3 zEnd = new Vector3(baseEndZ.x + xOffset, baseEndZ.y, baseEndZ.z + zOffset);
                Debug.DrawLine(xStart, xEnd, color, 60);
                Debug.DrawLine(zStart, zEnd, Color.red, 60);
            }
        } 
    }

    private Vector2 CalculateGrid()
    {
        float heightDistance = Math.Abs((mapTopLeftCorner.z - mapBottomRightCorner.z) / cellSize);
        float widthDistance = Math.Abs((mapTopLeftCorner.x - mapBottomRightCorner.x) / cellSize);

        return new Vector2(widthDistance, heightDistance);
    }

    public void GetSensorData()
    {
        sensorList = JsonUtility.FromJson<SensorsList>(jsonDataExtraction.GetJsonData());
    }

    public void UpdateGrid()
    {
        GetSensorData();
        ChangeDataType(sensorDataType);
    }

    public void ChangeDataType(string newDataType)
    {
        this.sensorDataType = newDataType;
        int sensorLooper = 0;

        foreach (Sensor sensor in sensorList.sensor)
        {
            Point sensorInHeatMap = baseSensors[sensorLooper];
            sensorInHeatMap.id = sensor.id;
            SetSensorValue(sensorInHeatMap, sensor);
            sensorLooper++;
        }
        SetSensorsOnGrid();
    }

    private void ShowGridMatrix()
    {
        string output = "";
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                output += gridArray[i, j] + "\t|";
            }
            output += "\n";
        }
        Debug.Log(output);
    }

    private bool IsPointInsideConvexHull(Vector2 point)
    {
        bool result = false;
        int j = convexHullFigure.Length - 1;
        for (int i = 0; i < convexHullFigure.Length; i++)
        {
            if (convexHullFigure[i].y < point.y && convexHullFigure[j].y >= point.y ||
                convexHullFigure[j].y < point.y && convexHullFigure[i].y >= point.y)
            {
                if (convexHullFigure[i].x + (point.y - convexHullFigure[i].y) /
                    (convexHullFigure[j].y - convexHullFigure[i].y) *
                    (convexHullFigure[j].x - convexHullFigure[i].x) < point.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;       
    }

    private void CalculateConvexHull()
    {
        ConvexHullCalculation convexHull = new ConvexHullCalculation();
        convexHullFigure = convexHull.CalculateConvexHull(listOfSensorsInScene);
    }

    private void SetSensorValue(Point sensorInHeatMap, Sensor sensor)
    {
        switch (sensorDataType)
        {
            case "temperature":
                sensorInHeatMap.value = sensor.data.temperature;
                break;
            case "humidity":
                sensorInHeatMap.value = sensor.data.humidity;
                break;
            case "ambientLight":
                sensorInHeatMap.value = sensor.data.ambientLight;
                break;
            case "pressure":
                sensorInHeatMap.value = sensor.data.pressure;
                break;
            case "soundNoise":
                sensorInHeatMap.value = sensor.data.soundNoise;
                break;
            case "eTVOC":
                sensorInHeatMap.value = sensor.data.eTVOC;
                break;
            case "eCO2":
                sensorInHeatMap.value = sensor.data.eCO2;
                break;
            case "discomfort":
                sensorInHeatMap.value = sensor.data.discomfort;
                break;
            case "heatStroke":
                sensorInHeatMap.value = sensor.data.heatStroke;
                break;
            case "vibration":
                sensorInHeatMap.value = sensor.data.vibration;
                break;
            case "siValue":
                sensorInHeatMap.value = sensor.data.siValue;
                break;
            case "pga":
                sensorInHeatMap.value = sensor.data.pga;
                break;
            case "seismicIntensity":
                sensorInHeatMap.value = sensor.data.seismicIntensity;
                break;
            default:
                Debug.Log(sensorDataType+" is NOT a valid data type");
                break;
        }
    }
}
