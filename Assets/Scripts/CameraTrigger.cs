using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera newCamera;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            CameraManager.Instance.SwapCamera(newCamera);
        }
    }
}