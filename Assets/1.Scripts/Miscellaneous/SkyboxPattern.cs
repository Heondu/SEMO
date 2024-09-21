using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkyboxPattern : MonoBehaviour
{
    public Vector2 StartPos { get; private set; }

    public virtual void Setup(Camera camera)
    {
        StartPos = camera.transform.position;
    }
    public abstract void Apply(Camera camera);
}
