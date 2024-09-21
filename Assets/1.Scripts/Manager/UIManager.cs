using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }
    private NetworkRunner runner;

    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button completeExitButton;
    [SerializeField] private TextMeshProUGUI completeTimeText;
    [SerializeField] private Image[] voiceImages;
    [SerializeField] private TextMeshProUGUI micText;
    private float[] lastVoiceTimes;

    [Networked] public int NetworkedMin { get; set; }
    [Networked] public float NetworkedSec { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(() => menuPanel.SetActive(false));
        settingsButton.onClick.AddListener(() => {
            menuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        });
        exitButton.onClick.AddListener(() => GameManager.Instance.LeaveGame());
        completeExitButton.onClick.AddListener(() => GameManager.Instance.LeaveGame());

        lastVoiceTimes = new float[voiceImages.Length];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
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
        completeTimeText.text = "";

        int elapsedTime = NetworkedMin * 60 + (int)NetworkedSec;
        int bestTime = (PlayerPrefs.GetInt("BestTime") <= 0) ? int.MaxValue : PlayerPrefs.GetInt("BestTime");
        if (elapsedTime < bestTime)
        {
            bestTime = elapsedTime;
            PlayerPrefs.SetInt("BestTime", bestTime);
            completeTimeText.text += "BEST\n";
        }
        int min = bestTime / 60;
        int sec = bestTime % 60;

        completeTimeText.text += string.Format("{0:D2}:{1:D2}", min, sec);
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
}