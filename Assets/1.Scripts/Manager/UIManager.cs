using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }
    private NetworkRunner runner;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Image[] voiceImages;
    [SerializeField] private TextMeshProUGUI micText;

    [Header("Menu")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private UISelector menuUISelector;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private UISelector settingsUISelector;
    [SerializeField] private Button closeButton;

    [Header("Complete")]
    [SerializeField] private Button completeExitButton;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI currentTimeText;

    private float[] lastVoiceTimes;

    [Networked] public int NetworkedMin { get; set; }
    [Networked] public float NetworkedSec { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(() => CloseMenu());
        settingsButton.onClick.AddListener(() => {
            CloseMenu();
            OpenSettings();
        });
        exitButton.onClick.AddListener(() => GameManager.Instance.LeaveGame());
        completeExitButton.onClick.AddListener(() => GameManager.Instance.LeaveGame());

        closeButton.onClick.AddListener(() => CloseSettings());

        lastVoiceTimes = new float[voiceImages.Length];
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing && GameManager.Instance.GameState != GameState.UISelecting)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuPanel.activeSelf)
            {
                if (settingsPanel.activeSelf)
                {
                    CloseSettings();
                }
                else
                {
                    OpenMenu();
                }
            }
            else
            {
                CloseMenu();
            }
        }

        if (Object != null && !GameManager.Instance.IsClear)
        {
            UpdateTimeUI();
        }
    }

    public void Init(NetworkRunner runner)
    {
        this.runner = runner;
    }

    public void ShowRoomCode(string roomCode)
    {
        roomCodeText.text = "<b>Room Code</b>\n" + roomCode;
    }

    public void UpdatePlayerCountUI()
    {
        int playerNum = 0;
        foreach (var player in runner.ActivePlayers)
        {
            playerNum++;
        }
        playerCountText.text = $"[{playerNum}/{runner.SessionInfo.MaxPlayers}]";
    }

    private void UpdateTimeUI()
    {
        if (Object.HasStateAuthority)
        {
            NetworkedSec += Time.deltaTime;
            if (NetworkedSec >= 60f)
            {
                NetworkedMin++;
                NetworkedSec = 0;
            }
        }
        timeText.text = string.Format("{0:D2}:{1:D2}", NetworkedMin, (int)NetworkedSec);
    }

    public void ShowCompleteTime()
    {
        bestTimeText.text = "";

        int elapsedTime = NetworkedMin * 60 + (int)NetworkedSec;
        int bestTime = (PlayerPrefs.GetInt("BestTime") <= 0) ? int.MaxValue : PlayerPrefs.GetInt("BestTime");
        if (elapsedTime < bestTime)
        {
            bestTime = elapsedTime;
            PlayerPrefs.SetInt("BestTime", bestTime);
            currentTimeText.text = "신기록 달성!\n";
        }
        else
        {
            currentTimeText.text = "현재 기록\n";
        }
        int min = bestTime / 60;
        int sec = bestTime % 60;
        bestTimeText.text = "최고 기록\n" + string.Format("{0:D2}:{1:D2}", min, sec);

        min = elapsedTime / 60;
        sec = elapsedTime % 60;
        currentTimeText.text += string.Format("{0:D2}:{1:D2}", min, sec);
    }

    public void ActiveVoiceUI()
    {
        //if (GameManager.Instance.PlayerRef == runner.LocalPlayer)
            RPC_ActiveVoiceUI(GameManager.Instance.Index);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ActiveVoiceUI(int index)
    {
        StartCoroutine(VoiceUIRoutine(index));
    }

    private IEnumerator VoiceUIRoutine(int index)
    {
        Transform parent = voiceImages[index].transform.parent;
        voiceImages[index].transform.SetSiblingIndex(GetActiveSiblingIndex(parent));
        voiceImages[index].gameObject.SetActive(true);
        lastVoiceTimes[index] = Time.time;

        yield return new WaitForSeconds(2.1f);

        if (lastVoiceTimes[index] + 2f < Time.time)
        {
            voiceImages[index].gameObject.SetActive(false);
        }
    }

    private int GetActiveSiblingIndex(Transform target)
    {
        int activeIndex = 0;
        for (int i = 0; i < target.childCount; i++)
        {
            Transform child = target.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                activeIndex++;
            }
        }
        return activeIndex;
    }

    public void UpdateMicText(bool isMuted)
    {
        micText.gameObject.SetActive(isMuted);
    }

    private void OpenMenu()
    {
        menuPanel.SetActive(true);
        menuUISelector.Select();
        GameManager.Instance.SetGameState(GameState.UISelecting);
    }

    private void CloseMenu()
    {
        menuPanel.SetActive(false);
        GameManager.Instance.SetGameState(GameState.Playing);
    }

    private void OpenSettings()
    {
        settingsPanel.SetActive(true);
        settingsUISelector.Select();
        GameManager.Instance.SetGameState(GameState.UISelecting);
    }

    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
        OpenMenu();
    }
}