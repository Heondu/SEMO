using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyMovePattern : MovePattern
{
    [SerializeField]
    private Vector2 velocity;
    [SerializeField]
    private Rigidbody2D rigidbody;

    public override void Apply(Transform target)
    {
        if (velocity != Vector2.zero)
        {
            rigidbody.MovePosition((Vector2)target.position + velocity);
        }

        if (MoveBound)
        {
            MoveBound.SetTargetInBounds(target, velocity, Trail);
        }
    }
}
