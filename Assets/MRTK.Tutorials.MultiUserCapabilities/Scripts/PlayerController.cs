using Photon.Pun;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;


namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PlayerController : MonoBehaviourPun, IPunObservable
    {
        public bool isLaserOn = false;
        public Material laserPointerMaterial;
        
        [SerializeField] private bool isUser = default;

        private Camera mainCamera;

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;
        private Vector3 networkLaserPointerOrigin;
        private Vector3 networkLaserPointerDestination;

        private Vector3 startingLocalPosition;
        private Quaternion startingLocalRotation;
        private Ray rightHandRay;
        private GameObject myLine;
        private GameObject myLineToDiscard;
        private LineRenderer lineRenderer;


        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
                stream.SendNext(networkLaserPointerOrigin);
                stream.SendNext(networkLaserPointerDestination);
            }
            else
            {
                networkLocalPosition = (Vector3) stream.ReceiveNext();
                networkLocalRotation = (Quaternion) stream.ReceiveNext();
                networkLaserPointerOrigin = (Vector3)stream.ReceiveNext();
                networkLaserPointerDestination = (Vector3)stream.ReceiveNext();
    }
        }

        private void Start()
        {
            mainCamera = Camera.main;

            if (isUser)
            {
                if (TableAnchor.Instance != null) transform.parent = FindObjectOfType<TableAnchor>().transform;

                if (photonView.IsMine) GenericNetworkManager.Instance.localUser = photonView;
            }

            var trans = transform;
            startingLocalPosition = trans.localPosition;
            startingLocalRotation = trans.localRotation;
            networkLocalPosition = startingLocalPosition;
            networkLocalRotation = startingLocalRotation;
            networkLaserPointerOrigin = new Vector3();
            networkLaserPointerDestination = new Vector3();
            myLine = new GameObject();
            lineRenderer = myLine.AddComponent<LineRenderer>();
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
                if ( isLaserOn && 
                    InputRayUtils.TryGetHandRay(Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right, out rightHandRay)) {
                    Debug.Log("Right hand available: origin = " + rightHandRay.origin + " direction = " + rightHandRay.direction * 4f);
                    networkLaserPointerOrigin = rightHandRay.origin;
                    networkLaserPointerDestination = rightHandRay.origin + rightHandRay.direction;
                } else {
                    networkLaserPointerOrigin = new Vector3();
                    networkLaserPointerDestination = new Vector3();
                }
            }
            
            if (Vector3.Distance(networkLaserPointerOrigin, networkLaserPointerDestination) > 0.1f)  {
                //DrawLine(networkLaserPointerOrigin, networkLaserPointerDestination, Color.red);
            }

            DrawLine(transform.position + new Vector3(1f, -0.2f, 0.4f), transform.position + new Vector3(1,2,4), Color.red);
        }
        void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            //if (myLine != null) {
            //    myLineToDiscard = myLine;
            //    myLine = new GameObject();
            //    GameObject.Destroy(myLineToDiscard);
            //}

            myLine.transform.position = start;
            
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            //lr.SetPositions(new Vector3[] ())
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            lineRenderer.material = laserPointerMaterial; 
        }

    }
}
