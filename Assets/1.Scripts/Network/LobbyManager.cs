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

        //로비 접속시 호스트 버튼 활성화
        networkManager.onJoinLobby.AddListener(() => {
            hostButton.interactable = true;
            roomCodeInputField.interactable = true;
            uiSelector.Select();
            });

        //초기에는 잘못된 연결을 막기위해 비활성화
        hostButton.interactable = false;
        hostButton.onClick.AddListener(() => networkManager.StartGame());
        
        joinButton.interactable = false;
        joinButton.onClick.AddListener(Join);

        roomCodeInputField.interactable = false;
        //인풋필드 값 여부에 따라 활성화 및 비활성화
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
        //인풋필드에 입력된 값을 대문자로 변환
        string roomCode = roomCodeInputField.text.ToUpper();

        //해당 룸코드를 가진 방이 존재하는지 확인
        if (!networkManager.IsExistSession(roomCode))
        {
            roomInfoText.text = "해당 룸코드는 존재하지 않습니다.";
            roomInfoText.gameObject.SetActive(true);
        }
        //버전 일치 여부 확인
        else if (!networkManager.IsMatchedVersion(roomCode))
        {
            roomInfoText.text = "버전이 일치하지 않습니다.";
            roomInfoText.gameObject.SetActive(true);
        }
        //플레이어가 다 찼는지 확인
        else if (networkManager.IsPlayerFull(roomCode))
        {
            roomInfoText.text = "이 방은 꽉 찼습니다.";
            roomInfoText.gameObject.SetActive(true);
        }
        else
        {
            //위 모두 아닐 시 방 접속
            networkManager.JoinGame(roomCode);
            roomCodeInputField.interactable = false;
            joinButton.interactable = false;
        }
    }
}
