using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveProjectile : NetworkBehaviour
{
    public ShootFireBall parent; // Reference to the ShootFireBall script that spawned this projectile.
    [SerializeField] private GameObject hitParticles; // The prefab for particles to spawn on hit.
    [SerializeField] private float shootForce; // The force at which the projectile is shot.
    private Rigidbody rb; // Reference to the Rigidbody component.

    // Start is called before the first frame update.
    // Initializes the rigidbody component.
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame.
    // Applies velocity to the projectile, moving it forward.
    void Update()
    {
        rb.velocity= rb.transform.forward * shootForce;
    }

    // Called when the projectile collides with another collider.
    // If the collider belongs to a player, it calls the Die method on the player.
    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        if (other.gameObject.name == "Capsule")
        {
            PlayerMovement playerMovement = other.GetComponentInParent<PlayerMovement>();
            playerMovement.Die();
        }

        InstantiateHitParticlesServerRpc();
        parent.DestroyServerRpc();
    }

    // A server RPC to handle instantiation of hit particles on the server.
    [ServerRpc]
    private void InstantiateHitParticlesServerRpc()
    {
        //instantiate the hit particles when we collide with something then destory the fireball
        GameObject hitImpact = Instantiate(hitParticles, transform.position, Quaternion.identity);
        hitImpact.GetComponent<NetworkObject>().Spawn();
        hitImpact.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
    }



}
