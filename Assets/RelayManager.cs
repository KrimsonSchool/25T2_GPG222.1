using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    private string joinCode;
    public TMP_InputField joinCodeInput;
    
    public TextMeshProUGUI lobbyCodeText;
    public GameObject lobbyUI;
    public GameObject connectingBuffer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ClientUpdatedJoinCode()
    {
        joinCode = joinCodeInput.text;
    }

    public void InitialiseHostWithRelay()
    {
        connectingBuffer.SetActive(true);
        StartHostWithRelay(4, "udp");
    }
    
    public async Task<string> StartHostWithRelay(int maxConnections, string connectionType)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        
        lobbyCodeText.text = "Lobby Code: " + joinCode;
        lobbyUI.SetActive(false);
        
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public void StartClientWithJoinCode()
    {
        ClientUpdatedJoinCode();
        if (joinCode != "")
        {
            connectingBuffer.SetActive(true);
            StartClientWithRelay(joinCode, "udp");
        }
        else
        {
            Debug.LogError("No join code given");
        }
    }
    
    public async Task<bool> StartClientWithRelay(string joinCode, string connectionType)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
        
        lobbyUI.SetActive(false);
        
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}
