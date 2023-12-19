using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene; // 存储上一个场景名
    public Vector2 platformingRespawnPoint; // 当前受伤重置点
    public Vector2 respawnPoint; // 复活点

    public GameObject playerShade;

    [SerializeField]
    private Bench bench;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        SaveData.Instance.Init();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.isHalfMana)
            {
                SaveData.Instance.LoadPlayerShadeData();
                if (SaveData.Instance.sceneWithPlayerShade == SceneManager.GetActiveScene().name
                    || SaveData.Instance.sceneWithPlayerShade == "")
                {
                    Instantiate(playerShade, SaveData.Instance.playerShadePos, SaveData.Instance.playerShadeRot);
                }
            }
        }

        SaveScene();

        DontDestroyOnLoad(gameObject);

        bench = FindObjectOfType<Bench>();
    }

    private void Update()
    {
        // TODO:Test
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveData.Instance.SavePlayerData();
        }
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }

    public void RespawnPlayer()
    {
        SaveData.Instance.LoadBench();
        if (SaveData.Instance.benchSceneName != null)
        {
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }

        if (SaveData.Instance.benchPos != null)
        {
            respawnPoint = SaveData.Instance.benchPos;
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