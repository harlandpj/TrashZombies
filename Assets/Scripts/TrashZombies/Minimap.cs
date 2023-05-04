using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform playerCamera;

    // rotate minimap with player movement and when stationary also
    private void LateUpdate()
    {
        Vector3 newPosition = playerCamera.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, playerCamera.eulerAngles.y,0f);
    }
}
