using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAdjust : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotation;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(rotation);
    }
}
