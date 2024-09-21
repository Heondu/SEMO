using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkyboxPattern : SkyboxPattern
{
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private float changeTime;
    [SerializeField]
    private Color[] skyColors;

    private float curTime = 0f;
    private int curIndex = 0;
    private Color curColor;
    private enum SkyboxState
    {
        WAIT, CHANGE
    }

    private SkyboxState curSkyboxState = SkyboxState.WAIT;

    public override void Setup(Camera camera)
    {
        base.Setup(camera);

        curColor = camera.backgroundColor;
    }

    public override void Apply(Camera camera)
    {
        curTime += Time.deltaTime;

        switch (curSkyboxState)
        {
            case SkyboxState.WAIT:

                if (curTime >= waitTime)
                {
                    ChangeSkyboxState(SkyboxState.CHANGE);
                }

                break;

            case SkyboxState.CHANGE:

                camera.backgroundColor = Color.Lerp(curColor, skyColors[curIndex], curTime / changeTime);

                if (curTime >= changeTime)
                {
                    curColor = skyColors[curIndex];
                    curIndex = Random.Range(0, skyColors.Length);
                    ChangeSkyboxState(SkyboxState.WAIT);
                }

                break;
        }
    }

    private void ChangeSkyboxState(SkyboxState newState)
    {
        curTime = 0f;
        curSkyboxState = newState;
    }
}
