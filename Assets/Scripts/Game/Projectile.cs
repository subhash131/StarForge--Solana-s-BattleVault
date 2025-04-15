using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour{
    public float speed = 20f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5f); // Destroy after 5 seconds
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         // Send position and velocity
    //         stream.SendNext(rb.position);
    //         stream.SendNext(rb.velocity);
    //     }
    //     else
    //     {
    //         // Receive position and velocity
    //         rb.position = (Vector3)stream.ReceiveNext();
    //         rb.velocity = (Vector3)stream.ReceiveNext();
    //     }
    // }
}