using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        public static PhotonRoom Room;

        [SerializeField] private GameObject photonUserPrefab = default;
        //[SerializeField] private GameObject roverExplorerPrefab = default;
        //[SerializeField] private Transform roverExplorerLocation = default;

        // private PhotonView pv;
        private Player[] photonPlayers;
        private int playersInRoom;
        private int myNumberInRoom;
        private int avatarId = 0;
        private GameObject player;

        // private GameObject module;
        // private Vector3 moduleLocation = Vector3.zero;

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom++;
            
            //DISCARTING BECAUSE NOW WORKS WITH SCENECONTENTCONTROLLER
            /*
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                ApplicationController.GetInstance().lineDrawController.SendLinesToNewPlayer(newPlayer);
                OnlineMaps.instance?.gameObject?.GetComponent<MapLineController>()?.SendLinesToNewPlayer(newPlayer);
            }
            */
        }

        private void Awake()
        {
            if (Room == null)
            {
                Room = this;
            }
            else
            {
                if (Room != this)
                {
                    Destroy(Room.gameObject);
                    Room = this;
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Start()
        {
            // pv = GetComponent<PhotonView>();

            // Allow prefabs not in a Resources folder
            if (PlayerPrefs.HasKey("AvatarType"))
            {
                avatarId = PlayerPrefs.GetInt("AvatarType");
            }
            if (PhotonNetwork.PrefabPool is DefaultPool pool)
            {
                if (photonUserPrefab != null) pool.ResourceCache.Add(photonUserPrefab.name, photonUserPrefab);

                //if (roverExplorerPrefab != null) pool.ResourceCache.Add(roverExplorerPrefab.name, roverExplorerPrefab);
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom = photonPlayers.Length;
            myNumberInRoom = playersInRoom;
            PhotonNetwork.NickName = myNumberInRoom.ToString();

            StartGame();
        }

        private void StartGame()
        {
            CreatPlayer();

            if (!PhotonNetwork.IsMasterClient) return;

            if (TableAnchor.Instance != null) CreateInteractableObjects();
        }

        private void CreatPlayer()
        {
            player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
            if (PhotonNetwork.LocalPlayer == player.GetComponent<PhotonView>().Owner)
            {
//                PhotonNetwork.LocalPlayer.SetCustomProperties(ApplicationController.GetInstance().apiController.GetProperties());
//                ApplicationController.GetInstance().localPlayer = player;
                //player.GetComponent<PlayerModelController>().SetModel(avatarId);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer == PhotonNetwork.LocalPlayer && PhotonNetwork.LocalPlayer == player.GetComponent<PhotonView>().Owner)
            {
//                player.GetComponent<PlayerModelController>().SetModel(avatarId);
            }
        }

        private void CreateInteractableObjects()
        {
            //var position = roverExplorerLocation.position;
            //var positionOnTopOfSurface = new Vector3(position.x, position.y + roverExplorerLocation.localScale.y / 2,
            //    position.z);
            //
            //var go = PhotonNetwork.Instantiate(roverExplorerPrefab.name, positionOnTopOfSurface,
            //    roverExplorerLocation.rotation);
        }

        // private void CreateMainLunarModule()
        // {
        //     module = PhotonNetwork.Instantiate(roverExplorerPrefab.name, Vector3.zero, Quaternion.identity);
        //     pv.RPC("Rpc_SetModuleParent", RpcTarget.AllBuffered);
        // }
        //
        // [PunRPC]
        // private void Rpc_SetModuleParent()
        // {
        //     Debug.Log("Rpc_SetModuleParent- RPC Called");
        //     module.transform.parent = TableAnchor.Instance.transform;
        //     module.transform.localPosition = moduleLocation;
        // }
    }
}
