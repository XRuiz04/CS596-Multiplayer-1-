using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f; // The speed at which the player moves.
    [SerializeField] private float rotationSpeed = 500f; // The speed of player rotation.
    [SerializeField] private int randomPositionRange = 3; // Range for spawning the player at a random position.
    private Renderer[] renderers; // Array of renderers in the player object.
    private Collider collider; // Reference to the player's collider.
    private AudioSource src; // Reference to the AudioSource component.
    [SerializeField] private AudioClip Movement, Death, Spawn; // Audio clips for movement, death, and spawn sounds.
    private bool isPlayingMovementSound = false; // Tracks if the movement sound is currently playing.
    public bool isAlive = true; // Tracks if the player is alive.

    // Start is called before the first frame update.
    // Initializes the audio source component.
    void Start()
    {
        src = GetComponent<AudioSource>();
    }

    // Called when the object has been spawned on the network.
    // Initializes components and moves the player to a random position.
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Get references to components
        renderers = GetComponentsInChildren<Renderer>();
        collider = GetComponentInChildren<Collider>();
        UpdatePositionClientRpc();
    }

    // Coroutine to play movement sound repeatedly while moving.
    IEnumerator PlayMovementSoundCoroutine()
    {
        isPlayingMovementSound = true;
        while (true)
        {
            if (!src.isPlaying)
            {
                src.clip = Movement;
                src.Play();
            }
            yield return new WaitForSeconds(Movement.length); // Wait for the length of the movement sound clip.
        }
    }

    // Update is called once per frame to handle movement and rotation.
    void Update()
    {
        if (!IsOwner || !isAlive) return;

        // Capture the player's input.
        float horizontalInput = Input.GetAxis("Vertical");
        float verticalInput = Input.GetAxis("Horizontal");

        // Calculate the movement direction and normalize it.
        Vector3 movementDirection = new Vector3(-horizontalInput, 0, verticalInput);
        movementDirection.Normalize();

        // Move the player if there's input.
        if (movementDirection.magnitude > 0)
        {
            transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);

            // Play movement sound if not already playing.
            if (!src.isPlaying || src.clip != Movement)
            {
                src.clip = Movement;
                src.Play();
            }
        }
        else if (src.isPlaying && src.clip == Movement)
        {
            // Stop the movement sound if the player stops moving.
            src.Stop();
        }

        //Rotate the player to face the movement direction.
        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }


    }

    // Method to handle the player's death.
    public void Die()
    {
        if (IsServer)
        {
            isAlive = false;
            StartCoroutine(Respawn());
            SetPlayerActiveClientRpc(false); // Disable player components on all clients.
        }
    }

    // ClientRpc to toggle player's active state, affecting rendering and collision.
    [ClientRpc]
    void SetPlayerActiveClientRpc(bool isActive)
    {
        // Enable or disable renderers and collider based on the isActive flag
        foreach (var renderer in renderers)
        {
            renderer.enabled = isActive;
        }
        collider.enabled = isActive;
    }

    // Coroutine to handle respawning of the player after death.
    private IEnumerator Respawn()
    {
        src.clip = Death;
        src.Play();

        yield return new WaitForSeconds(3); // Wait for 3 seconds before respawning.
        isAlive = true;

        src.clip = Spawn;
        src.Play();

        SetPlayerActiveClientRpc(true); // Enable player components on all clients.
        UpdatePositionClientRpc(); // Update the player position on respawn.
    }

    // ClientRpc to randomly update the player's position upon respawn or initial spawn.
    [ClientRpc]
    private void UpdatePositionClientRpc()
    {
        transform.position = new Vector3(Random.Range(randomPositionRange, -randomPositionRange), 0, Random.Range(randomPositionRange, -randomPositionRange));
        transform.rotation = new Quaternion(0, 180, 0, 0); // Resets rotation to default facing direction.
    }
 
}
