using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera[] allVirtualCameras;
    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    [Header("Y Damping Settings For Player Jump/Fall")]
    [SerializeField]
    private float panAmount = 0.1f;
    [SerializeField]
    private float panTime = 0.2f;

    public bool isLerpingYDamping;
    public bool hasLerpingYDamping;

    private float normalYDamping;

    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            if (allVirtualCameras[i].enabled)
            {
                currentCamera = allVirtualCameras[i];
                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        normalYDamping = framingTransposer.m_YDamping;
    }

    private void Start()
    {
        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            allVirtualCameras[i].Follow = PlayerController.Instance.transform;
        }
    }

    public void SwapCamera(CinemachineVirtualCamera _newCamera)
    {
        currentCamera.enabled = false;
        currentCamera = _newCamera;
        currentCamera.enabled = true;
    }

    public IEnumerator LerpYDamping(bool _isPlayerFalling)
    {
        isLerpingYDamping = true;

        float _startYDamping = framingTransposer.m_YDamping;
        float _endYDamping = 0;
        if (_isPlayerFalling)
        {
            _endYDamping = panAmount;
            hasLerpingYDamping = true;
        }
        else
        {
            _endYDamping = normalYDamping;
        }

        float _timer = 0;
        while (_timer < panTime)
        {
            _timer += Time.deltaTime;
            float _lerpedPanAmount = Mathf.Lerp(_startYDamping, _endYDamping, (_timer / panTime));
            framingTransposer.m_YDamping = _lerpedPanAmount;
            yield return null;
        }
        isLerpingYDamping = false;
    }
}