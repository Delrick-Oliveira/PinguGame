using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private int _maxCoins = 5;
    [SerializeField, Range(0f,1f)] 
    private float _chanceToSpawn = 0.5f;
    [SerializeField] private bool _forceSpawnAll = false;

    private GameObject[] coins;

    private void Awake()
    {
        coins = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            coins[i] = transform.GetChild(i).gameObject;
        }

        OnDisable();
    }

    private void OnEnable()
    {
        if (Random.Range(0.0f, 1.0f) > _chanceToSpawn)
            return;

        if(_forceSpawnAll)
        {
            for (int i = 0; i < _maxCoins; i++)
            {
                coins[i].SetActive(true);
            }
        }
        else
        {
            int r = Random.Range(0, _maxCoins);
            for(int i = 0; i<r; i++)
            {
                coins[i].SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        foreach (GameObject go in coins)
        {
            go.SetActive(false);
        }
    }
}
