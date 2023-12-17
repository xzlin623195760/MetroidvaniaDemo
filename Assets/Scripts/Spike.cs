using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField]
    private float spikeDamage = 1;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    private IEnumerator RespawnPoint()
    {
        PlayerController.Instance.pState.cutScene = true;
        PlayerController.Instance.pState.invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        PlayerController.Instance.TakeDamage(spikeDamage);
        yield return new WaitForSecondsRealtime(1);

        PlayerController.Instance.transform.position = GameManager.Instance.platformingRespawnPoint;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);

        PlayerController.Instance.pState.cutScene = false;
        PlayerController.Instance.pState.invincible = false;
        Time.timeScale = 1;
    }
}