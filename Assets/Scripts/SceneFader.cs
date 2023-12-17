using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public float fadeTime;

    private Image fadeOutUIImage;

    public enum FadeDirection
    {
        In,
        Out
    }

    private void Awake()
    {
        fadeOutUIImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void SetColorImage(ref float _alpha, FadeDirection _fadeDir)
    {
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, _alpha);

        _alpha += Time.deltaTime * (1 / fadeTime) * (_fadeDir == FadeDirection.Out ? -1 : 1);
    }

    public IEnumerator Fade(FadeDirection _fadeDir)
    {
        float _alpha = _fadeDir == FadeDirection.Out ? 1 : 0;
        float _fadeEndValue = _fadeDir == FadeDirection.Out ? 0 : 1;
        if (_fadeDir == FadeDirection.Out)
        {
            while (_alpha >= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDir);
                yield return null;
            }

            fadeOutUIImage.enabled = false;
        }
        else
        {
            fadeOutUIImage.enabled = true;
            while (_alpha <= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDir);
                yield return null;
            }
        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDir, string _levelToLoad)
    {
        fadeOutUIImage.enabled = true;
        yield return Fade(_fadeDir);

        SceneManager.LoadScene(_levelToLoad);
    }
}