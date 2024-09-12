using UnityEngine;
using UnityEngine.Events;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner networkRunner;

    private List<SessionInfo> sessionList;

    [HideInInspector]
    public UnityEvent onJoinLobby = new UnityEvent();

    private void Awake()
    {
        networkRunner = GetComponent<NetworkRunner>();
        //유저로부터 입력 허용
        networkRunner.ProvideInput = true;
        //INetworkRunnerCallbacks 콜백 할당
        networkRunner.AddCallbacks(this);

        //시작시 룸 목록을 확인하기 위해 로비에 접속
        JoinLobby();
    }

    private async void JoinLobby()
    { 
        //비동기로 로비에 접속
        StartGameResult result = await networkRunner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok)
        {
            onJoinLobby.Invoke();
        }
    }

    public async void StartGame()
    {
        //6자리의 랜덤 룸코드 생성 및 세션이름에 할당
        string roomCode = GenerateRandomRoomCode(6);
        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomCode,
            CustomLobbyName = roomCode,
            PlayerCount = 4,
            //룸 접속시 1번씬으로 전환 설정
            Scene = SceneRef.FromIndex(1),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };
        
        await networkRunner.StartGame(startGameArgs);
    }

    public async void JoinGame(string sessionName)
    {
        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };
        await networkRunner.StartGame(startGameArgs);
    }

    private string GenerateRandomRoomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string randomCode = "";
        do
        {
            for (int i = 0; i < length; i++)
            {
                randomCode += chars[UnityEngine.Random.Range(0, chars.Length)];
            }

            //생성한 룸코드가 이미 존재하는지 확인
        } while (IsExistSession(randomCode));

        return randomCode;
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //함수가 호출될 때마다 세션목록 업데이트
        this.sessionList = sessionList;
    }

    public bool IsExistSession(string name)
    {
        if (sessionList == null)
            return false;

        foreach (SessionInfo sessionInfo in sessionList)
        {
            if (sessionInfo.Name == name)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsPlayerFull(string name)
    {
        //플레이어 수를 확인하기 위해 목록에서 세션 검색
        SessionInfo sessionInfo = GetSessionInfo(name);

        if (sessionInfo == null)
            return true;

        if (sessionInfo.PlayerCount < sessionInfo.MaxPlayers)
            return false;
        else
            return true;
    }

    private SessionInfo GetSessionInfo(string name)
    {
        if (sessionList == null)
            return null;

        foreach (SessionInfo sessionInfo in sessionList)
        {
            if (sessionInfo.Name == name)
            {
                return sessionInfo;
            }
        }
        return null;
    }

    #region INetworkRunnerCallbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}
