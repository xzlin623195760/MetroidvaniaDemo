using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRespawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            GameManager.Instance.platformingRespawnPoint = transform.position;
        }
    }
}