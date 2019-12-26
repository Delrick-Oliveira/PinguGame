using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that allow the camera to follow the player
/// </summary>

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] Vector3 offset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 _playRotation = new Vector3(35, 0, 0);

    public bool IsMoving { get; set; }

    private void LateUpdate()
    {
        if (!IsMoving) return;
        Vector3 desiredPosition = target.localPosition + offset;
        desiredPosition.x = 0;
        transform.localPosition  = Vector3.Lerp(transform.localPosition, desiredPosition, Time.deltaTime);
        //isolar em uma courotina
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_playRotation), Time.deltaTime);
    }
}
