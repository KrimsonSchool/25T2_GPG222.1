using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbySystem : MonoBehaviour
{

    //need:
    //loading screen for create/join
    //show error if couldn't create/join
    public TextMeshProUGUI lobbiesText;
    public Toggle privateToggle;
    public TMP_InputField lobbyNameInput;
    public TMP_InputField lobbyCodeInput;
    
    public GameObject loadingScreen;

    private float timer;

    private Lobby currentLobby;

    public TextMeshProUGUI currentLobbyText;

    public GameObject joinButtonHolder;
    public GameObject lobbyJoinButton;
    
    public RelayManager relayManager;

    private string relayJoinCode;

    public GameObject lobbyButtons;
    public GameObject startGameButton;
    //public Button[] joinButtons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        QueryForLobbies();
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if (timer >= 1)
        {
            QueryForLobbies();
            UpdateCurrentLobby();
            timer = 0;
        }
    }

    public async void CreateLobby()
    {
        if (lobbyNameInput.text != "")
        {
            loadingScreen.SetActive(true);
            if (currentLobby != null)
            {
                LeaveLobby(currentLobby.Id);
            }
            Lobby createdLobby = await CreateLobbyTask(lobbyNameInput.text);
            currentLobby = createdLobby;
            print(relayJoinCode);
            
            lobbyButtons.SetActive(false);
            startGameButton.SetActive(true);
            
            loadingScreen.SetActive(false);
            QueryForLobbies();
        }
    }

    public async Task<Lobby> CreateLobbyTask(string lobbyNme)
    {
        string lobbyName = lobbyNme; //shoudl be username+'s lobby
        int maxPlayers = 4;
        CreateLobbyOptions options = new CreateLobbyOptions();

        options.IsPrivate = privateToggle.isOn;
        options.IsLocked = false;
        
        string lobbyCode = await relayManager.StartHostWithRelay(4, "udp");
        relayJoinCode = lobbyCode;
        options.Data = new Dictionary<string, DataObject>()
        {
            {
                "LobbyCode", new DataObject(
                    visibility: DataObject.VisibilityOptions.Public,
                    value: lobbyCode)
            },
        };
        
        var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));
        print("created lobby, private: " + privateToggle.isOn);
        return lobby;
    }

    /*
    public async void JoinLobbyCode()
    {
        if (lobbyCodeInput.text != "")
        {
            try
            {
                if (currentLobby != null)
                {
                    LeaveLobby(currentLobby.Id);
                }
                loadingScreen.SetActive(true);
                Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text);
                currentLobby = joinedLobby;
                loadingScreen.SetActive(false);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                loadingScreen.SetActive(false);
            }
        }
    }*/

    public async void JoinLobbyId(string lobbyId)
    {
        try
        {
            if (currentLobby != null)
            {
                LeaveLobby(currentLobby.Id);
            }
            loadingScreen.SetActive(true);
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            currentLobby = joinedLobby;

            relayJoinCode = currentLobby.Data["LobbyCode"].Value;

            print(relayJoinCode);
            relayManager.joinCode = relayJoinCode;
            relayManager.StartClientWithJoinCode();
            
            
            lobbyButtons.SetActive(false);
            
            loadingScreen.SetActive(false);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            loadingScreen.SetActive(false);
        }
    }

    public async void DeleteLobby(string lobbyId)
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync("lobbyId");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QueryForLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            //lobbiesText.text = "";
            DestroyAllChildren();
            foreach (var result in lobbies.Results)
            {
                GameObject ljb = Instantiate(lobbyJoinButton, joinButtonHolder.transform);
                ljb.GetComponent<LobbyButton>().lobby = result;
                ljb.GetComponent<LobbyButton>().UpdateText();
                //lobbiesText.text += "<u><b><color=#0000FF>"+ result.Name+"</color></b></u>\n"+result.AvailableSlots+"/"+result.MaxPlayers+" slots free\n";
            }
            //...
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
    
    ConcurrentQueue<string> createdLobbyIds = new ConcurrentQueue<string>();
    void OnApplicationQuit()
    {
        while (createdLobbyIds.TryDequeue(out var lobbyId))
        {
            LobbyService.Instance.DeleteLobbyAsync(lobbyId);
        }
    }

    public void UpdateCurrentLobby()
    {
        if (currentLobby != null)
        {
            currentLobbyText.text = "Current Lobby: <u><b><color=#0000FF>"  + currentLobby.Name +"</color></b></u>. Code: " + currentLobby.LobbyCode;
        }
    }
    
    public void DestroyAllChildren()
    {
        for (int i = joinButtonHolder.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = joinButtonHolder.transform.GetChild(i);
            Destroy(child.gameObject); 
        }
    }

    public async void LeaveLobby(string lobbyId)
    {
        try
        {
            //Ensure you sign-in before calling Authentication Instance
            //See IAuthenticationService interface
            string playerId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        
        FindFirstObjectByType<Eye>().started.Value = true;
    }

}
