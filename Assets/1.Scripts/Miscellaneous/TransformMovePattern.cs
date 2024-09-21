using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMovePattern : MovePattern
{
    [SerializeField]
    private Vector2 velocity;

    public override void Apply(Transform target)
    {
        Vector2 transition = new Vector2(velocity.x * Speed, velocity.y * Speed);
        target.Translate(transition * Time.deltaTime);

        if (MoveBound)
        {
            MoveBound.SetTargetInBounds(target, velocity, Trail);
        }
    }
}
