using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Fusion;
using Fusion.Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance { get; private set; }
    private ChatClient chatClient;
    [SerializeField] private string playerChannel = "GlobalChat";
    [SerializeField] private string systemChannel = "SystemChat";
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI chatDisplay;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private string systemName = "System";
    [SerializeField] private Color systemColor = Color.red;
    [SerializeField] private float showDuration = 5f;
    private int index;
    private bool isInited = false;
    private float lastShowTime = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        button.onClick.AddListener(SendChatMessage);
        CloseInputField();
        CloseScroll();
    }

    public void Init(string sessionName, int index)
    {
        if (isInited)
            return;
        
        this.index = index;
        playerChannel = sessionName + "_GlobalChat";
        systemChannel = sessionName + "_SystemChat";
        ConnectToPhotonChat();
        isInited = true;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing && GameManager.Instance.GameState != GameState.Chatting)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                OpenInputField();
                OpenScroll();
            }
            else if (GameManager.Instance.GameState == GameState.Chatting)
            {
                SendChatMessage();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.GameState == GameState.Chatting)
            {
                CloseInputField();
                CloseScroll();
            }
        }
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    private void ConnectToPhotonChat()
    {
        chatClient = new ChatClient(this);
        string appID = PhotonAppSettings.Global.AppSettings.AppIdChat;
        string appVersion = Application.version;
        chatClient.Connect(appID, appVersion, new Photon.Chat.AuthenticationValues(index.ToString()));
    }

    public void SendChatMessage(string text)
    {
        string message = text;
        if (!string.IsNullOrEmpty(message))
        {
            chatClient.PublishMessage(playerChannel, message);
            inputField.text = "";
        }
        CloseInputField();
    }

    public void SendChatMessage() => SendChatMessage(inputField.text);

    public void SendSystemMessage(string text)
    {
        string message = text;
        if (!string.IsNullOrEmpty(message))
        {
            chatClient.PublishMessage(systemChannel, message);
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        bool isValid = false;
        for (int i = 0; i < senders.Length; i++)
        {
            string color;
            string name;
            if (channelName.Equals(systemChannel))
            {
                color = ColorUtility.ToHtmlStringRGB(systemColor);
                name = systemName;
                chatDisplay.text += $"<color=#{color}>[{DateTime.Now.ToString("HH:mm:ss")}][{name}]{messages[i]}</color>\n";
                isValid = true;
            }
            else if (channelName.Equals(playerChannel))
            {
                int index = senders[i][0] - '0';
                color = ColorUtility.ToHtmlStringRGB(GameManager.Instance.GetColor(index));
                name = GameManager.Instance.GetName(index);
                chatDisplay.text += $"[{DateTime.Now.ToString("HH:mm:ss")}]<color=#{color}>[{name}]</color>{messages[i]}\n";
                isValid = true;
            }
        }

        if (isValid)
        {
            OpenScroll();
            lastShowTime = Time.time;
            Invoke(nameof(TryCloseScroll), showDuration);
        }
    }

    public void OnConnected()
    {
        if (chatClient != null && chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            chatClient.Subscribe(playerChannel);
            chatClient.Subscribe(systemChannel);
        }
    }

    private void OpenScroll()
    {
        scroll.gameObject.SetActive(true);
    }

    private void CloseScroll()
    {
        scroll.gameObject?.SetActive(false);
    }

    private void TryCloseScroll()
    {
        if (Time.time - lastShowTime <= showDuration - 0.1f) return;
        if (GameManager.Instance.GameState == GameState.Chatting) return;

        CloseScroll();
    }

    private void OpenInputField()
    {
        inputField.gameObject.SetActive(true);
        button.gameObject.SetActive(true);
        inputField.ActivateInputField();

        GameManager.Instance.SetGameState(GameState.Chatting);
    }

    private void CloseInputField()
    {
        inputField.gameObject.SetActive(false);
        button.gameObject.SetActive(false);

        GameManager.Instance.SetGameState(GameState.Playing);
    }

    #region IChatClientListener
    public void DebugReturn(DebugLevel level, string message) { }
    public void OnChatStateChange(ChatState state) { }
    public void OnDisconnected() { }
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnSubscribed(string[] channels, bool[] results) { }
    public void OnUnsubscribed(string[] channels) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }
    #endregion
}