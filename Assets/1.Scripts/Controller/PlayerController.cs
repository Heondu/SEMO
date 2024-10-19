using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using Fusion.Addons.Physics;

public class PlayerController : NetworkBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float slamForce = 30f;
    [SerializeField] private float jumpDoneDuration = 0.1f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private float groundSize;
    private bool isGrounded;
    private bool isDashing;
    private bool isSlaming;
    private float lastDashTime;
    private float lastDirection = 1;
    [HideInInspector] public UnityEvent onJump;
    [HideInInspector] public UnityEvent onDash;
    [HideInInspector] public UnityEvent onSlam;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip slamSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            return;

        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        runner.SetIsSimulated(GetComponent<NetworkObject>(), false);

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.inertia = 0;
    }

    //네트워크 프레임마다 물리 연산을 처리하여 일관된 동기화 보장
    public override void FixedUpdateNetwork()
    {
        //클라이언트 입력을 서버에서 처리하여 모든 클라이언트에 일관된 상태 반영
        if (Object.HasStateAuthority && GetInput(out NetworkInputData inputData))
        {
            Move(inputData.direction);
            Jump(inputData.isJumping);
            JumpDone(inputData.isJumpDone);
            Dash(inputData.isDashing);
            //Slam(inputData.isSlaming);
        }
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position + groundOffset, groundSize, groundLayer);
    }

    private void Move(float direction)
    {
        if (isDashing || isSlaming)
            return;

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        if (direction != 0)
        {
            lastDirection = Mathf.Sign(direction);
        }
    }

    private void Jump(bool isJumping)
    {
        if (isGrounded && isJumping)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            //audioSource.PlayOneShot(jumpSound);
            AudioPlayOneShot(jumpSound);

            onJump?.Invoke();
        }
    }

    private void JumpDone(bool isJumpDone)
    {
        if (isJumpDone && rb.velocity.y > 0)
        {
            //rb.velocity = rb.velocity * 0.5f;
            jumpDoneRoutine = StartCoroutine(JumpDoneRoutine());
        }
    }

    private void Dash(bool isDashing)
    {
        if (isDashing && !this.isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            this.isDashing = true;
            lastDashTime = Time.time;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(lastDirection * dashForce, 0), ForceMode2D.Impulse);
            //audioSource.PlayOneShot(dashSound);
            AudioPlayOneShot(dashSound);
            StartCoroutine(DashRoutine());

            onDash?.Invoke();
        }
    }

    private void Slam(bool isSlaming)
    {
        if (isGrounded)
        {
            this.isSlaming = false;
        }

        if (isSlaming && !isGrounded && !this.isSlaming)
        {
            this.isSlaming = true;

            if (jumpDoneRoutine != null)
                StopCoroutine(jumpDoneRoutine);

            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.down * slamForce, ForceMode2D.Impulse);
            AudioPlayOneShot(slamSound);

            onSlam?.Invoke();
        }
    }

    public void AudioPlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    private Coroutine jumpDoneRoutine;
    private IEnumerator JumpDoneRoutine()
    {
        float startVelocityY = rb.velocity.y;
        float endVelocityY = rb.velocity.y * 0.5f;
        float curVelocityY = startVelocityY;

        float elapsedTime = 0;

        while (elapsedTime < jumpDoneDuration)
        {
            elapsedTime += Time.deltaTime;

            float percent = elapsedTime / jumpDoneDuration;

            curVelocityY = Mathf.Lerp(startVelocityY, endVelocityY, percent);
            rb.velocity = new Vector2(rb.velocity.x, curVelocityY);

            yield return null;
        }
    }

    private IEnumerator DashRoutine()
    {
        float elapsedTime = 0;
        
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        if (!isSlaming)
            rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundOffset, groundSize);
    }
}