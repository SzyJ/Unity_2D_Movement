using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;

    float dx = 0;

    public float runSpeed = 40f;

    // Update is called once per frame
    void Update()
    {
        dx = Input.GetAxisRaw("Horizontal") * runSpeed;
    }

    // Apply input to player
    void FixedUpdate()
    {
        controller.Move(dx * Time.fixedDeltaTime, false, false);
    }
}
