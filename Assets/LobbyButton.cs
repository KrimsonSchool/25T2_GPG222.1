using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    public Lobby  lobby;
    
    public TextMeshProUGUI buttonInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText()
    {
        buttonInfo.text = "<u><b><color=#0000FF>"+ lobby.Name + "</color></b></u>\n" + lobby.AvailableSlots + "/" + lobby.MaxPlayers + " free slots left.";
    }

    public void JoinLobby()
    {
        FindFirstObjectByType<LobbySystem>().JoinLobbyId(lobby.Id);
    }
}
