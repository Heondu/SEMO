using System.Collections;
using UnityEngine;
using Fusion;
using System;

public class PlayerSpriteRenderer : NetworkBehaviour
{
    [Networked] public int NetworkedIndex { get; set; }

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float trailDuration;
    [SerializeField] private AnimationCurve trailSpeed;
    [SerializeField] private Gradient[] trailColors;
    [SerializeField] private AnimationCurve jumpAnimCurve;
    [SerializeField] private AnimationCurve dashAnimCurve;
    [SerializeField] private AnimationCurve collideAnimCurve;
    [SerializeField] private AnimationCurve emotionAnimCurve;
    [SerializeField] private float emotionDuration;

    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer faceRenderer;
    private Sprite defaultEmotion;
    private PlayerController playerController;
    private EmotionController emotionController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        emotionController = FindObjectOfType<EmotionController>();
        defaultEmotion = faceRenderer.sprite;

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
        bodyRenderer.sprite = sprites[NetworkedIndex];
        trailRenderer.colorGradient = trailColors[NetworkedIndex];
    }

    public override void Spawned()
    {
        bodyRenderer.sprite = sprites[NetworkedIndex];
        trailRenderer.colorGradient = trailColors[NetworkedIndex];

        if (Object.HasInputAuthority)
        {
            GameManager.Instance.OnSpawned(NetworkedIndex);
        }

    }

    private void Update()
    {
        if (!Object.HasInputAuthority) return;
        if (GameManager.Instance.GameState != GameState.Playing) return;

        if (Input.GetKeyDown(KeyCode.G))
        {
            emotionController.Show();
        }

        if (emotionController.IsShowing)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RPC_Emotion(0);
                emotionController.Hide();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                RPC_Emotion(1);
                emotionController.Hide();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                RPC_Emotion(2);
                emotionController.Hide();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                RPC_Emotion(3);
                emotionController.Hide();
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_Emotion(int index)
    {
        if (emotionRoutine != null)
            StopCoroutine(emotionRoutine);
        emotionRoutine = StartCoroutine(EmotionRoutine(index));
        playerController.AudioPlayOneShot(emotionController.GetAudioClip(index));
    }

    private Coroutine emotionRoutine;
    private IEnumerator EmotionRoutine(int index)
    {
        faceRenderer.sprite = emotionController.GetEmotion(index);
        StartCoroutine(FaceRoutine(emotionAnimCurve));

        yield return new WaitForSeconds(emotionDuration);

        faceRenderer.sprite = defaultEmotion;
        StartCoroutine(FaceRoutine(emotionAnimCurve));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Object.HasStateAuthority)
            RPC_DoCollideAnim();
    }

    private void ShowTrail()
    {
        if (Object.HasStateAuthority)
            RPC_ShowTrail();
    }

    private void DoJumpAnim()
    {
        if (Object.HasStateAuthority)
            RPC_DoJumpAnim();
    }

    private void DoDashAnim()
    {
        if (Object.HasStateAuthority)
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
            bodyRenderer.transform.localScale = newScale;

            yield return null;
        }
    }

    private IEnumerator FaceRoutine(AnimationCurve animCurve)
    {
        float startTime = 0f;
        float endTime = 1f;

        float curTime = startTime;

        while (curTime < endTime)
        {
            curTime += Time.deltaTime * 10;
            float newValue = animCurve.Evaluate(curTime);
            Vector3 newScale = new Vector3(newValue, newValue, newValue);
            faceRenderer.transform.localScale = newScale;

            yield return null;
        }
    }
}
