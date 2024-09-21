using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalSkyboxPattern : SkyboxPattern
{
    [SerializeField]
    private float interval;
    [SerializeField]
    private Color[] skyColors;

    private Vector2 curPos;
    private Vector2 prevPos;
    private Vector2 nextPos;

    private int curIndex = 0;
    private int prevIndex;
    private int nextIndex;

    private Color curColor;

    public override void Setup(Camera camera)
    {
        base.Setup(camera);

        curPos = StartPos;
        curIndex = 0;
        curColor = camera.backgroundColor;

        prevPos = new Vector2(curPos.x, curPos.y - interval);
        nextPos = new Vector2(curPos.x, curPos.y + interval);

        prevIndex = (curIndex - 1 + skyColors.Length) % skyColors.Length;
        nextIndex = (curIndex + 1 + skyColors.Length) % skyColors.Length;
    }

    public override void Apply(Camera camera)
    {
        Vector2 camPos = camera.transform.position;

        // ī�޶� ���� Index���� ���� ��
        if (curPos.y <= camPos.y)
        {
            camera.backgroundColor = Color.Lerp(curColor, skyColors[nextIndex], (camPos.y - curPos.y) / interval);

            // ī�޶� ���� Index���ٵ� ���� ��
            if (nextPos.y <= camPos.y)
            {
                curPos = nextPos;
                curIndex = nextIndex;
                curColor = camera.backgroundColor;

                ChangeIndex();
            }
        }
        // ī�޶� ���� Index���� ���� ��
        else
        {
            camera.backgroundColor = Color.Lerp(curColor, skyColors[prevIndex], (curPos.y - camPos.y) / interval);

            // ī�޶� ���� Index���ٵ� ���� ��
            if (prevPos.y > camPos.y)
            {
                curPos = prevPos;
                curIndex = prevIndex;
                curColor = camera.backgroundColor;

                ChangeIndex();
            }
        }
    }

    private void ChangeIndex()
    {
        prevPos = new Vector2(curPos.x, curPos.y - interval);
        nextPos = new Vector2(curPos.x, curPos.y + interval);

        prevIndex = (curIndex - 1 + skyColors.Length) % skyColors.Length;
        nextIndex = (curIndex + 1 + skyColors.Length) % skyColors.Length;
    }
}
