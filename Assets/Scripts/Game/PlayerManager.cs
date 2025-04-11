using UnityEngine;
using Photon.Pun;


public class PlayerManager : MonoBehaviour{

    InputManager inputManager;
    PlayerMovement playerMovement;
    CameraManager cameraManager;
    PhotonView view;


    void Awake(){
        view = GetComponent<PhotonView>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        if(view.IsMine) {
            cameraManager = FindFirstObjectByType<CameraManager>();
        }
    }

    void Start(){
        if(!view.IsMine) {
            CameraManager cam = GetComponentInChildren<CameraManager>();
            if (cam != null) {
                Destroy(cam.gameObject);
            } else {
                Debug.LogWarning("🚨 No CameraManager found in PlayerManager! Check your hierarchy.");
            }
        }
    }


    void Update(){
        if(!view.IsMine) return;
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate() {
        if(!view.IsMine) return;
        playerMovement.HandleAllMovement();
    }

    void LateUpdate(){
          if(!view.IsMine) return;
        cameraManager.HandleAllCameraMovement();
    }

}
