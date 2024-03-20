using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class AutoDestroy : NetworkBehaviour
{
    public float delayBeforeDestroy = 5f; // Time in seconds before the object is destroyed.
    private AudioSource src; // Reference to the AudioSource component.
    [SerializeField] private AudioClip Burn; // Audio clips for burn sound.

    // Start is called before the first frame update.
    // It initializes the audio source and plays the burn sound clip.
    private void Start()
    {
        src = GetComponent<AudioSource>();
        src.clip = Burn;
        src.Play();
    }

    // A server RPC to handle the destruction of particles on the server side.
    // It despawns the network object and destroys the game object after a specified delay.
    [ServerRpc(RequireOwnership = false)]
    private void DestroyParticlesServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject, delayBeforeDestroy);
    }

}
