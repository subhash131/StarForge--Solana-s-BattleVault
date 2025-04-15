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
        // Base spawn position
        Vector3 basePosition = new Vector3(5f, 1f, 5f);

        // Calculate offset based on ActorNumber (ActorNumber starts at 1)
        float offset = (PhotonNetwork.LocalPlayer.ActorNumber - 1) * 20f; // 20 units apart per player
        Vector3 spawnPosition = basePosition + new Vector3(offset, 0f, 0f);


        controller = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefab", "Player"),
            spawnPosition,
            Quaternion.identity,
            0,
            new object[] { view.ViewID }
        );
        Debug.Log("Player instantiated for ViewID: " + view.ViewID);
    }

}