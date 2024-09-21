using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System;
using TMPro;
using Cinemachine;

class PlayerObject
{
    public NetworkObject player;
    public int colorIndex;
}

public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static GameManager Instance { get; private set; }

    private NetworkRunner networkRunner;

    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private NetworkPrefabRef ballPrefab;
    [SerializeField] private Vector2 playerSpawnPoint;
    [SerializeField] private float playerSpawnRangeMinX;
    [SerializeField] private float playerSpawnRangeMaxX;
    [SerializeField] private Vector2 ballSpawnPoint;
    [SerializeField] private GoalController goal;
    [SerializeField] private CinemachineVirtualCamera camera;
    private bool[] validColor = new bool[4];
    public int Index;
    public PlayerRef PlayerRef;

    private Dictionary<PlayerRef, PlayerObject> spawnedPlayers = new Dictionary<PlayerRef, PlayerObject>();
    public bool IsClear = false;

    public GoalController Goal => goal;
    public CinemachineVirtualCamera Cam => camera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        networkRunner = FindObjectOfType<NetworkRunner>();
        networkRunner.AddCallbacks(this);

        //게임시작 시 호스트에서 공 생성
        if (networkRunner.IsServer)
        {
            SpawnBall();
        }

        for (int i = 0; i < 4; i++)
        {
            validColor[i] = true;
        }
    }

    public async void SpawnPlayer(PlayerRef playerRef)
    {
        Vector2 spawnPosition = playerSpawnPoint;
        spawnPosition.x = UnityEngine.Random.Range(playerSpawnRangeMinX, playerSpawnRangeMaxX);
        //비동기로 플레이어 스폰
        NetworkObject playerObject = await networkRunner.SpawnAsync(playerPrefab, spawnPosition, Quaternion.identity, playerRef);

        //플레이어마다 고유 색상 지정
        int colorIndex = GetValidColorIndex();
        playerObject.GetComponent<PlayerSpriteRenderer>().Init(colorIndex);
        validColor[colorIndex] = false;
        spawnedPlayers.Add(playerRef, new PlayerObject { player = playerObject, colorIndex = colorIndex} );
    }

    public async void SpawnBall()
    {
        await networkRunner.SpawnAsync(ballPrefab, ballSpawnPoint, Quaternion.identity);
    }

    public void DestroyPlayer(PlayerRef playerRef)
    {
        if (spawnedPlayers.TryGetValue(playerRef, out PlayerObject playerObject))
        {
            networkRunner.Despawn(playerObject.player);
            validColor[playerObject.colorIndex] = true;
            spawnedPlayers.Remove(playerRef);
        }
    }

    private int GetValidColorIndex()
    {
        for (int i = 0; i < validColor.Length; i++)
        {
            if (validColor[i] == true)
                return i;
        }
        return 0;
    }

    public async void LeaveGame()
    {
        await networkRunner.Shutdown(true);
        SceneManager.LoadScene(0);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            PlayerRef = player;
        }

        if (runner.IsServer)
        {
            SpawnPlayer(player);
        }

        UIManager.Instance.Init(runner);
        UIManager.Instance.UpdatePlayerCountUI();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        DestroyPlayer(player);

        UIManager.Instance.UpdatePlayerCountUI();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        UIManager.Instance.ShowRoomCode(runner.SessionInfo.Name);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene(0);
    }

    public void OnDestroy()
    {
        if (networkRunner != null)
        {
            networkRunner.RemoveCallbacks(this);
        }
    }

    #region INetworkCallbacks
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}