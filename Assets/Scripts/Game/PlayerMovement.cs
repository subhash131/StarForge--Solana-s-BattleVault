using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;



public class PlayerMovement : MonoBehaviour{


    [Header("Player Health")]
    const float maxHealth = 150f;
    public float currentHealth = maxHealth;

    public Slider healthBarSlider;
    public GameObject playerUI;


    [Header("Ref and physics")]
    InputManager inputManager;
    PlayerManager playerManager;
    PlayerControllerManager playerControllerManager;
    Vector3 moveDirection;
    Transform cameraGameObject;
    Rigidbody playerRigidbody;
    public float movementSpeed = 15f;
    public float rotationSpeed = 5f;

    public bool isMoving = false;
    public bool isSprinting =false;

    PhotonView view;

    void Awake(){

        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraGameObject = Camera.main.transform;
        view = GetComponent<PhotonView>();

        playerControllerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControllerManager>();
        
        healthBarSlider.minValue = 0f;
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;    
    }

    void Start(){
        playerManager = GetComponent<PlayerManager>();

        if(!view.IsMine){
            Destroy(playerUI);
        }
        
    }


    public void HandleAllMovement(){
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement(){
        moveDirection = new Vector3(cameraGameObject.forward.x, 0, cameraGameObject.forward.z ) * inputManager.verticalInput;
        moveDirection = cameraGameObject.forward * inputManager.verticalInput;
        moveDirection += cameraGameObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;

        moveDirection *= movementSpeed;

        Vector3 movementVelocity = moveDirection;
        // playerRigidbody.linearVelocity = movementVelocity;
        
    }

    void HandleRotation(){
        Vector3 targetDirection = cameraGameObject.forward * inputManager.verticalInput;
        targetDirection += cameraGameObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero){
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = playerRotation;
        
    }


    public void ApplyDamage(float damageValue){
        view.RPC("RPC_TakeDamage", RpcTarget.All, damageValue);
    }

    [PunRPC]
    void RPC_TakeDamage(float damageValue){
        if(!view.IsMine) return;
        currentHealth -= damageValue;
        healthBarSlider.value = currentHealth;
        if(currentHealth <= 0){
            Die();
        }
        Debug.Log("Damage: " + damageValue);
        Debug.Log("Current Health: " + currentHealth);
    }   

    private void Die(){
        playerControllerManager.Die();
    }
}
