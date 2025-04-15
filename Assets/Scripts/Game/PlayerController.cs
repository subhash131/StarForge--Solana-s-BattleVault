using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviourPunCallbacks{
    private PhotonView view;
    private GameObject controller;

    void Awake(){
        view = GetComponent<PhotonView>();
    }
    private void Start(){
        if (view.IsMine){
            CreateController();
        }
    }

    void CreateController()
    {
        controller = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefab", "Player"),
            new Vector3(5f, 1f, 5f),
            Quaternion.identity,
            0,
            new object[] { view.ViewID }
        );
        Debug.Log("Player instantiated for ViewID: " + view.ViewID);
    }

}