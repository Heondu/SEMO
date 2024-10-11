using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class PlayerInput : NetworkBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner networkRunner;
    private bool isJumping;
    private bool isJumpDone;
    private bool isDashing;

    private void Awake()
    {
        networkRunner = FindObjectOfType<NetworkRunner>();
        networkRunner.AddCallbacks(this);
    }

    private void Update()
    {
        //OnInput 함수가 호출될 때까지 입력 상태 유지
        isJumping = isJumping || Input.GetButtonDown("Jump");
        isJumpDone = isJumpDone || Input.GetButtonUp("Jump");
        isDashing = isDashing || Input.GetButtonDown("Dash");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (Object != null && Object.HasInputAuthority)
        {
            float direction = Input.GetAxis("Horizontal");

            NetworkInputData inputData;
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                //입력 구조체에 저장, 추후 호스트에서 받아와 입력 수행
                inputData = new NetworkInputData
                {
                    direction = direction,
                    isJumping = this.isJumping,
                    isJumpDone = this.isJumpDone,
                    isDashing = this.isDashing
                };
            }
            else
            {
                inputData = new NetworkInputData
                {
                    direction = 0,
                    isJumping = false,
                    isJumpDone = false,
                    isDashing = false
                };
            }
            isJumping = false;
            isJumpDone = false;
            isDashing = false;

            input.Set(inputData);
        }
    }

    public void OnDestroy()
    {
        if (networkRunner != null)
        {
            networkRunner.RemoveCallbacks(this);
        }
    }

    #region INetworkCallbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
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
