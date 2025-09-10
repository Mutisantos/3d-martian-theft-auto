using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedTreeController : MonoBehaviour
{
    private Animator myAnimator;
    private float speed = 0.0f;
    private float mouseWheelSpeed = 0.05f;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Con la rueda del ratón cambiamos su velocidad
        speed += Input.mouseScrollDelta.y * mouseWheelSpeed;
        speed = Mathf.Clamp01(speed);
            
        myAnimator.SetFloat("Speed",speed);
    }
}
