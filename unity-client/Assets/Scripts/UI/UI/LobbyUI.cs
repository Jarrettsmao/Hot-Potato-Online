using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button joinCreateButton;
    [SerializeField] private Button leaveButton;

    [Header("Player List")]
    [SerializeField] private Transform playerProfileContainer;  // The Grid Layout parent

    void Start()
    {
        NetworkManager.Instance.OnMessageReceived += OnMessageReceived;

        joinCreateButton.onClick.AddListener(OnJoinStatusClicked);
        startButton.onClick.AddListener(OnStartClicked);
        startButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnJoinStatusClicked()
    {
        Debug.Log("Join Status Clicked");
        // Implement join status logic here
    }

    void OnStartClicked()
    {
        Debug.Log("Start Clicked");
        // Implement start game logic here
    }

    void OnMessageReceived(ServerMessage message)
    {
        Debug.Log("On Message Received in LobbyUI");
    }
}
