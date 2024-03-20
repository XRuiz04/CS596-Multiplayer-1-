using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private Button hostButton; // Button to start hosting a game.
    [SerializeField] private Button clientButton; // Button to join a game as a client.
    [SerializeField] private TextMeshProUGUI playersCountText; // Text element to display the number of players.

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone); // Tracks the number of players.

    // Awake is called when the script instance is being loaded.
    // It sets up the listeners for the host and client buttons.
    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

    // Update is called once per frame.
    // It updates the displayed player count and, if the current instance is the server, updates the playersNum variable.
    private void Update()
    {
        playersCountText.text = "Players: " + playersNum.Value.ToString();

        if (!IsServer) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}
