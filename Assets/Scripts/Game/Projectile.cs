using UnityEngine;
using Photon.Pun;
public class Projectile : MonoBehaviour{
    public float speed = 20f;
    private Rigidbody rb;
    
    void Start() {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5f); 
    }

     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting){
            // stream.SendNext(rb.linearVelocity);
        }
        else{
            // rb.linearVelocity = (Vector3)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void InitializeProjectileMovement(Vector3 direction){
        if (rb == null){
            rb = GetComponent<Rigidbody>();
        }
        rb.AddForce(direction * speed, ForceMode.Impulse);
    }
}
