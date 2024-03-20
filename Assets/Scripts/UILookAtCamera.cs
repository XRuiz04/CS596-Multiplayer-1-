using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    // This function is called once per frame after all Update functions have been called.
    // It makes the UI element face towards the camera every frame, ensuring it's always readable by the player.
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
