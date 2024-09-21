using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovePattern : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private MoveBounds moveBound;
    public float Speed => speed;
    public MoveBounds MoveBound => moveBound;
    public Vector2 Center { get; private set; }
    public TrailRenderer Trail { get; private set; }

    public virtual void Setup(Transform target, TrailRenderer trail)
    {
        Center = target.position;
        Trail = trail;
    }

    public abstract void Apply(Transform target);
}
