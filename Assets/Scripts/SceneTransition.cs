using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    private string transitionTo;
    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Vector2 exitDirection;
    [SerializeField]
    private float exitTime;

    private void Start()
    {
        if (transitionTo == GameManager.Instance.transitionedFromScene)
        {
            PlayerController.Instance.transform.position = startPoint.position;
            // 切换场景时控制角色移动
            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            // 切场景时保存阴影怪数据
            CheckPlayerShadeData();

            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            // 切换场景时通知角色
            PlayerController.Instance.pState.cutScene = true;
            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
        }
    }

    private void CheckPlayerShadeData()
    {
        GameObject[] _enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < _enemyObjects.Length; i++)
        {
            if (_enemyObjects[i].GetComponent<PlayerShade>() != null)
            {
                SaveData.Instance.SavePlayerShadeData();
            }
        }
    }
}