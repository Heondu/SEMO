using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private TextMeshProUGUI roomInfoText;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        //�κ� ���ӽ� ȣ��Ʈ ��ư Ȱ��ȭ
        networkManager.onJoinLobby.AddListener(() => {
            hostButton.interactable = true;
            roomCodeInputField.interactable = true;
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
        roomCodeInputField.onSubmit.AddListener((s) => Join());

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
            return;
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
        }
    }
}
