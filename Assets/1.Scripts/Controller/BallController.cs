using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class BallController : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hit;
    [SerializeField] private ParticleSystem particle;

    [SerializeField] private AnimationCurve collideAnimCurve;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponentInChildren<CircleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Object.HasStateAuthority)
        {
            //�浹�� ������ RPC�� Ŭ���̾�Ʈ�� ��ƼŬ �÷��� �Լ� ȣ��
            RPC_PlayParticle();
            // �浹 �� �ִϸ��̼� ���
            RPC_DoDashAnim();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayParticle()
    {
        particle.Play();
        audioSource.PlayOneShot(hit);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DoDashAnim()
    {
        StartCoroutine(AnimRoutine(collideAnimCurve));
    }

    private IEnumerator AnimRoutine(AnimationCurve animCurve)
    {
        float startTime = 0f;
        float endTime = 1f;

        float curTime = startTime;

        while (curTime < endTime)
        {
            curTime += Time.deltaTime * 10;
            float newValue = animCurve.Evaluate(curTime);
            Vector3 newScale = new Vector3(newValue, newValue, newValue);
            spriteRenderer.transform.localScale = newScale;

            yield return null;
        }
    }

    public void StopMove()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        circleCollider.isTrigger = true;
    }

    public void MoveTo(Vector2 pos)
    {
        if (Object.HasStateAuthority)
        {
            rb.position = pos;
        }
    }
}
