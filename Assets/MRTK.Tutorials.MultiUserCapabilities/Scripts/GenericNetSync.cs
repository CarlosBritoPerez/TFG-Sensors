using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class GenericNetSync : MonoBehaviourPun, IPunObservable
    {
        public enum AvatarColorIndexes { WHITE, GREEN, BLUE, CYAN, RED, MAGENTA, YELLOW }

        public string displayName = "";
        public TextMeshPro displayNameTMP;
        public Material laserPointerMaterial;
        public GameObject avatarGameObject;
        public AvatarColorIndexes avatarColor;
        public Dictionary<AvatarColorIndexes, Color> avatarColorDictionary;
        public Color currentAvatarColor;
        public Color currentAvatarColorBase;
        public Color currentAvatarColorWire;

        [SerializeField] private bool isUser = default;

        private Camera mainCamera;
        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;
        private Vector3 networkLaserPointerOrigin;
        private Vector3 networkLaserPointerDestination;
        private Vector3 startingLocalPosition;
        private Quaternion startingLocalRotation;
        private Ray rightHandRay;
        private Material avatarMaterial;
        public int avatarType;

        private LineRenderer lineRenderer;

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
                stream.SendNext(networkLaserPointerOrigin);
                stream.SendNext(networkLaserPointerDestination);
                stream.SendNext(displayName);
                //stream.SendNext(lineRenderer.GetComponent<LaserController>().isLaserOn);
                stream.SendNext((int)avatarColor);
            }
            else
            {
                networkLocalPosition = (Vector3)stream.ReceiveNext();
                networkLocalRotation = (Quaternion)stream.ReceiveNext();
                networkLaserPointerOrigin = (Vector3)stream.ReceiveNext();
                networkLaserPointerDestination = (Vector3)stream.ReceiveNext();
                displayName = (string)stream.ReceiveNext();
                //lineRenderer.GetComponent<LaserController>().isLaserOn = (bool)stream.ReceiveNext();
                avatarColor = (AvatarColorIndexes)stream.ReceiveNext();
            }
        }

        private void Start()
        {
            mainCamera = Camera.main;
            avatarColor = (AvatarColorIndexes)Random.Range(0, 7);
            if (isUser)
            {
                if (TableAnchor.Instance != null) transform.parent.parent = FindObjectOfType<TableAnchor>().transform;
                if (photonView.IsMine) GenericNetworkManager.Instance.localUser = photonView;
                var trans = transform;
                startingLocalPosition = trans.localPosition;
                startingLocalRotation = trans.localRotation;
                networkLocalPosition = startingLocalPosition;
                networkLocalRotation = startingLocalRotation;
                networkLaserPointerOrigin = new Vector3();
                networkLaserPointerDestination = new Vector3();
                avatarColorDictionary = new Dictionary<AvatarColorIndexes, Color>();
                InitializeAvatarColor();
                InitializeDisplayName();
                //lineRenderer = ApplicationController.GetInstance().CreateLine(currentAvatarColor);
                DrawLine(networkLaserPointerOrigin, networkLaserPointerDestination);

                Debug.Log("Lets RegisterPlayer = " + photonView.ViewID);
                //ApplicationController.GetInstance().multiplayerController.RegisterPlayer(photonView.ViewID, gameObject);
                if (photonView.IsMine && isUser)
                {
                    //ApplicationController.GetInstance().multiplayerController.localPlayerViewID = photonView.ViewID;
                }
                //Debug.Log("LocalPlayer = " + ApplicationController.GetInstance().multiplayerController.localPlayerViewID);
            }
        }
        private void OnDestroy()
        {
            //ApplicationController.GetInstance().multiplayerController.UnRegisterPlayer(photonView.ViewID);
        }

        // private void FixedUpdate()
        private void Update()
        {
            if (!photonView.IsMine)
            {
                var trans = transform;
                trans.localPosition = networkLocalPosition;
                trans.localRotation = networkLocalRotation;
            }

            if (photonView.IsMine && isUser)
            {
                var trans = transform;
                var mainCameraTransform = mainCamera.transform;
                trans.position = mainCameraTransform.position;
                trans.rotation = mainCameraTransform.rotation;
                //Debug.Log("Right HAND available?");
                /*if (InputRayUtils.TryGetHandRay(Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right, out rightHandRay))
                {
                    //Debug.Log("Right HAND available: origin = " + rightHandRay.origin + " direction = " + rightHandRay.direction);
                    networkLaserPointerOrigin = rightHandRay.origin;
                    networkLaserPointerDestination = rightHandRay.origin + rightHandRay.direction;
                }
                else
                {
                    networkLaserPointerOrigin = new Vector3();
                    networkLaserPointerDestination = new Vector3();
                }*/
            }

            /*if (!lineRenderer.GetComponent<LaserController>().isLaserOn)
            {
                networkLaserPointerOrigin = new Vector3();
                networkLaserPointerDestination = new Vector3();
            }
            if (isUser) {
                DrawLine(networkLaserPointerOrigin, networkLaserPointerOrigin + (networkLaserPointerDestination - networkLaserPointerOrigin) * 10f);
            }*/
        }

        void DrawLine(Vector3 start, Vector3 end)
        {
            if (lineRenderer != null)
            {
                lineRenderer.gameObject.transform.position = start;
                lineRenderer.startWidth = 0.005f;
                lineRenderer.endWidth = 0.005f;
                lineRenderer.startColor = currentAvatarColor;
                lineRenderer.endColor = currentAvatarColor;
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);
                lineRenderer.material = laserPointerMaterial;
                lineRenderer.material.color = currentAvatarColor;
            }
        }

        public void ApplyLaserPointer(bool value)
        {
            Debug.Log("ApplyLaserPointer, viewID = " + photonView.ViewID + " , value = " + value);
            //lineRenderer.GetComponent<LaserController>().isLaserOn = value;
            if (!value)
            {
                networkLaserPointerOrigin = new Vector3();
                networkLaserPointerDestination = new Vector3();
                DrawLine(networkLaserPointerOrigin, networkLaserPointerDestination);
            }
        }

        public void ApplyCustomLaserPointer(bool value, Vector3 origin, Vector3 destination)
        {
            Debug.Log("ApplyLaserPointer, viewID = " + photonView.ViewID + " , value = " + value);
            //lineRenderer.GetComponent<LaserController>().isLaserOn = value;
            if (!value)
            {
                DrawLine(origin, destination);
            }
        }

        private void InitializeAvatarColor()
        {
            //avatarColor = (AvatarColorIndexes) (PhotonNetwork.LocalPlayer.ActorNumber % 7);
            int index = photonView.ViewID / 1000;
            avatarColor = (AvatarColorIndexes)(index % 7);
            Debug.Log("PhotonNetwork.LocalPlayer.ActorNumber = " + PhotonNetwork.LocalPlayer.ActorNumber + " , avatarColor = " + avatarColor);
            UpdateAvatarColor();
        }

        private void InitializeDisplayName()
        {
            if (displayNameTMP != null)
            {
                displayName = "User " + (photonView.ViewID / 1000).ToString();
                displayNameTMP.text = displayName;
            }
        }

        public void ApplyAvatarColor(AvatarColorIndexes value)
        {
            avatarColor = value;
            UpdateAvatarColor();
        }

        private void UpdateAvatarColor()
        {
            if (avatarGameObject != null)
            {
                currentAvatarColor = Color.white;
                currentAvatarColorWire = Color.white;
                currentAvatarColorBase = Color.black;
                if (avatarColor == AvatarColorIndexes.GREEN)
                {
                    //currentAvatarColor = Color.green;
                    currentAvatarColor = new Color(0.07489803f, 0.3207547f, 0.0499288f);
                    currentAvatarColorBase = new Color(0.05097561f, 0.22f, 0);
                    currentAvatarColorWire = new Color(0.05126891f, 0.596f, 0);
                }
                else if (avatarColor == AvatarColorIndexes.BLUE)
                {
                    //currentAvatarColor = Color.blue;
                    currentAvatarColor = new Color(0.03114987f, 0.1010185f, 0.2641509f);
                    currentAvatarColorBase = new Color(0.0501513f, 0.1010485f, 0.2169811f);
                    currentAvatarColorWire = new Color(0.125304f, 0.2973053f, 0.681f);
                }
                else if (avatarColor == AvatarColorIndexes.CYAN)
                {
                    //currentAvatarColor = Color.cyan;
                    currentAvatarColor = new Color(0.1902367f, 0.5377358f, 0.645f);
                    currentAvatarColorBase = new Color(0.1039756f, 0.2907073f, 0.348f);
                    currentAvatarColorWire = new Color(0.2309573f, 0.6457378f, 0.773f);
                }
                else if (avatarColor == AvatarColorIndexes.RED)
                {
                    //currentAvatarColor = Color.red;
                    currentAvatarColor = new Color(0.447f, 0f, 0f);
                    currentAvatarColorBase = new Color(0.3f, 0, 0);
                    currentAvatarColorWire = new Color(0.8f, 0, 0);
                }
                else if (avatarColor == AvatarColorIndexes.MAGENTA)
                {
                    //currentAvatarColor = Color.magenta;
                    currentAvatarColor = new Color(0.709f, 0.3130562f, 0.589f);
                    currentAvatarColorBase = new Color(0.4f, 0.12f, 0.316f);
                    currentAvatarColorWire = new Color(0.8f, 0.32f, 0.6526734f);
                }
                else if (avatarColor == AvatarColorIndexes.YELLOW)
                {
                    //currentAvatarColor = Color.yellow;
                    currentAvatarColor = new Color(0.8584906f, 0.7456911f, 0.1984247f);
                    currentAvatarColorBase = new Color(0.504f, 0.417f, 0);
                    currentAvatarColorWire = new Color(0.9f, 0.7757142f, 0.18f);
                }
                Material currentAvatarMaterial = new Material(avatarGameObject.GetComponent<SkinnedMeshRenderer>().material);
                //currentAvatarMaterial.color = currentAvatarColor;
                /*if (avatarType == 0)
                {
                    currentAvatarMaterial.SetColor("_BaseColor", currentAvatarColorBase);
                    currentAvatarMaterial.SetColor("_WireColor", currentAvatarColorWire);
                    transform.parent.GetComponent<PhotonUser>().hair.GetComponent<SkinnedMeshRenderer>().material.SetColor("_BaseColor", currentAvatarColorBase);
                    transform.parent.GetComponent<PhotonUser>().hair.GetComponent<SkinnedMeshRenderer>().material.SetColor("_WireColor", currentAvatarColorWire);
                    transform.parent.GetComponent<PlayerHMDController>().SetColorTriangulate(0, currentAvatarColorBase, currentAvatarColorWire);
                }
                else if (avatarType == 1)
                {
                    currentAvatarMaterial.SetColor("_ColorDecal", currentAvatarColor);
                }*/
                /*else if (avatarType == 2)
                {
                    currentAvatarMaterial.SetColor("_Color", currentAvatarColor);
                }*/
                currentAvatarMaterial.SetColor("_Color", currentAvatarColor);
                //currentAvatarMaterial.SetColor("_Fill_Color_", currentAvatarColor);
                //currentAvatarMaterial.SetColor("_Line_Color_", Color.black);
                avatarGameObject.GetComponent<SkinnedMeshRenderer>().material = currentAvatarMaterial;
            }
        }

        public void ApplyAvatarName(string value)
        {
            displayName = value;
            //discomment for use web name
            //displayNameTMP.text = displayName;
        }

    }
}
