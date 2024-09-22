using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Fusion.Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance { get; private set; }

    private ChatClient chatClient;
    [SerializeField] private string channelName = "GlobalChat";
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI chatDisplay;
    [SerializeField] private ScrollRect scroll;
    private int index;
    private bool isInited = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        button.onClick.AddListener(SendChatMessage);
        ShowUI(false);
    }

    public void Init(int index)
    {
        if (isInited)
            return;

        this.index = index;
        ConnectToPhotonChat();
        isInited = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                GameManager.Instance.GameState = GameState.Chatting;
                ShowUI(true);
            }
            else if (GameManager.Instance.GameState == GameState.Chatting)
            {
                SendChatMessage();
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
        chatClient.Connect(appID, "1.0", new Photon.Chat.AuthenticationValues(index.ToString()));
    }

    public void SendChatMessage(string text)
    {
        string message = text;
        if (!string.IsNullOrEmpty(message))
        {
            chatClient.PublishMessage(channelName, message);
            inputField.text = "";
            inputField.gameObject.SetActive(false);
            button.gameObject.SetActive(false);
        }
        else
        {
            ShowUI(false);
        }

        GameManager.Instance.GameState = GameState.Playing;
    }

    public void SendChatMessage() => SendChatMessage(inputField.text);

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            string color;
            string name;
            if (senders[i].Equals("System"))
            {
                color = ColorUtility.ToHtmlStringRGB(Color.black);
                name = senders[i];
            }
            else
            {
                int index = senders[i][0] - '0';
                color = ColorUtility.ToHtmlStringRGB(GameManager.Instance.GetColor(index));
                name = GameManager.Instance.GetName(index);
            }

            chatDisplay.text += $"[{DateTime.Now.ToString("HH:mm:ss")}]<color=#{color}>[{name}]</color>:{messages[i]}\n";
        }

        scroll.gameObject.SetActive(true);
        Invoke("HideScroll", 5f);
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { channelName });
    }

    private void ShowUI(bool value)
    {
        scroll.gameObject.SetActive(value);
        inputField.gameObject.SetActive(value);
        button.gameObject.SetActive(value);

        if (value)
        {
            inputField.ActivateInputField();
        }
    }

    private void HideScroll()
    {
        if (GameManager.Instance.GameState != GameState.Chatting)
        {
            scroll.gameObject.SetActive(false);
        }
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
