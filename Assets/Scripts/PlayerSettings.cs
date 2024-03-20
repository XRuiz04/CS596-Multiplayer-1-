using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer; // Reference to the MeshRenderer component for changing player color.
    [SerializeField] private TextMeshProUGUI playerName; // UI element to display the player's name.
    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>(
        "Player: 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // Stores the player's name.
    public List<Color> colors = new List<Color>(); // A list of colors to choose from for player materials.

    // Awake is called when the script instance is being loaded.
    // It initializes the meshRenderer reference.
    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    // Called when the object has been spawned on the network.
    // It sets the player's name and color based on the network ID.
    public override void OnNetworkSpawn()
    {
        networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        playerName.text = networkPlayerName.Value.ToString();
        meshRenderer.material.color = colors[(int)OwnerClientId];
    }


}
