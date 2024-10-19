using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private UISelector uiSelector;

    [Header("UI")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private TextMeshProUGUI roomInfoText;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI gameVersionText;

    private void Start()
    {
        gameVersionText.text = "v" + networkManager.GameVersion;

        //�κ� ���ӽ� ȣ��Ʈ ��ư Ȱ��ȭ
        networkManager.onJoinLobby.AddListener(() => {
            hostButton.interactable = true;
            roomCodeInputField.interactable = true;
            uiSelector.Select();
            });

        //�ʱ⿡�� �߸��� ������ �������� ��Ȱ��ȭ
        hostButton.interactable = false;
        hostButton.onClick.AddListener(() => networkManager.StartGame());
        
        joinButton.interactable = false;
        joinButton.onClick.AddListener(Join);

        roomCodeInputField.interactable = false;
        //��ǲ�ʵ� �� ���ο� ���� Ȱ��ȭ �� ��Ȱ��ȭ
        roomCodeInputField.onValueChanged.AddListener((s) => {
            roomInfoText.gameObject.SetActive(false);
            joinButton.interactable = !(s == string.Empty); 
        });
        roomCodeInputField.onSubmit.AddListener((s) => {
            roomCodeInputField.ActivateInputField();
            Join();

        });

        quitButton.onClick.AddListener(() => Application.Quit());
    }

    private void Join()
    {
        //��ǲ�ʵ忡 �Էµ� ���� �빮�ڷ� ��ȯ
        string roomCode = roomCodeInputField.text.ToUpper();

        //�ش� ���ڵ带 ���� ���� �����ϴ��� Ȯ��
        if (!networkManager.IsExistSession(roomCode))
        {
            roomInfoText.text = "�ش� ���ڵ�� �������� �ʽ��ϴ�.";
            roomInfoText.gameObject.SetActive(true);
        }
        //���� ��ġ ���� Ȯ��
        else if (!networkManager.IsMatchedVersion(roomCode))
        {
            roomInfoText.text = "������ ��ġ���� �ʽ��ϴ�.";
            roomInfoText.gameObject.SetActive(true);
        }
        //�÷��̾ �� á���� Ȯ��
        else if (networkManager.IsPlayerFull(roomCode))
        {
            roomInfoText.text = "�� ���� �� á���ϴ�.";
            roomInfoText.gameObject.SetActive(true);
        }
        else
        {
            //�� ��� �ƴ� �� �� ����
            networkManager.JoinGame(roomCode);
            roomCodeInputField.interactable = false;
            joinButton.interactable = false;
        }
    }
}
