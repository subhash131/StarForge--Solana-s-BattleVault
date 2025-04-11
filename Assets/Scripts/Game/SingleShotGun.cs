using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SingleShotGun : Gun
{
    Camera cam;
    public GameObject projectilePrefab;
    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 1f; 
    private GameObject playerController; 

    public override void Use()
    {
        Shoot();
    }

    public void SetPlayerController(GameObject player)
    {
        playerController = player;
        Debug.Log("Player controller set to: " + playerController.name + " at " + playerController.transform.position);
    }

    void Start()
    {
        cam = Camera.main;
        Debug.Log("Camera: " + (cam != null ? cam.name : "null"));
        if (!playerController)
        {
            Debug.LogWarning("Player controller not set yet!");
        }
    }

    void Shoot()
    {
        if (!cam)
        {
            cam = Camera.main;
            if (!cam)
            {
                Debug.LogError("Main Camera not found!");
                return;
            }
        }

        if (!playerController)
        {
            Debug.LogError("Player controller not assigned! Cannot shoot.");
            return;
        }

        Vector3 targetPoint;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = (targetPoint - cam.transform.position).normalized;

        // Use the instantiated player's position
        Vector3 localOffset = new Vector3(offsetX, offsetY, offsetZ);
        Vector3 spawnPosition = playerController.transform.position + playerController.transform.TransformDirection(localOffset);
        Debug.Log("Spawning projectile at: " + spawnPosition + " | Player at: " + playerController.transform.position);

        GameObject projectile = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefab", "Projectile"),
            spawnPosition,
            Quaternion.identity
        );

        projectile.GetComponent<PhotonView>().RPC("InitializeProjectileMovement", RpcTarget.All, direction);
    }
}