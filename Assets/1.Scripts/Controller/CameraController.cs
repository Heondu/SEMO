using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;
        
    [SerializeField] private Vector2 minCameraPos;
    [SerializeField] private Vector2 maxCameraPos;

    private void LateUpdate()
    {
        if (target == null)
            return;

        //지정된 좌표 범위내로 카메라 범위 한정
        float posX = Mathf.Clamp(target.position.x, minCameraPos.x, maxCameraPos.x);
        float posY = Mathf.Clamp(target.position.y, minCameraPos.y, maxCameraPos.y);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }

    public void Setup(Transform target)
    {
        this.target = target;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(minCameraPos, maxCameraPos);
    }
}