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
        //로비 접속시 호스트 버튼 활성화
        networkManager.onJoinLobby.AddListener(() => {
            hostButton.interactable = true;
            roomCodeInputField.interactable = true;
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
        roomCodeInputField.onSubmit.AddListener((s) => Join());

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
            return;
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
        }
    }
}
