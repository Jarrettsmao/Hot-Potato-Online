using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
// using System.Diagnostics;

public class LobbyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button joinCreateButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button randomButton;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField roomIdInputField;

    [Header("Player List")]
    [SerializeField] private Transform playerProfileContainer;  // The Grid Layout parent
    [SerializeField] private TMP_Text playerListTitleText;
    private List<GameObject> playerProfiles = new List<GameObject>();
    
    private NetworkManager nm;

    void Start()
    {
        nm = NetworkManager.Instance;
        nm.OnMessageReceived += OnMessageReceived;

        joinCreateButton.onClick.AddListener(OnJoinCreateClicked);
        startButton.onClick.AddListener(OnStartClicked);
        leaveButton.onClick.AddListener(OnLeaveClicked);
        randomButton.onClick.AddListener(OnRandomClicked);

        startButton.gameObject.SetActive(false);
        leaveButton.gameObject.SetActive(false);

        HideAllPlayerProfiles();
    }

    void OnDestroy()
    {
        if (nm != null)
        {
            nm.OnMessageReceived -= OnMessageReceived;
        }
    }

    void OnMessageReceived(ServerMessage message)
    {
        switch (message.type)
        {
            case "JOIN_SUCCESS":
                Debug.Log("✅ Joined room successfully!");
                UpdatePlayerListTitle();  
                joinCreateButton.gameObject.SetActive(false);
                leaveButton.gameObject.SetActive(true);
                break;

            case "LEAVE_SUCCESS":
                Debug.Log("🚪 Left room successfully");
                joinCreateButton.gameObject.SetActive(true);
                leaveButton.gameObject.SetActive(false);
                startButton.gameObject.SetActive(false);
                HideAllPlayerProfiles();
                UpdatePlayerListTitle();
                break;
                
            case "ROOM_UPDATE":
                Debug.Log("📋 Room updated");
                UpdatePlayerListTitle();  
                break;
                
            case "GAME_STARTED":
                Debug.Log("🎮 Game started!");

                break;
                
            case "ERROR":
                Debug.LogError($"❌ Server error: {message.message}");
                break;
        }
    }

    void OnJoinCreateClicked()
    {
        Debug.Log("Join Status Clicked");
        
        string roomId = roomIdInputField.text;
        string playerName = playerNameInputField.text;

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(roomId))
        {
            Debug.LogWarning("Player name and room ID are required to join or create a room.");
            return;
        }

        nm.JoinRoom(roomId, playerName);
    }

    void OnStartClicked()
    {
        Debug.Log("Start Clicked");
        // Implement start game logic here
    }

    void OnLeaveClicked()
    {
        Debug.Log("Leave Clicked");
        
        nm.LeaveRoom();
    }

    void OnRandomClicked()
    {
        Debug.Log("Random Clicked");
        string roomText = GenerateRoomCode();
        roomIdInputField.text = roomText;
    }
    
    //Header Methods 
    private string GenerateRoomCode()
    {
        const string chars = "ABCEDGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int length = 6;

        string code = "";
        for (int i = 0; i < length; i++)
        {
            code += chars[Random.Range(0, chars.Length)];
        }

        return code;
    }

    private void HideAllPlayerProfiles()
    {
        for (int i = 0; i < playerProfileContainer.childCount; i++)
        {
            GameObject profile = playerProfileContainer.GetChild(i).gameObject;
            playerProfiles.Add(profile);
            profile.SetActive(false);
            
        }
    }

    private void UpdatePlayerListTitle()
    {
        if (playerListTitleText == null)
        {
            Debug.LogWarning("⚠️ playerListTitleText not assigned!");
            return;
        }

        if (nm.CurrentRoom == null)
        {
            Debug.LogWarning("⚠️ Not in a room!");
            playerListTitleText.text = "Join A Room | Players (0/4)";
            return;
        }
        
        if (nm.CurrentRoom.players == null)
        {
            Debug.LogWarning("⚠️ CurrentRoom.players is null!");
            playerListTitleText.text = "Join A Room | Players (0/4)";
            return;
        }
        
        int playerCount = nm.CurrentRoom.players.Count;
        int maxPlayers = nm.CurrentRoom.maxPlayers;
        
        playerListTitleText.text = $"Room {nm.CurrentRoom.roomId} | Players ({playerCount}/{maxPlayers})";

        for (int i = 0; i < playerCount; i++)
        {
            if (i < playerProfiles.Count)
            {
                GameObject profile = playerProfiles[i];
                profile.SetActive(true);
                TMP_Text nameText = profile.GetComponentInChildren<TMP_Text>();
                if (nameText != null)
                {
                    nameText.text = nm.CurrentRoom.players[i].name;
                }
            }
        }
        
        Debug.Log($"✅ Updated player list: {playerCount}/{maxPlayers}");
    }
}
