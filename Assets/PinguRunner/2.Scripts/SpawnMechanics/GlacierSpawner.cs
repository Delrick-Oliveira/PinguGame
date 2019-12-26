using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacierSpawner : MonoBehaviour
{
    [SerializeField] private float _distanceToRespawn = 10.0f;
    [SerializeField] private float _scrollSpeed = -2f;
    [SerializeField] private float _totalLenght = 0;
    [SerializeField] private Transform playerTransform = null;

    private float _scrollLocation;

    public bool IsScrolling { set; get; }


    private void Update()
    {
        if (!IsScrolling)
            return;

        _scrollLocation += _scrollSpeed * Time.deltaTime;
        Vector3 newLocation = (playerTransform.localPosition.z + _scrollLocation) * Vector3.forward;
        transform.localPosition = newLocation;

        if(transform.GetChild(0).transform.position.z < playerTransform.localPosition.z - _distanceToRespawn)
        {
            transform.GetChild(0).localPosition += Vector3.forward * _totalLenght;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);

            // moving the second paralel glacier
            transform.GetChild(0).localPosition += Vector3.forward * _totalLenght;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);
        }
    }
}
