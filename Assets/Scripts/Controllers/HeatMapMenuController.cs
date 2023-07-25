using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapMenuController : MonoBehaviour
{
    [SerializeField] GameObject heatMapMenu;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject showMenu;

    ButtonController lastButtonClicked;

    public void ButtonClicked(ButtonController button)
    {
        if (lastButtonClicked != null)
        {
            lastButtonClicked.TurnButtonOff();
        }
        lastButtonClicked = button;
    }
}
