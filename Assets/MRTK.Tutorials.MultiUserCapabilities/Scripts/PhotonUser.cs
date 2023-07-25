using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using UnityEngine;
//using RootMotion.FinalIK;
using TMPro;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonUser : MonoBehaviour, IMixedRealitySourceStateHandler
    {
        //public MultiplayerController multiplayerController;

        public GameObject leftHand;
        public GameObject rightHand;
        public GameObject head;
        public GameObject pelvis;

        public GameObject hair;
        public GameObject eyes;
        public GameObject eyeslash;
        public GameObject eyesbrow;
        public GameObject teeth;
        public GameObject tongue;
        public GameObject skin;

        public TextMeshPro userDisplay;

        public GameObject defaultLeftHandPosition;
        public GameObject defaultRightHandPosition;
        public GameObject pelvisPosition;
        public TextMeshPro textMesh;

        private PhotonView pv;
        private string username;

        private bool leftHandDetected = false;
        private bool rightHandDetected = false;
        private float pelvisHeight;
        private float handsLHeight;
        private float handsRHeight;

        //public VRIK vrik;

        public bool invertedSkeleton = false;

        private void Start()
        {
            pv = GetComponent<PhotonView>();
            //multiplayerController = ApplicationController.GetInstance().multiplayerController;

            if (!pv.IsMine) return;

            hair.SetActive(false);
            eyes.SetActive(false);
            skin.SetActive(false);
            eyeslash.SetActive(false);
            eyesbrow.SetActive(false);
            teeth.SetActive(false);
            tongue.SetActive(false);

            if (PlayerPrefs.HasKey("firstname"))
            {
                if (PlayerPrefs.GetString("firstname").StartsWith("guestuser"))
                {
                    username = "Guest " + pv.Owner.ActorNumber.ToString();
                }
                else
                {
                    username = PlayerPrefs.GetString("firstname");
                }
            }
            else
            {
                username = "User " + pv.Owner.ActorNumber.ToString();//"User" + PhotonNetwork.NickName;
            }
            pv.RPC("PunRPC_SetNickName", RpcTarget.AllBuffered, username);
            pv.RPC("RPC_PlayConnectedSound", RpcTarget.Others);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            pelvisHeight = head.transform.position.y - pelvisPosition.transform.position.y;
            handsLHeight = head.transform.position.y - defaultLeftHandPosition.transform.position.y;
            handsRHeight = head.transform.position.y - defaultRightHandPosition.transform.position.y;
        }

        void OnDestroy()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        }
        /**
        private void LateUpdate()
        {
            if (!pv.IsMine) return;

            head.transform.position = ApplicationController.GetInstance().headSolver.transform.position;
            head.transform.rotation = ApplicationController.GetInstance().headSolver.transform.rotation;
            pelvis.transform.position = new Vector3(ApplicationController.GetInstance().headSolver.transform.position.x, 
                head.transform.position.y - pelvisHeight, ApplicationController.GetInstance().headSolver.transform.position.z);

            if (invertedSkeleton)
            {
                leftHand.transform.position = ApplicationController.GetInstance().leftHandSolver.transform.position;
                leftHand.transform.rotation = ApplicationController.GetInstance().leftHandSolver.transform.rotation;
                rightHand.transform.position = ApplicationController.GetInstance().rightHandSolver.transform.position;
                rightHand.transform.rotation = ApplicationController.GetInstance().rightHandSolver.transform.rotation;
            }
            else
            {

                if (leftHandDetected)
                {
                    leftHand.transform.position = ApplicationController.GetInstance().leftHandSolver.transform.position;
                    leftHand.transform.rotation = ApplicationController.GetInstance().leftHandSolver.transform.rotation;
                }
                else
                {
                    leftHand.transform.position = new Vector3(defaultLeftHandPosition.transform.position.x,
                        head.transform.position.y - handsLHeight, defaultLeftHandPosition.transform.position.z);
                    leftHand.transform.rotation = defaultLeftHandPosition.transform.rotation;
                }
                if (rightHandDetected)
                {
                    rightHand.transform.position = ApplicationController.GetInstance().rightHandSolver.transform.position;
                    rightHand.transform.rotation = ApplicationController.GetInstance().rightHandSolver.transform.rotation;
                }
                else
                {
                    rightHand.transform.position = new Vector3(defaultRightHandPosition.transform.position.x,
                        head.transform.position.y - handsRHeight, defaultRightHandPosition.transform.position.z);
                    rightHand.transform.rotation = defaultRightHandPosition.transform.rotation;
                }
            }
        }

        [PunRPC]
        private void PunRPC_SetNickName(string nName)
        {
            if (gameObject.GetComponent<CutOutShaderGeneric>() != null)
                gameObject.GetComponent<CutOutShaderGeneric>().enabled = true;
            gameObject.name = nName;
            textMesh.text = nName;
        }

        [PunRPC]
        private void RPC_PlayConnectedSound()
        {
            Debug.Log("Play Sound");
            ApplicationController.GetInstance().soundController.PlaySound(SoundController.SoundType.CONNECTION);
        }

        [PunRPC]
        private void TrackLeftArm(int track, int vid)
        {
            PhotonView.Find(vid).GetComponent<PhotonUser>().vrik.solver.leftArm.positionWeight = track;
            PhotonView.Find(vid).GetComponent<PhotonUser>().vrik.solver.leftArm.rotationWeight = track;
        }

        [PunRPC]
        private void TrackRightArm(int track, int vid)
        {
            PhotonView.Find(vid).GetComponent<PhotonUser>().vrik.solver.rightArm.positionWeight = track;
            PhotonView.Find(vid).GetComponent<PhotonUser>().vrik.solver.rightArm.rotationWeight = track;
        }

        [PunRPC]
        private void PunRPC_ShareAzureAnchorId(string anchorId)
        {
            Debug.Log("AZURE: entered PunRPC_ShareAzureAnchorId");
            GenericNetworkManager.Instance.azureAnchorId = anchorId;
            ApplicationController.GetInstance().multiplayerController.spatialAnchorID = anchorId;
            if (ApplicationController.GetInstance().multiplayerController.isSameRoom) {
                ApplicationController.GetInstance().spatialAnchorController.RelocateSpatialAnchor();
                Debug.Log("Joined the same room");
            }
            //ApplicationController.GetInstance().spatialAnchorController.currentAnchorID = anchorId;


            Debug.Log("\n AZURE: PhotonUser.PunRPC_ShareAzureAnchorId()");
            Debug.Log("AZURE: GenericNetworkManager.instance.azureAnchorId= " + GenericNetworkManager.Instance.azureAnchorId);
            Debug.Log("AZURE: Azure Anchor ID shared by user: " + pv.Controller.UserId);
        }
        */
        public void ShareAzureAnchorId()
        {
            Debug.LogError("AZURE: sharing anchor ID");
            if (pv != null)
                pv.RPC("PunRPC_ShareAzureAnchorId", RpcTarget.AllBuffered, GenericNetworkManager.Instance.azureAnchorId);
            else
                Debug.LogError("PV is null");
        }
        
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            IMixedRealityHand hand = eventData.Controller as IMixedRealityHand;

            if (hand != null)
            {
                if (invertedSkeleton)
                {
                    if (hand.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
                    {
                        pv.RPC("TrackLeftArm", RpcTarget.AllBuffered, 1, GetComponent<PhotonView>().ViewID);
                        leftHandDetected = true;
                    }
                    else if (hand.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
                    {
                        pv.RPC("TrackRightArm", RpcTarget.AllBuffered, 1, GetComponent<PhotonView>().ViewID);
                        rightHandDetected = true;
                    }
                }
                else
                {
                    if (hand.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
                    {
                        pv.RPC("TrackLeftArm", RpcTarget.AllBuffered, 1, GetComponent<PhotonView>().ViewID);
                        leftHandDetected = true;
                    }
                    else if (hand.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
                    {
                        pv.RPC("TrackRightArm", RpcTarget.AllBuffered, 1, GetComponent<PhotonView>().ViewID);
                        rightHandDetected = true;
                    }
                }
            }
        }
        
        public void OnSourceLost(SourceStateEventData eventData)
        {
            IMixedRealityHand hand = eventData.Controller as IMixedRealityHand;

            if (hand != null)
            {
                if (invertedSkeleton)
                {
                    if (hand.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
                    {
                        pv.RPC("TrackLeftArm", RpcTarget.AllBuffered, 0, GetComponent<PhotonView>().ViewID);
                        leftHandDetected = false;
                    }
                    else if (hand.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
                    {
                        pv.RPC("TrackRightArm", RpcTarget.AllBuffered, 0, GetComponent<PhotonView>().ViewID);
                        rightHandDetected = false;
                    }
                }
                else
                {
                    if (hand.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
                    {
                        pv.RPC("TrackLeftArm", RpcTarget.AllBuffered, 0, GetComponent<PhotonView>().ViewID);
                        leftHandDetected = false;
                    }
                    else if (hand.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
                    {
                        pv.RPC("TrackRightArm", RpcTarget.AllBuffered, 0, GetComponent<PhotonView>().ViewID);
                        rightHandDetected = false;
                    }
                }
            }
        }
    }
}
