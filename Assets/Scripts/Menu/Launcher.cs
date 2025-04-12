using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks{

    public static Launcher instance;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text roomNameText;
    public TMP_Text errorText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    public GameObject startButton;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public static void ConnectRealtime(){
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster(){
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene=true;
    }

    public override void OnJoinedLobby(){
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 10000).ToString("0000");
        MenuManager.instance.OpenMenu("TitleMenu");
    }

    public void CreateRoom(){
        if(string.IsNullOrEmpty(roomNameInputField.text)){
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public override void OnJoinedRoom(){
        MenuManager.instance.OpenMenu("RoomMenu");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent){
            Destroy(child.gameObject);
        }

        for(int i=0; i<players.Count(); i++){
            GameObject playerItem = Instantiate(playerListItemPrefab, playerListContent);
            playerItem.SetActive(true);
            playerItem.GetComponent<PlayerListItem>().SetUp(players[i]);
         } 

         startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient){
       startButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string ErrorMessage){
       errorText.text = "Failed to create room: " + ErrorMessage;
        MenuManager.instance.OpenMenu("ErrorMenu");
    }

    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public override void OnLeftRoom(){
       MenuManager.instance.OpenMenu("TitleMenu");
    }

    public void JoinRoom(){
        if(string.IsNullOrEmpty(roomNameInputField.text)){
            return;
        }
        PhotonNetwork.JoinRoom(roomNameInputField.text);
    }

    public void startGame(){
        PhotonNetwork.LoadLevel(1);
    }

    public void JoinRoom(RoomInfo info){
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        foreach(Transform trans in roomListContent){
            Destroy(trans.gameObject);
        }
        
        for(int i=0; i<roomList.Count; i++){
            if(roomList[i].RemovedFromList) continue;
            Instantiate(roomListItemPrefab,  roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer){
       GameObject playerItem = Instantiate(playerListItemPrefab, playerListContent);
       playerItem.SetActive(true);
       playerItem.GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
