using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
   public Transform playerTransform;
   public Transform cameraTransform;

   [Header("Camera Movement")]
   public Transform cameraPivot;
   private Vector3 cameraFollowVelocity = Vector3.zero;
   public float cameraFollowSpeed = 0.3f;
   public float lookAngle;
   public float pivotAngle;
   public float cameraLookSpeed = 2;
   public float cameraPivotSpeed = 2;

   public float minPivotAngle = -5f;
   public float maxPivotAngle = 5f;


    [Header("Camera Collision")]
    public LayerMask collisionLayer;
    private float defaultPosition;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 0.2f;
    private Vector3 cameraVectorPosition;

    private PlayerMovement playerMovement;

    IEnumerator Start(){
        yield return new WaitUntil(() => FindFirstObjectByType<PlayerManager>() != null);
        playerTransform = FindFirstObjectByType<PlayerManager>().transform;
        inputManager = FindFirstObjectByType<InputManager>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
    }

    public void HandleAllCameraMovement(){
        FollowTarget();
        RotateCamera(); 
        CameraCollision();
    }


    private void FollowTarget(){
        if(transform && playerTransform){
            Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerTransform.position,ref cameraFollowVelocity, cameraFollowSpeed);
            transform.position = targetPosition;      
        }  
    }

    private void RotateCamera(){

        if(!inputManager)return;

        Vector3 rotation;
        Quaternion targetRotation;
        lookAngle += inputManager.cameraInputX * cameraLookSpeed;
        pivotAngle += inputManager.cameraInputY * cameraPivotSpeed;

        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);
        

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;


        if(playerMovement.isMoving == false && playerMovement.isSprinting == false){
            playerTransform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
    }

    void CameraCollision(){

        if(!cameraTransform) return;

        float targetPosition =defaultPosition;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        bool sphere = Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out RaycastHit hit, Mathf.Abs(targetPosition),collisionLayer);

        if(sphere){
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }
        if(Mathf.Abs(targetPosition) < minCollisionOffset){
            targetPosition -= minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
