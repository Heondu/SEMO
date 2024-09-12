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
        //�����κ��� �Է� ���
        networkRunner.ProvideInput = true;
        //INetworkRunnerCallbacks �ݹ� �Ҵ�
        networkRunner.AddCallbacks(this);

        //���۽� �� ����� Ȯ���ϱ� ���� �κ� ����
        JoinLobby();
    }

    private async void JoinLobby()
    { 
        //�񵿱�� �κ� ����
        StartGameResult result = await networkRunner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok)
        {
            onJoinLobby.Invoke();
        }
    }

    public async void StartGame()
    {
        //6�ڸ��� ���� ���ڵ� ���� �� �����̸��� �Ҵ�
        string roomCode = GenerateRandomRoomCode(6);
        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomCode,
            CustomLobbyName = roomCode,
            PlayerCount = 4,
            //�� ���ӽ� 1�������� ��ȯ ����
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

            //������ ���ڵ尡 �̹� �����ϴ��� Ȯ��
        } while (IsExistSession(randomCode));

        return randomCode;
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //�Լ��� ȣ��� ������ ���Ǹ�� ������Ʈ
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
        //�÷��̾� ���� Ȯ���ϱ� ���� ��Ͽ��� ���� �˻�
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
