using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Sidji.TestProject.Controller.Player;
using UnityEngine;
using Agones;

[RequireComponent(typeof (NetworkRunner))]
public class MainNetworkRunner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static MainNetworkRunner Instance;
    
    NetworkRunner networkRunner;
    [SerializeField] NetworkPlayerController playerPrefab;
    [SerializeField] SessionSelection sessionSelection;
    

    List<PlayerRef> playerList = new List<PlayerRef>();

    bool isJoined = false;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        networkRunner = GetComponent<NetworkRunner>();
        Instance = this;
    }

    async void Start()
    {
        #if !UNITY_STANDALONE_LINUX || UNITY_EDITOR
        var result = await networkRunner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok) {
            Debug.Log("Join Session ClientServer Success");
        } else {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        #endif
    }
    

    public async void StartSessionFusion(string sessionName)
    {
        networkRunner.ProvideInput = true;

        Debug.Log("Session Initializing : " + sessionName);

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
        #if !UNITY_SERVER || UNITY_EDITOR
            GameMode = GameMode.Client,
        #else
            GameMode = GameMode.Server,
        #endif
            SessionName = sessionName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok) {
            isJoined = true;
            Debug.Log("Session Initialized : " + sessionName);
        } else {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        #if UNITY_SERVER && !UNITY_EDITOR
        if (playerList.Count == 0)
        {
            Debug.Log("Run Fuction Allocate");
            AgonesServerBehaviour.Instance.AgonesSdk.Allocate();
        }
        #endif

        playerList.Add(player);
        if (runner.IsServer)
        {
            Vector2 spawnLocation = Vector2.zero;
            Debug.Log("Player Joined");
            runner.Spawn(playerPrefab, spawnLocation, Quaternion.identity, player.PlayerId);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        playerList.Remove(player);
        playerList.TrimExcess();
        #if UNITY_SERVER && !UNITY_EDITOR
        if (playerList.Count == 0)
        {
            Debug.Log("Run Function Shutdown");
            AgonesServerBehaviour.Instance.AgonesSdk.Shutdown();
        }
        #endif
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        #if !UNITY_SERVER || UNITY_EDITOR
        if (!isJoined)
            sessionSelection.RefreshSession(sessionList);
        #endif
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
