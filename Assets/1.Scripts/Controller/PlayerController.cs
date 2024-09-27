using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float jumpDoneDuration = 0.1f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private float groundSize;
    private bool isGrounded;
    private bool isDashing;
    private float lastDashTime;
    private float lastDirection = 1;
    [HideInInspector] public UnityEvent onDash;
    [HideInInspector] public UnityEvent onJump;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GameManager.Instance.GameState != GameState.Playing)
        {
            Move(0);
            Jump(false);
            JumpDone(false);
            Dash(false);
        }
        else if (GetInput(out NetworkInputData inputData))
        {
            Move(inputData.direction);
            Jump(inputData.isJumping);
            JumpDone(inputData.isJumpDone);
            Dash(inputData.isDashing);
        }
    }

    private void Move(float direction)
    {
        if (isDashing)
            return;

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        if (direction != 0)
        {
            lastDirection = Mathf.Sign(direction);
        }
    }

    private void Jump(bool isJumping)
    {
        isGrounded = Physics2D.OverlapCircle(transform.position + groundOffset, groundSize, groundLayer);
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
            StartCoroutine(JumpDoneRoutine());
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

            onDash.Invoke();
        }
    }

    public void AudioPlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

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
        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundOffset, groundSize);
    }
}