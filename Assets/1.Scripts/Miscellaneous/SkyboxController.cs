using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField]
    private SkyboxPattern skyboxPattern;
    private Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();

        skyboxPattern.Setup(camera);
    }

    private void Update()
    {
        skyboxPattern.Apply(camera);
    }
}
