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

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private GhostRenderer[] ghostRenderers;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //ghostRenderers = new GhostRenderer[ghostNum];

        playerController.onDash.AddListener(ShowTrail);
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

    private void ShowTrail()
    {
        RPC_ShowTrail();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ShowTrail()
    {
        StartCoroutine(TrailRoutine());
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
