using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GoalController : NetworkBehaviour
{
    [SerializeField] private string ballTag;
    [SerializeField] private GameObject completeUI;
    [SerializeField] private GameObject exitButton;

    private bool isActived = false;

    private void Update()
    {
        if (!completeUI.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            completeUI.SetActive(false);
        }
    }

    //OnTriggerEnter와 같은 것들은 호스트에서만 처리됨
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActived)
            return;

        if (Object.HasStateAuthority)
        {
            if (collision.CompareTag(ballTag))
            {
                RPC_Complete();
            }
        }
    }

    //RPC를 쏘기 위해서는 NetworkBehaviour를 상속해야 됨(NetworkObject컴포넌트가 붙어있어야 됨)
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Complete()
    {
        GameManager.Instance.IsClear = true;
        UIManager.Instance.ShowCompleteTime();
        completeUI.SetActive(true);
        isActived = true;

        BallController ball = FindObjectOfType<BallController>();
        ball.StopMove();
        if (Object.HasStateAuthority)
        {
            StartCoroutine(MoveBall(ball));
        }

        Invoke("Complete", 3f);
    }

    public void Complete()
    {
        exitButton.SetActive(true);
    }

    private IEnumerator MoveBall(BallController ball)
    {
        Vector3 startPos = ball.transform.position;
        float percent = 0;
        while (percent < 1)
        {
            ball.MoveTo(Vector3.Lerp(startPos, transform.position, percent));
            percent += Time.deltaTime;
            yield return null;
        }
    }
}