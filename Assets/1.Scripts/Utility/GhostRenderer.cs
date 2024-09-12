using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRenderer : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private Color[] colors;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer targetRenderer;
    private Transform targetTransform;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Transform transform, SpriteRenderer renderer)
    {
        targetTransform = transform;
        targetRenderer = renderer;
        gameObject.SetActive(false);
    }

    public void Active(int index)
    {
        spriteRenderer.sprite = targetRenderer.sprite;
        spriteRenderer.sortingOrder = index;
        transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
        transform.parent = null;
        gameObject.SetActive(true);

        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        float elapsedTime = 0;
        float colorTime = 0;
        int colorIndex = 1;
        Color startColor = colors[0];
        while (elapsedTime < lifeTime)
        {
            spriteRenderer.color = Color.Lerp(startColor, colors[colorIndex], elapsedTime / lifeTime); 
            if (colorTime > lifeTime / (colors.Length-1))
            {
                colorTime = 0;
                startColor = colors[colorIndex];
                colorIndex++;
                colorIndex = colorIndex == colors.Length ? 0 : colorIndex;
            }
            elapsedTime += Time.deltaTime;
            colorTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = Color.white;
        Deactive();
    }

    private void Deactive()
    {
        transform.parent = targetTransform;
        gameObject.SetActive(false);
    }
}
