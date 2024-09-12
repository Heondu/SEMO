using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class BallController : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hit;
    [SerializeField] private ParticleSystem particle;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private NetworkRigidbody2D networkRigidbody2D;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponentInChildren<CircleCollider2D>();
        networkRigidbody2D = GetComponent<NetworkRigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Object.HasStateAuthority)
        {
            //충돌할 때마다 RPC로 클라이언트에 파티클 플레이 함수 호출
            RPC_PlayParticle();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayParticle()
    {
        particle.Play();
        audioSource.PlayOneShot(hit);
    }

    public void StopMove()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        circleCollider.isTrigger = true;
    }

    public void MoveTo(Vector2 pos)
    {
        rb.MovePosition(pos);
        //networkRigidbody2D.RBPosition = pos;
        //networkRigidbody2D.Rigidbody.MovePosition(pos);
    }
}
