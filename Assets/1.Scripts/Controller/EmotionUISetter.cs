using UnityEngine;
using UnityEngine.UI;

public class EmotionUISetter : MonoBehaviour
{
    [SerializeField] private Image face;
    [SerializeField] private Image index;

    public void Init(Sprite face, Sprite index)
    {
        this.face.sprite = face;
        this.index.sprite = index;
    }
}
