using Photon.Pun;
using UnityEngine;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class GenericTransformNetSync : MonoBehaviourPun, IPunObservable
    {
        //[SerializeField] private bool isUser = default;

        private Vector3 networkLocalPosition;
        private Quaternion networkLocalRotation;
        private Vector3 startingLocalPosition;
        private Quaternion startingLocalRotation;

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
            }
            else
            {
                networkLocalPosition = (Vector3) stream.ReceiveNext();
                networkLocalRotation = (Quaternion) stream.ReceiveNext();
            }
        }

        private void Start()
        {
            var trans = transform;
            startingLocalPosition = trans.localPosition;
            startingLocalRotation = trans.localRotation;
            networkLocalPosition = startingLocalPosition;
            networkLocalRotation = startingLocalRotation;
        }

        // private void FixedUpdate()
        private void LateUpdate()
        {
            var trans = transform;
            trans.localPosition = networkLocalPosition;
            trans.localRotation = networkLocalRotation;
        }
    }
}
