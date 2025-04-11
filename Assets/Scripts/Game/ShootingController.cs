using Photon.Pun;
using UnityEngine;

public class ShootingController : MonoBehaviour{

    InputManager inputManager;
    PlayerMovement playerMovement;
    PhotonView view;

    [Header("Shooting Variables")]
    public Transform firePoint;
    public float fireRate = 15f;
    public float fireRange = 100f;
    public float fireDamage = 15;
    public float nextFireTime = 0.2f;

    [Header("Shooting Flags")]
    public bool isShooting;
    public bool isMoving;
    public bool isShootingInput; 


    void Start(){
        view =GetComponent<PhotonView>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update(){
        if(!view.IsMine) return;
        isShootingInput = inputManager.fireInput;
        if(isShootingInput){
            if(Time.time >= nextFireTime){
                nextFireTime = Time.time +1f / fireRate;
                // Shoot();
            }        
            isShooting = true;
        }
    }

    private void Shoot(){
        RaycastHit hit;
        bool flag = Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange);
        if (flag){
            Debug.Log(hit.transform.name);
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;

            PlayerMovement playerMovementDamage = hit.collider.GetComponent<PlayerMovement>();
            if(playerMovementDamage != null){
                playerMovementDamage.ApplyDamage(fireDamage);
                view.RPC("RPC_Shoot", RpcTarget.All, hitPoint, hitNormal);
            }
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPoint, Vector3 hitNormal){
        // Handle the hit effect here, e.g., instantiate a hit effect prefab at hitPoint
        // and orient it according to hitNormal.
        //add animations and effects here
        Debug.Log("Hit Point: " + hitPoint);
        Debug.Log("Hit Normal: " + hitNormal);
    }

}
