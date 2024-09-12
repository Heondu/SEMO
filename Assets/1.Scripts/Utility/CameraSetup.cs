using UnityEngine;
using Fusion;

public class CameraSetup : NetworkBehaviour
{
    private void Start()
    {
        //로컬 플레이어일 경우에만 카메라 타겟설정
        if (Object.HasInputAuthority)
        {
            CameraController camera = FindObjectOfType<CameraController>();
            camera.Setup(transform);
        }
    }
}