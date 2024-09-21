using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetPointer : MonoBehaviour
{
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite crossSprite;

    [SerializeField] private Transform target;
    private Vector3 targetPosition => target.position;
    private RectTransform pointerRectTransform;
    private Image pointerImage;

    private void Awake()
    {
        pointerRectTransform = GetComponent<RectTransform>();
        pointerImage = GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        float borderSize = 100f;
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

        if (isOffScreen)
        {
            RotatePointerTowardsTargetPosition();

            pointerImage.sprite = arrowSprite;
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

            Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        }
        else
        {
            pointerImage.sprite = crossSprite;
            Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

            pointerRectTransform.localEulerAngles = Vector3.zero;
        }
    }

    private void Update()
    {
        //if (target == null)
        //{
        //    FindTarget();
        //    return;
        //}

        //float borderSize = 100f;
        //Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
        //bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

        //if (isOffScreen)
        //{
        //    RotatePointerTowardsTargetPosition();

        //    pointerImage.sprite = arrowSprite;
        //    Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
        //    if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
        //    if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
        //    if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
        //    if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;

        //    Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
        //    pointerRectTransform.position = pointerWorldPosition;
        //    pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        //}
        //else
        //{
        //    pointerImage.sprite = crossSprite;
        //    Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
        //    pointerRectTransform.position = pointerWorldPosition;
        //    pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

        //    pointerRectTransform.localEulerAngles = Vector3.zero;
        //}
    }

    private void FindTarget()
    {
        BallController target = GameObject.FindObjectOfType<BallController>();

        if (target != null) 
        { 
            Show(target.transform); 
        }
    }

    private void RotatePointerTowardsTargetPosition()
    {
        Vector3 toPosition = targetPosition;
        Vector3 fromPosition = Camera.main.transform.position;

        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) % 360;

        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(Transform target)
    {
        gameObject.SetActive(true);
        this.target = target;
    }
}
