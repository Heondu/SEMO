using System.Collections;
using UnityEngine;
using Fusion;

public class PlayerSpriteRenderer : NetworkBehaviour
{
    [Networked] public int NetworkedIndex { get; set; }

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float trailDuration;
    [SerializeField] private AnimationCurve trailSpeed;
    [SerializeField] private Gradient[] trailColors;
    //[SerializeField] private GhostRenderer ghostPrefab;
    //[SerializeField] private int ghostNum = 10;
    //[SerializeField] private float ghostDelay = 0.05f;
    //private int ghostIndex = 0;
    //private float lastGhostTime = 0;
    [SerializeField] private AnimationCurve jumpAnimCurve;
    [SerializeField] private AnimationCurve dashAnimCurve;
    [SerializeField] private AnimationCurve collideAnimCurve;

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private GhostRenderer[] ghostRenderers;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //ghostRenderers = new GhostRenderer[ghostNum];

        playerController.onDash.AddListener(ShowTrail);
        playerController.onDash.AddListener(DoDashAnim);
        playerController.onJump.AddListener(DoJumpAnim);
    }

    public void Init(int index)
    {
        if (Object.HasStateAuthority)
        {
            NetworkedIndex = index;
        }
        spriteRenderer.sprite = sprites[NetworkedIndex];
        trailRenderer.colorGradient = trailColors[NetworkedIndex];

        if (Object.HasInputAuthority)
        {
            GameManager.Instance.Index = index;
        }

        //for (int i = 0; i < ghostNum; i++)
        //{
        //    GhostRenderer clone = Instantiate(ghostPrefab, transform);
        //    clone.Init(transform, spriteRenderer);
        //    ghostRenderers[i] = clone;
        //}
    }

    public override void Spawned()
    {
        spriteRenderer.sprite = sprites[NetworkedIndex];
        trailRenderer.colorGradient = trailColors[NetworkedIndex];

        if (Object.HasInputAuthority)
        {
            GameManager.Instance.Index = NetworkedIndex;
        }
    }

    private void Update()
    {
        //if (playerController.IsDashing)
        //{
        //
        //    Ghost();
        //}
        //else
        //{
        //    ghostIndex = 0;
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RPC_DoCollideAnim();
    }

    private void ShowTrail()
    {
        RPC_ShowTrail();
    }

    private void DoJumpAnim()
    {
        RPC_DoJumpAnim();
    }

    private void DoDashAnim()
    {
        RPC_DoDashAnim();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ShowTrail()
    {
        StartCoroutine(TrailRoutine());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DoJumpAnim()
    {
        StartCoroutine(AnimRoutine(jumpAnimCurve));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DoDashAnim()
    {
        StartCoroutine(AnimRoutine(dashAnimCurve));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DoCollideAnim()
    {
        StartCoroutine(AnimRoutine(collideAnimCurve));
    }

    private IEnumerator TrailRoutine()
    {
        trailRenderer.gameObject.SetActive(true);

        float percent = 0;
        while (percent < 1)
        {
            trailRenderer.time = trailSpeed.Evaluate(percent);
            percent += Time.deltaTime / trailDuration;
            yield return null;
        }

        trailRenderer.gameObject.SetActive(false);
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

    //private void Ghost()
    //{
    //    if (Time.time > lastGhostTime + ghostDelay)
    //    {
    //        GhostRenderer ghost = ghostRenderers[ghostIndex];
    //        ghostIndex = ghostIndex + 1 == ghostNum ? 0 : ghostIndex + 1;
    //        ghost.Active(ghostIndex - ghostNum);
    //        lastGhostTime = Time.time;
    //    }
    //}
}
