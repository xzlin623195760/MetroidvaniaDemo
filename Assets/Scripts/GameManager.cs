using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene; // �洢��һ��������
    public Vector2 platformingRespawnPoint; // ��ǰ�������õ�
    public Vector2 respawnPoint; // �����

    public GameObject shade;

    [SerializeField]
    private Bench bench;

    public static GameManager Instance { get; private set; }

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
        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();
    }

    public void RespawnPlayer()
    {
        if (bench != null)
        {
            if (bench.interacted)
            {
                respawnPoint = bench.transform.position;
            }
            else
            {
                respawnPoint = platformingRespawnPoint;
            }
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }

        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }
}