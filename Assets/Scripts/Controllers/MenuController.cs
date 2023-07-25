using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mapBoard;
    [SerializeField] GameObject heatMapFloor;

    [SerializeField] GameObject heatMapMenu;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject showMenu;

    [SerializeField] SensorController SensorController;

    ButtonController lastButtonClicked;

    public void startApplication()
    {
        startButton.SetActive(false);
        showMenu.SetActive(true);
    }

    public void ButtonClickedOn(ButtonController button)
    {
        if (lastButtonClicked != null)
        {
            lastButtonClicked.TurnButtonOff();
        }
        lastButtonClicked = button;
    }

    public void ButtonClickedOff()
    {
        lastButtonClicked = null;
    }

    public void ShowMap()
    {
        ChangeMapMode("3D");
        MoveObjectInOrOutOfSight(mapBoard, -1);
    }

    public void ShowHeatMap()
    {
        heatMapFloor.SetActive(true);         
    }

    public void ShowHeatMapMenu()
    {
        heatMapMenu.SetActive(true);
        ChangeMapMode("2D");
        MoveObjectInOrOutOfSight(mapBoard, -1);
        MoveObjectInOrOutOfSight(mapBoard.transform.GetChild(1).gameObject, 1);
    }

    public void HideMap()
    {
        MoveObjectInOrOutOfSight(mapBoard, 1);
    }

    public void HideHeatMap()
    {
        heatMapFloor.SetActive(false);
    }

    public void HideHeatMapMenu()
    {
        heatMapMenu.SetActive(false);
        MoveObjectInOrOutOfSight(mapBoard, 1);
        MoveObjectInOrOutOfSight(mapBoard.transform.GetChild(1).gameObject, -1);
    }

    private void ChangeMapMode(string mode)
    {
        int scale = 0;
        switch (mode)
        {
            case "3D":
                scale = 3;
                break;
            case "2D":
                scale = 0;
                break;
            default:
                Debug.Log(mode + " is not a valid mode");
                break;
        }
        mapBoard.GetComponentInChildren<OnlineMapsElevationManagerBase>().scale = scale;
        OnlineMaps.instance.Redraw();
    }

    private void MoveObjectInOrOutOfSight(GameObject objectToMove, int direction)
    {
        objectToMove.transform.position += new Vector3(0, 100 * direction, 0);
    }
}
