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

        // 카메라가 현재 Index보다 높을 때
        if (curPos.y <= camPos.y)
        {
            camera.backgroundColor = Color.Lerp(curColor, skyColors[nextIndex], (camPos.y - curPos.y) / interval);

            // 카메라가 다음 Index보다도 높을 때
            if (nextPos.y <= camPos.y)
            {
                curPos = nextPos;
                curIndex = nextIndex;
                curColor = camera.backgroundColor;

                ChangeIndex();
            }
        }
        // 카메라가 현재 Index보다 낮을 때
        else
        {
            camera.backgroundColor = Color.Lerp(curColor, skyColors[prevIndex], (curPos.y - camPos.y) / interval);

            // 카메라가 이전 Index보다도 낮을 때
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
