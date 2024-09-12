using UnityEngine;
using Fusion;

public class CameraSetup : NetworkBehaviour
{
    private void Start()
    {
        //���� �÷��̾��� ��쿡�� ī�޶� Ÿ�ټ���
        if (Object.HasInputAuthority)
        {
            CameraController camera = FindObjectOfType<CameraController>();
            camera.Setup(transform);
        }
    }
}