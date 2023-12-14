using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    private PlayerController player;
    private GameObject[] heartContainers;
    private Image[] heartFills;

    public Transform heartsParent;
    public GameObject heartContainerPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        player = PlayerController.Instance;
        heartContainers = new GameObject[player.maxHealth];
        heartFills = new Image[player.maxHealth];

        player.OnHealthChangedCallback += UpdateHeartHUD;
        InstantiateHeartContainers();
        UpdateHeartHUD();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < player.maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    private void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < player.health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }

    private void InstantiateHeartContainers()
    {
        for (int i = 0; i < player.maxHealth; i++)
        {
            GameObject _temp = Instantiate(heartContainerPrefab);
            _temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = _temp;
            heartFills[i] = _temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    private void UpdateHeartHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}