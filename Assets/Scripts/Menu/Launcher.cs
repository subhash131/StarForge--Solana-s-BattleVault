using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks{


    public static Launcher instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text USERNAME;
    [SerializeField] TMP_Text errorText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    public GameObject startButton;

    void Awake() {
    if (instance == null) {
        instance = this;
    } else {
        Destroy(gameObject);
    }
}

    void Start(){
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
        USERNAME.text = SolanaManager.instance.playerName;
    }

    public override void OnConnectedToMaster(){
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene=true;
    }

    public override void OnJoinedLobby(){
        MenuManager.instance.OpenMenu("TitleMenu");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 10000).ToString("0000");
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

    public void Back(){
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
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

    public void StartGame(){
        PhotonNetwork.LoadLevel(2);
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
