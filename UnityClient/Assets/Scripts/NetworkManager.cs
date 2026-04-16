using Fusion;
using Fusion.Sockets;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // Creating a singleton
    public static NetworkManager Instance { get; private set; }
    public NetworkRunner Runner { get; private set; }
    public bool IsGuide { get; private set; } = true;

    [SerializeField] private GameObject _runnerPrefab;

    private int _targetSceneIndex = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        // Fixing the server to a perticular region
        Fusion.Photon.Realtime.PhotonAppSettings.Global.AppSettings.FixedRegion = "eu";
    }

    public async void ConnectSession(string roomCode)
    {
        CreateRunner();
        await Connect(roomCode);
    }

    public void SetSessionConfig(int sceneIndex, bool isGuide)
    {
        _targetSceneIndex = sceneIndex;
        IsGuide = isGuide;
    }

    public void CreateRunner()
    {
        Runner = Instantiate(_runnerPrefab).GetComponent<NetworkRunner>();
        DontDestroyOnLoad(Runner.gameObject);
        Runner.AddCallbacks(this);
    }

    public async Task Shutdown()
    {
        if (Runner != null)
        {
            await Runner.Shutdown(destroyGameObject: true);
            Runner = null;
        }
    }

    private async Task Connect(string SessionName)
    {
        var args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = SessionName,
            SceneManager = Runner.GetComponent<NetworkSceneManagerDefault>(),
            Scene = SceneRef.FromIndex(_targetSceneIndex)

        };
        await Runner.StartGame(args);
    }

    #region INetworkRunnerCallbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("<<<<<<<< A new player joined to the session >>>>>>>");
        Debug.Log("<<<<<<< IsMasterClient >>>>>>>>" + player.IsMasterClient);
        Debug.Log("<<<<<<< PlayerID >>>>>>>>" + player.PlayerId);
        Debug.Log("<<<<<<< IsRealPlayer >>>>>>>>" + player.IsRealPlayer);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("<<<<<<<< A player left the session >>>>>>>");
        Debug.Log("<<<<<<< IsMasterClient >>>>>>>>" + player.IsMasterClient);
        Debug.Log("<<<<<<< PlayerID >>>>>>>>" + player.PlayerId);
        Debug.Log("<<<<<<< IsRealPlayer >>>>>>>>" + player.IsRealPlayer);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("<<<<<<< Runner Shutdown >>>>>>>>");
    }
    #endregion

    #region INetworkRunnerCallbacks (Unused)
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
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

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
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

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion

}
