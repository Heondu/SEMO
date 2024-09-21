using System.Collections;
using UnityEngine;
using Fusion;
using Cinemachine;

public class CameraSetup : NetworkBehaviour
{
    private CinemachineVirtualCamera camera;
    private GoalController goal;

    private void Start()
    {
        //로컬 플레이어일 경우에만 카메라 타겟설정
        if (Object.HasInputAuthority)
        {
            camera = GameManager.Instance.Cam;
            goal = GameManager.Instance.Goal;

            StartCoroutine(Setup(transform, 0f));
            StartCoroutine(Setup(goal.transform, 1f));
            StartCoroutine(Setup(transform, 4f));

            //CameraController camera = FindObjectOfType<CameraController>();
            //camera.Setup(transform);
        }
    }

    private IEnumerator Setup(Transform target, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        camera.m_Follow = target;
    }
}