using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandMenuInitializer : MonoBehaviour
{
    [SerializeField]
    GameObject menu;
    [SerializeField]
    GameObject initialMenu;
    [SerializeField]
    Image loader;

    private bool loading = false;
    private float startTime = 0;
    private float loadTime = 0.7f;

    private bool imReady = false;

    // Start is called before the first frame update
    void Start()
    {
        //transform.localPosition = new Vector3();

        Invoke("OnReady", 4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnReady() {
        transform.localPosition = new Vector3();
        gameObject.SetActive(false);
                    
        //LOGGIN WITHOUT PIN (HIDE PIN CANVAS)
        //room.id = 0;
        //room.roomName = "DEFAULT";
        //room.latitude = 0;
        //room.longitude = 0;
        //room.zoom = 0;
        //ApplicationController.GetInstance().apiController.LogginByUserPassword("site@admin.com", "site?admin2021");
        Invoke("ImReady", 1f);

    }

    public void SetVisible()
    {
        if (imReady)
        {
            gameObject.SetActive(true);
        }
        else
        {
            return;
        }
        if (!PhotonNetwork.IsConnected)
        {
            initialMenu.SetActive(true);
            menu.SetActive(false);
        }
        else
        {
            LoadSetVisible();
            InitializeToolsPanel();
            initialMenu.SetActive(false);
            //ApplicationController.GetInstance().lookAtYourHandPanel.SetActive(false);
        }
    }

    public void UndoSetVisible()
    {
        loading = false;
        loader.fillAmount = 0;
    }

    public void LoadSetVisible()
    {
        if (loading) return;
        loading = true;
        startTime = Time.time;
        StartCoroutine(StartLoading());
    }

    IEnumerator StartLoading()
    {
        while (Time.time - startTime < loadTime)
        {
            if (!loading)
            {
                loader.fillAmount = 0;
                yield break;
            }
            yield return new WaitForEndOfFrame();
            loader.fillAmount = Mathf.Lerp(0, 1, (Time.time - startTime) / loadTime);
        }
        if (loading)
        {
            menu.transform.position = transform.position;
            menu.transform.rotation = transform.rotation; //Quaternion.Euler(new Vector3(menu.transform.eulerAngles.x, transform.eulerAngles.y, menu.transform.eulerAngles.z));
            menu.SetActive(true);
        }
        loader.fillAmount = 0;
        loading = false;
    }

    public void SetVisibleAndPlace(Transform transform)
    {
        gameObject.SetActive(true);
        if (!PhotonNetwork.IsConnected)
        {
            initialMenu.SetActive(true);
            this.transform.parent.position = transform.position;
            menu.SetActive(false);
        }
        else
        {
            LoadSetVisible();
            initialMenu.SetActive(false);
            //ApplicationController.GetInstance().lookAtYourHandPanel.SetActive(false);
        }
    }

    public void ImReady()
    {
        Debug.Log("Im ready is called");
        imReady = true;
    }

    private void InitializeToolsPanel()
    {
        //ApplicationController.GetInstance().newUXController.GetButtonById("Tools:Panel").gameObject.SetActive(false);
        //ApplicationController.GetInstance().sceneController.ChangeBetweenOnAirOnMap(true);
        //ApplicationController.GetInstance().newUXController.GetButtonById("Main:Tools").SetButtonSettingsStateOff();
        //ApplicationController.GetInstance().newUXController.GetButtonById("Tools:Panel").gameObject.SetActive(false);
    }
}
