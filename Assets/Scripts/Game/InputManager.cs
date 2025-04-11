using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour{
//    PlayerActions playerActions;

   public Vector2 movementInput;
   public Vector2 cameraMovementInput;

   public float verticalInput;
   public float horizontalInput;

   public float cameraInputX;
   public float cameraInputY;
   public bool fireInput;
   
//    void OnEnable(){
//     if(playerActions == null){
//         playerActions = new PlayerActions();
//         playerActions.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
//         playerActions.PlayerMovement.CameraMovement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();
//         playerActions.PlayerAction.Fire.performed += i => fireInput = true;    
//         playerActions.PlayerAction.Fire.canceled += i => fireInput = false;    
//     }
//     playerActions.Enable();
//    }

//    private void OnDisable(){
//     playerActions.Disable();        
//    }

   public void HandleAllInputs(){
    HandleMovementInput();
   }

    private void HandleMovementInput(){
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        cameraInputX = cameraMovementInput.x;
        cameraInputY = cameraMovementInput.y;
        // fireInput = 
    }
}
