using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator _coinAnim;

    private void Awake()
    {
        _coinAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _coinAnim.SetTrigger("Spawn");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.GetCoin();
            _coinAnim.SetTrigger("Collected");
        }
            
    }
}
