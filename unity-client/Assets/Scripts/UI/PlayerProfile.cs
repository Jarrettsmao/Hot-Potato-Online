using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
// using System.Diagnostics;

public class PlayerProfile : PlayerProfileBase
{
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Image crownIcon;

    public void SetupProfile(Player player)
    {
        SetupBase(player);
        SetHost(player.isHost);
        SetStatus(player.isHost ? "Host" : "Not Ready");
        Debug.Log($"✅ Set up profile for player: {player.name} (ID: {player.id}, Host: {player.isHost}, Connected: {player.connected})");
    }

    public void SetHost(bool isHost)
    {
        if (crownIcon != null)
        {
            crownIcon.gameObject.SetActive(isHost);
        }
    }

    public void SetStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = status;
        }
    }

    public void SetAsLocalPlayer(bool isLocal)
    {
        if (isLocal) statusText.text += " (You)";
    }
}