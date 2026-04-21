using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef playerPrefab;

    // Dictionary of spawned user prefabs, to destroy them on disconnection
    private Dictionary<PlayerRef, NetworkObject> _spawnedUsers = new Dictionary<PlayerRef, NetworkObject>();

    private Transform GetXROrigin()
    {
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        return xrOrigin != null ? xrOrigin.transform : null;
    }

    #region INetworkRunnerCallbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer && playerPrefab != null)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            Quaternion spawnRotation = GetSpawnRotation();

            // Move XR Origin to match spawn position so camera starts in the right place
            Transform xrOrigin = GetXROrigin();
            if (xrOrigin != null)
            {
                xrOrigin.position = spawnPosition;
                xrOrigin.rotation = spawnRotation;
            }

            NetworkObject networkPlayerObject = runner.Spawn(
                playerPrefab, spawnPosition, spawnRotation, player);

            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedUsers.Add(player, networkPlayerObject);
        }

        // Start logging FPS and ping once the local player has joined the session
        if (player == runner.LocalPlayer)
            SessionLogger.Instance?.StartLogging(
                SceneManager.GetActiveScene().name);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedUsers.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedUsers.Remove(player);
        }
    }
    #endregion

    private Vector3 GetSpawnPosition()
    {
        int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        bool isTaskScene = sceneIndex == 2 || sceneIndex == 3;

        if (isTaskScene)
        {
            return NetworkManager.Instance.IsGuide
                ? new Vector3(-0.7f, 0.3f, 1.6f)   // Guide / Player A
                : new Vector3(-4.7f, 0.3f, 1.6f);  // Mover / Player B
        }

        return new Vector3(-2.5f, 0.3f, -4.0f); // Warmup default
    }

    private Quaternion GetSpawnRotation()
    {
        int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        bool isTaskScene = sceneIndex == 2 || sceneIndex == 3;

        if (isTaskScene)
        {
            return NetworkManager.Instance.IsGuide
                ? Quaternion.Euler(0f, -90f, 0f)  // Guide / Player A
                : Quaternion.Euler(0f, 90f, 0f);  // Mover / Player B
        }

        return Quaternion.identity; // Warmup default
    }

    #region Unsed INetworkRunnerCallbacks
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

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion
}
