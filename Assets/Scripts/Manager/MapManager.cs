using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] maps;
    [SerializeField]
    private string sceneNameTitle = "Cave_";
    private Bench bench;

    private void OnEnable()
    {
        bench = FindObjectOfType<Bench>();
        if (bench != null)
        {
            if (bench.interacted)
            {
                UpdateMap();
            }
        }
    }

    private void UpdateMap()
    {
        var savedScenes = SaveData.Instance.sceneNames;
        for (int i = 0; i < maps.Length; i++)
        {
            if (savedScenes.Contains($"{sceneNameTitle}{i + 1}"))
            {
                maps[i].SetActive(true);
            }
            else
            {
                maps[i].SetActive(false);
            }
        }
    }
}