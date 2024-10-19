using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D cursorNormal;
    [SerializeField] private Texture2D cursorClicked;

    private void Awake()
    {
        if (FindObjectOfType<CursorController>() != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        ChangeCursor(cursorNormal);
        Cursor.lockState = CursorLockMode.None;
    }

    private void ChangeCursor(Texture2D cursorType)
    {
        Vector2 hotspot = Vector2.zero;//ew Vector2(cursorType.width / 2, cursorType.height / 2);
        Cursor.SetCursor(cursorType, hotspot, CursorMode.Auto);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeCursor(cursorClicked);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ChangeCursor(cursorNormal);
        }
    }
}
