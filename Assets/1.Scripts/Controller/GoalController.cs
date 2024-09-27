using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GoalController : NetworkBehaviour
{
    [SerializeField] private string ballTag;
    [SerializeField] private GameObject completeUI;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private AudioClip clearAudioClip;
    [SerializeField] private ParticleSystem particle;

    private bool isComplete = false;
    private bool isMoved = false;

    private void Update()
    {
        if (!completeUI.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            completeUI.SetActive(false);
        }
    }

    //OnTriggerEnter�� ���� �͵��� ȣ��Ʈ������ ó����
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isComplete)
            return;

        if (collision.CompareTag(ballTag))
        {
            RPC_Complete();
        }
    }

    //RPC�� ��� ���ؼ��� NetworkBehaviour�� ����ؾ� ��(NetworkObject������Ʈ�� �پ��־�� ��)
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Complete()
    {
        particle.Play();
        AudioManager.instance.SwapTrack(clearAudioClip);
        GameManager.Instance.IsClear = true;
        UIManager.Instance.ShowCompleteTime();
        completeUI.SetActive(true);
        isComplete = true;

        BallController ball = FindObjectOfType<BallController>();
        ball.StopMove();
        StartCoroutine(MoveBall(ball));

        Invoke(nameof(ShowExitButton), 3f);
    }

    public void ShowExitButton()
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