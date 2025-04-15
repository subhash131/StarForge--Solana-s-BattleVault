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

}