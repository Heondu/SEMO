using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISelector : MonoBehaviour
{
    [SerializeField] private Selectable primarySelectable;

    public void Select()
    {
        primarySelectable.Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                Select();
            }
        }
    }
}
