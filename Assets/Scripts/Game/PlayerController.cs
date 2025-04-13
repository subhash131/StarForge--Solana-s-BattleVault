using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour,IDragHandler
{
    [Header("References")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Camera fpsCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationPerSwipe = 90f; 
    [SerializeField] private float rotationSmoothSpeed = 10f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Camera Lag")]
    [SerializeField] private float cameraLagFactor = 0.2f;

    private Quaternion targetRotation;

    private Vector3 cameraInitialLocalPosition;
    private Vector3 cameraVelocity;

    private float yaw;   // left/right
    private float pitch; // up/down

    private void Start()
    {
        if (!playerRigidbody) playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.useGravity = false;
        targetRotation = playerRigidbody.rotation;

        cameraInitialLocalPosition = fpsCamera.transform.localPosition;
    }

    public void OnSwipe(Vector2 delta)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float horizontalPercent = delta.x / screenWidth;
        float verticalPercent = delta.y / screenHeight;

        yaw += horizontalPercent * rotationPerSwipe;
        pitch -= verticalPercent * rotationPerSwipe;

        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        targetRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float horizontalPercent = eventData.delta.x / screenWidth;
        float verticalPercent = eventData.delta.y / screenHeight;

        yaw += horizontalPercent * rotationPerSwipe;
        pitch -= verticalPercent * rotationPerSwipe;

        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        targetRotation = Quaternion.Euler(pitch, yaw, 0f);
    }



    private void Update()
    {
        // Smoothly rotate player
        if (playerRigidbody.rotation != targetRotation)
        {
            playerRigidbody.MoveRotation(Quaternion.Slerp(
                playerRigidbody.rotation,
                targetRotation,
                rotationSmoothSpeed * Time.deltaTime
            ));
        }
    }

    private void FixedUpdate()
    {
        // Movement
        Vector3 input = new Vector3(joystick.Horizontal, 0f, joystick.Vertical).normalized;
        Vector3 worldDirection = transform.TransformDirection(input) * moveSpeed;
        worldDirection.y = playerRigidbody.velocity.y; // Keep current vertical velocity (if needed)
        playerRigidbody.velocity = worldDirection;

        // Camera lag (optional)
        Vector3 lagOffset = new Vector3(
            -joystick.Horizontal * moveSpeed * cameraLagFactor,
            0f,
            -joystick.Vertical * moveSpeed * cameraLagFactor
        );
        Vector3 targetCameraLocalPos = cameraInitialLocalPosition + lagOffset;
        fpsCamera.transform.localPosition = Vector3.SmoothDamp(
            fpsCamera.transform.localPosition,
            targetCameraLocalPos,
            ref cameraVelocity,
            0.1f
        );
    }
}
