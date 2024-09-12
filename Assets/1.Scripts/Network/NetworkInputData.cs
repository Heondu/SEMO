using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float direction;
    public bool isJumping;
    public bool isDashing;
}
