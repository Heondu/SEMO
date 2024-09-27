using UnityEngine;

public class EmotionController : MonoBehaviour
{
    [SerializeField] private EmotionUISetter uiPrefab;
    [SerializeField] private Transform holder;
    [SerializeField] private Sprite[] emotions;
    [SerializeField] private Sprite[] numbers;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private float radius;
    [SerializeField] private Vector3 offest;
    private bool isShowing = false;
    public bool IsShowing => isShowing;

    private void Start()
    {
        Hide();

        for (int i = 0; i < emotions.Length; i++)
        {
            float angle = i * (Mathf.PI * 2.0f) / emotions.Length;
            Vector3 pos = (new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0)) * radius + offest;
            EmotionUISetter clone = Instantiate(uiPrefab, pos, Quaternion.identity);
            clone.transform.SetParent(holder, false);
            clone.Init(emotions[i], numbers[i]);
        }
    }

    public void Show()
    {
        isShowing = !isShowing;
        holder.gameObject.SetActive(isShowing);
    }

    public void Hide()
    {
        isShowing = false;
        holder.gameObject.SetActive(false);
    }

    public Sprite GetEmotion(int index)
    {
        return emotions[index];
    }

    public AudioClip GetAudioClip(int index)
    {
        return audioClips[index];
    }
}
