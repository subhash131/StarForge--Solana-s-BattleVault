using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float cameraLagFactor = 0.2f; // Controls how much the camera lags (0 = no lag, 1 = max lag)

    private Vector3 cameraInitialLocalPosition; // Camera's initial local position relative to player
    private Vector3 cameraTargetLocalPosition;
    private Vector3 cameraVelocity; // For smooth damp

    private void Start()
    {
        // Store the camera's initial local position relative to the player
        cameraInitialLocalPosition = fpsCamera.transform.localPosition;
        cameraTargetLocalPosition = cameraInitialLocalPosition;
    }

    private void FixedUpdate()
    {
        // Player movement
        Vector3 playerVelocity = new Vector3(
            joystick.Horizontal * moveSpeed,
            playerRigidbody.velocity.y,
            joystick.Vertical * moveSpeed
        );
        playerRigidbody.velocity = playerVelocity;

        // Camera trailing effect: Adjust the camera's local position with a lag
        cameraTargetLocalPosition = cameraInitialLocalPosition; // Reset to initial position as base
        Vector3 cameraOffset = new Vector3(
            -joystick.Horizontal * moveSpeed * cameraLagFactor, // Small offset opposite to movement
            0f, // Keep Y stable
            -joystick.Vertical * moveSpeed * cameraLagFactor
        );

        // Apply the offset to the target position
        cameraTargetLocalPosition += cameraOffset;

        // Smoothly move the camera toward the target local position
        fpsCamera.transform.localPosition = Vector3.SmoothDamp(
            fpsCamera.transform.localPosition,
            cameraTargetLocalPosition,
            ref cameraVelocity,
            0.1f // Smoothing time
        );
    }
}