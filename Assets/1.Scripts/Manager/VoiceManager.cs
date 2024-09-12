using UnityEngine;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;

public class VoiceManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private FusionVoiceClient voiceClient;
    private Recorder recorder;
    private float micVolume;
    private bool isMuted = false;

    [SerializeField] private float volumeThreshold = 0.1f;

    private void Awake()
    {
        voiceClient = FindObjectOfType<FusionVoiceClient>();
        recorder = voiceClient.PrimaryRecorder;
        recorder.VoiceDetectionThreshold = volumeThreshold;
        voiceClient.ConnectAndJoinRoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isMuted = !isMuted;
            recorder.RecordingEnabled = !isMuted;
            UIManager.Instance.UpdateMicText(isMuted);
        }

        if (!isMuted && recorder.LevelMeter != null)
        {
            micVolume = recorder.LevelMeter.CurrentPeakAmp;
            if (micVolume > volumeThreshold)
            {
                OnVoiceDetected();
            }
        }
    }

    private void OnVoiceDetected()
    {
        UIManager.Instance.ActiveVoiceUI();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        voiceClient.ConnectAndJoinRoom();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        voiceClient.Disconnect();
    }

    #region INetworkCallbacks
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
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
