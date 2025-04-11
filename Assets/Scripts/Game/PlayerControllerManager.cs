using UnityEngine;
using Photon.Pun;
using System.IO;
// using UnityEngine;

public class PlayerControllerManager : MonoBehaviour
{
    PhotonView view;
    GameObject controller; // The instantiated player
    public Item[] items; // Items (e.g., SingleShotGun) on the manager object

    void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (view.IsMine)
        {
            CreateController();
        }
    }

    void Update()
    {
        if (view.IsMine) // Only local player should shoot
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    items[i].Use();
                }
            }
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
        // Pass the controller reference to items (e.g., SingleShotGun)
        foreach (Item item in items)
        {
            if (item is SingleShotGun gun)
            {
                gun.SetPlayerController(controller);
            }
        }
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}