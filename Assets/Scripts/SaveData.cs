using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SaveData
{
    private const string benchPath = "/save.bench.data";
    private const string playerPath = "/save.player.data";
    private const string playerShadePath = "/save.playerShade.data";
    public static SaveData Instance;

    // map stuff
    public HashSet<string> sceneNames;

    // bench stuff
    public string benchSceneName;
    public Vector2 benchPos;

    // player stuff
    public int playerHealth;
    public float playerMana;
    public bool playerIsHalfMana;
    public Vector2 playerPosition;
    public string lastScene;

    // enemies stuff
    // playerShade
    public Vector2 playerShadePos;
    public string sceneWithPlayerShade;
    public Quaternion playerShadeRot;

    public void Init()
    {
        InitWriter(benchPath);
        InitWriter(playerPath);
        InitWriter(playerShadePath);

        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    private void InitWriter(string _path)
    {
        if (!File.Exists(Application.persistentDataPath + _path))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + _path));
        }
    }

    public void SaveBench()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + benchPath)))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
        }
    }

    public void LoadBench()
    {
        if (File.Exists(Application.persistentDataPath + benchPath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + benchPath)))
            {
                benchSceneName = reader.ReadString();
                benchPos.x = reader.ReadSingle();
                benchPos.y = reader.ReadSingle();
            }
        }
    }

    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + playerPath)))
        {
            playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);

            playerMana = PlayerController.Instance.Mana;
            writer.Write(playerMana);

            playerIsHalfMana = PlayerController.Instance.isHalfMana;
            writer.Write(playerIsHalfMana);

            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
    }

    public void LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + playerPath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + playerPath)))
            {
                if (reader.PeekChar() != -1)
                {
                    playerHealth = reader.ReadInt32();
                    playerMana = reader.ReadSingle();
                    playerIsHalfMana = reader.ReadBoolean();
                    playerPosition.x = reader.ReadSingle();
                    playerPosition.y = reader.ReadSingle();
                    lastScene = reader.ReadString();

                    SceneManager.LoadScene(lastScene);

                    PlayerController.Instance.Health = playerHealth;
                    PlayerController.Instance.isHalfMana = playerIsHalfMana;
                    PlayerController.Instance.Mana = playerMana;
                    PlayerController.Instance.transform.position = playerPosition;
                }
                else
                {
                    Debug.Log("No player data");
                    // 初始化角色数据
                    PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
                    PlayerController.Instance.isHalfMana = false;
                    PlayerController.Instance.Mana = 0;
                }
            }
        }
        else
        {
            Debug.Log("Player File doesnt exit");
            // 初始化角色数据
            PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            PlayerController.Instance.isHalfMana = false;
            PlayerController.Instance.Mana = 0;
        }
    }

    public void SavePlayerShadeData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + playerShadePath)))
        {
            sceneWithPlayerShade = SceneManager.GetActiveScene().name;
            writer.Write(sceneWithPlayerShade);

            playerShadePos = PlayerShade.Instance.transform.position;
            writer.Write(playerShadePos.x);
            writer.Write(playerShadePos.y);

            playerShadeRot = PlayerShade.Instance.transform.rotation;
            writer.Write(playerShadeRot.x);
            writer.Write(playerShadeRot.y);
            writer.Write(playerShadeRot.z);
            writer.Write(playerShadeRot.w);
        }
    }

    public void LoadPlayerShadeData()
    {
        if (File.Exists(Application.persistentDataPath + playerShadePath))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + playerShadePath)))
            {
                if (reader.PeekChar() != -1)
                {
                    sceneWithPlayerShade = reader.ReadString();
                    playerShadePos.x = reader.ReadSingle();
                    playerShadePos.y = reader.ReadSingle();

                    float _rotationX = reader.ReadSingle();
                    float _rotationY = reader.ReadSingle();
                    float _rotationZ = reader.ReadSingle();
                    float _rotationW = reader.ReadSingle();
                    playerShadeRot = new Quaternion(_rotationX, _rotationY, _rotationZ, _rotationW);
                }
                else
                {
                    Debug.Log("No playerShade data");
                }
            }
        }
        else
        {
            Debug.Log("PlayerShade File doesnt exit");
        }
    }
}