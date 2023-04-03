using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Sidji.TestProject.Controller.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof (NetworkRunner))]
public class MainNetworkRunner : MonoBehaviour, INetworkRunnerCallbacks
{
    NetworkRunner networkRunner;
    [SerializeField] NetworkPlayerController playerPrefab;



    void Awake()
    {
        networkRunner = GetComponent<NetworkRunner>();
    }

    void Start()
    {
        InitializeNetworkRunner(
            runner: networkRunner,
        #if UNITY_SERVER && !UNITY_EDITOR
            mode: GameMode.Server,
        #else
            mode: GameMode.Client,
        #endif
            address: NetAddress.Any(),
            scene: SceneManager.GetActiveScene().buildIndex,
            initialized: null
        );
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode mode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        runner.ProvideInput = true;

        Debug.Log("Session Initializing");

        return runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            Address = address,
            Scene = scene,
            SessionName = "DEFAULT",
            Initialized = initialized,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
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
        if (NetworkPlayerController.Local != null)
        {
            // input.Set(NetworkPlayerController.Local.GetPlayerMovement());
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector2 spawnLocation = Vector2.zero;
            Debug.Log("Player Joined");
            runner.Spawn(playerPrefab, spawnLocation, Quaternion.identity, player.PlayerId);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // runner.Despawn(runner)
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
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
