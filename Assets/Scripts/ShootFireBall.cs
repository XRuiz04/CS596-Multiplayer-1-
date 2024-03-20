using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootFireBall : NetworkBehaviour
{
    [SerializeField] private GameObject fireball; // Prefab of the fireball to shoot.
    [SerializeField] private Transform shootTransform; // The transform from which the fireball will be shot.
    [SerializeField] private List<GameObject> spawnedFireBalls = new List<GameObject>(); // Tracks all spawned fireballs.
    private AudioSource src; // Reference to the AudioSource component.
    [SerializeField] private AudioClip Fireball; // Audio clip for the fireball sound.

    // Start is called before the first frame update.
    // Initializes the audio source component.
    private void Start()
    {
        src = GetComponent<AudioSource>();
    }

    // Update is called once per frame.
    // Checks for input to shoot fireballs and calls the appropriate RPC.
    void Update()
    {
        if (!IsOwner) return;
        //Shoot fireball when clicking left mouse using the position and the direction of shooting transform
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootServerRpc();      
        }

    }

    // A server RPC for handling the shooting logic.
    // Instantiates a fireball, assigns it a parent, spawns it, and plays the fireball sound.
    [ServerRpc]
    private void ShootServerRpc()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (!playerMovement.isAlive) return; //Prevents dead player from shooting

        GameObject go = Instantiate(fireball, shootTransform.position, shootTransform.rotation);
        spawnedFireBalls.Add(go);
        go.GetComponent<MoveProjectile>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();

        src.clip = Fireball;
        src.Play();
    }

    // A server RPC to destroy the first spawned fireball.
    // Ensures the object is despawned from the network and destroyed locally.
    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        GameObject toDestroy = spawnedFireBalls[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedFireBalls.Remove(toDestroy);
        Destroy(toDestroy);
    }


}
