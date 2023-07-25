using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour, IMixedRealityFocusHandler, IMixedRealityPointerHandler, IMixedRealityTouchHandler
{
    [SerializeField] Material HoverOnColor;
    [SerializeField] Material HoverOffColor;
    [SerializeField] Material OnColor;
    [SerializeField] Material OffColor;

    [SerializeField] GameObject Button;
    [SerializeField] GameObject ButtonBorder;

    [SerializeField] GameObject MenuController;

    public bool buttonChanges = true;

    [SerializeField]
    private bool isClicked = false;

    [Header("Methods On/Off")]
    public UnityEvent invokeMethodOn;
    public UnityEvent invokeMethodOff;

    public TextMeshPro label;


    private void Start()
    {
        if (isClicked)
        {
            ChangeState();
            OnClick();
        }
    }
    public void OnClick()
    {
        if (!isClicked)
        {
            MenuController.GetComponent<MenuController>().ButtonClickedOn(this);
            invokeMethodOn?.Invoke();
            ChangeColor(Button, OnColor);
            
        }
        else
        {
            MenuController.GetComponent<MenuController>().ButtonClickedOff();
            invokeMethodOff?.Invoke();
            ChangeColor(Button, OffColor);
        }
        ChangeState();
    }

    private void ChangeState()
    {
        if (buttonChanges)
        {
            isClicked = !isClicked;
        }
    }

    public void TurnButtonOff()
    {
        OnClick();
    }

    private void ChangeColor(GameObject buttonPart, Material buttonColor)
    {
        buttonPart.GetComponentInChildren<MeshRenderer>().material = buttonColor;
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        Debug.Log("CLICK Button " + gameObject.name + " has been clicked.");
        OnClick();
    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        Debug.Log("TOUCH Button " + gameObject.name + " has been clicked.");
        OnClick();
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        ChangeColor(ButtonBorder, HoverOnColor);
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        ChangeColor(ButtonBorder, HoverOffColor);
    }


    #region unused
    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {

    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {

    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {

    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {

    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {

    }
    #endregion
}