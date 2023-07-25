using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapController : MonoBehaviour
{
    [SerializeField] private GameObject heatMapPrefab;
    [SerializeField] private GameObject heatMapFloor;

    private int height;
    private int width;

    private float cellSize;
    private float slope;
    private float height2DMode = 0.1f;
    private float[,] gridArray;

    private bool alreadyGenerated = false;
    private bool mode2DActive = false;

    private string dataType;
    private string dataTypeSymbol = "ºC";

    private Vector2 heightScale;

    private GameObject[,] heatMapTileArray;

    public static HeatMapController instance;

    private bool infoPanelActive = false;

    private Vector2 coordinatesActivePanel;

    //Scale of the height of the tiles
    private Vector2 baseScale = new Vector2(1, 30);

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

    public void SetParameters(int width, int height, float cellSize, float[,] gridArray, string dataType)
    {
        this.height = height;
        this.width = width;
        this.cellSize = cellSize;
        this.gridArray = gridArray;
        this.dataType = dataType;
        this.heightScale = GetHeightScale();

        this.slope = (baseScale.y - baseScale.x) / (heightScale.y - heightScale.x);

        heatMapTileArray = new GameObject[width, height];

        GenerateHeatMap();
    }

    public void GenerateHeatMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tileGridPosition = new Vector2(i, j);

                if (alreadyGenerated)
                {
                    GameObject heatMapTile = heatMapFloor.transform.GetChild(j + i * height).gameObject;
                    ChangeTilePrefab(tileGridPosition, heatMapTile);
                }
                else
                {
                    Vector3 tilePosition = new Vector3(1.95f - (j + 1) * cellSize, -1.5f, 7f - (i + 1) * cellSize);
                    CreateTilePrefab(tilePosition, tileGridPosition);
                }
            }
        }
        alreadyGenerated = true;
    }

    private void CreateTilePrefab(Vector3 tilePosition, Vector2 tileGridPosition)
    {
        float tileValue = gridArray[(int)tileGridPosition.x, (int)tileGridPosition.y];

        GameObject heatMap = GameObject.Instantiate(heatMapPrefab, tilePosition, Quaternion.identity);
        heatMap.transform.parent = heatMapFloor.transform;

        heatMapTileArray[(int)tileGridPosition.x, (int)tileGridPosition.y] = heatMap;

        //float baseHeight = heightScale.x - 1;

        heatMap.transform.localScale = new Vector3(10f, 10f, 10f);
        heatMap.transform.GetChild(1).localScale = new Vector3(cellSize * 10, CalculateTileHeight(tileValue) * 0.1f, cellSize * 10); 

        Color customColor = GetTileColor(tileValue);

        heatMap.transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material.color = customColor;

        heatMap.GetComponentInChildren<ShowMarkerInfo>().position = tileGridPosition;
        HideTilesOutOfConvexHull(tileValue, heatMap);
    }

    private void ChangeTilePrefab(Vector2 tileGridPosition, GameObject heatMapTile)
    {
        float tileValue = gridArray[(int)tileGridPosition.x, (int)tileGridPosition.y];

        float baseHeight = heightScale.x - 1;

        heatMapTile.transform.GetChild(1).localScale = new Vector3(cellSize * 10, CalculateTileHeight(tileValue) * 0.1f, cellSize * 10);

        Color customColor = GetTileColor(tileValue);

        heatMapTile.transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material.color = customColor;
    }

    private Vector2 GetHeightScale()
    {
        Vector2 heightScale;

        //This method returns a scale from the lowest value to the hightest
        //heightScale = CalculateHeightScale();

        //This method returns a custom scale for each type of data
        heightScale = GetHeightScaleOfType();

        return heightScale;
    }

    private float CalculateTileHeight(float value)
    {
        if (mode2DActive)
        {
            return height2DMode;
        }

        return baseScale.x + slope * (value - heightScale.x);
    }

    public void UpdateHeatMap()
    {
        if (coordinatesActivePanel.x != -1)
        {
            SetInfoPanel(coordinatesActivePanel);
        }
    }

    private Vector2 GetHeightScaleOfType()
    {
        Vector2 heightScale = new Vector2(0,0);
        switch (dataType)
        {
            case "temperature":
                heightScale = new Vector2(0, 40);
                dataTypeSymbol = "ºC";
                Debug.Log("Scale: Temp");
                break;
            case "humidity":
                heightScale = new Vector2(10, 80);
                dataTypeSymbol = "%RH";
                Debug.Log("Scale: hum");
                break;
            case "ambientLight":
                heightScale = new Vector2(1000, 100000);
                dataTypeSymbol = "lx";
                break;
            case "pressure":
                heightScale = new Vector2(60, 120);
                dataTypeSymbol = "kPa";
                break;
            case "soundNoise":
                heightScale = new Vector2(20, 130);
                dataTypeSymbol = "dB";
                break;
            //The rest are NOT implemented yet
            case "eTVOC":
                heightScale = new Vector2(0,0);
                break;
            case "eCO2":
                heightScale = new Vector2(0, 0);
                break;
            case "discomfort":
                heightScale = new Vector2(0, 0);
                break;
            case "heatStroke":
                heightScale = new Vector2(0, 0);
                break;
            case "vibration":
                heightScale = new Vector2(0, 0);
                break;
            case "siValue":
                heightScale = new Vector2(0, 0);
                break;
            case "pga":
                heightScale = new Vector2(0, 0);
                break;
            case "seismicIntensity":
                heightScale = new Vector2(0, 0);
                break;
            default:
                Debug.Log(dataType + " is NOT a valid data type");
                break;
        }

        return heightScale;
    }

    private Vector2 CalculateHeightScale()
    {
        float[] arrayOfMinimuns = new float[gridArray.Length];
        float[] arrayOfMaximuns = new float[gridArray.Length];

        int i = 0;
        foreach (float line in gridArray)
        {
            float min = Mathf.Min(line);
            arrayOfMinimuns[i] = min;
            float max = Mathf.Max(line);
            arrayOfMaximuns[i] = max;
            i++;
        }

        float minValue = Mathf.Min(arrayOfMinimuns);
        float maxValue = Mathf.Max(arrayOfMaximuns);
        Vector2 heightScale = new Vector2(minValue, maxValue);

        return heightScale;
    }

    private Color GetTileColor(float cellValue)
    {
        float transparency = 0.5f;
        float colorGradient = (cellValue - heightScale.x) / (heightScale.y - heightScale.x);
        Color customColor = new Color(0.0f + colorGradient, 0.0f, 1.0f - colorGradient, transparency);
        return customColor;
    }

    private void SetInfoPanel(Vector2 tileGridPosition)
    {
        int x = (int)tileGridPosition.x;
        int y = (int)tileGridPosition.y;

        GameObject heatMapTile = heatMapFloor.transform.GetChild(x * height + y).gameObject;
        string tileValue = MathF.Round(gridArray[x, y], 1).ToString("0.0");
        heatMapTile.GetComponentInChildren<ShowMarkerInfo>().titleText.text = tileValue + dataTypeSymbol;      
    }

    public void HeatMapTileActive(Vector2 tileGridPosition)
    {
        if (coordinatesActivePanel == tileGridPosition)
        {
            //Resets the coordinates
            coordinatesActivePanel.x = -1;
            coordinatesActivePanel.y = -1;
        }
        else 
        {
            SetInfoPanel(tileGridPosition);
            if (coordinatesActivePanel.x > -1)
            {
                int heatMapFloorChild = (int)coordinatesActivePanel.x * height + (int)coordinatesActivePanel.y;
                GameObject.Find("HeatMapFloor").transform.GetChild(heatMapFloorChild).GetChild(0).gameObject.SetActive(false);
            }
            coordinatesActivePanel = tileGridPosition;
        }
    }

    private void HideTilesOutOfConvexHull(float tileValue, GameObject tile)
    {
        if (tileValue == 0)
        {
            tile.SetActive(false);
        }
    }

    public void ChangeMode(String mode)
    {
        switch (mode)
        {
            case "3D":
                mode2DActive = false;
                break;
            case "2D":
                mode2DActive = true;
                break;
            default:
                Debug.Log(mode + " is not a valid mode");
                break;
        }
        GenerateHeatMap();
    }
}
