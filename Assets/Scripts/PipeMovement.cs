using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Moves pipes left across the screen and destroys them when off-screen
/// </summary>
public class PipeMovement : MonoBehaviour
{
    float horizontalSpeed = -5f;
    float verticalSpeed = 0f;
    public Rigidbody2D pipeRigidBody;
    private Vector2 velocity;

    void FixedUpdate()
    {
        //Move left at constant speed
        velocity = new Vector2(horizontalSpeed, verticalSpeed);
        pipeRigidBody.velocity = velocity;

        //Destroy when sufficiently far off screen
        if (pipeRigidBody.position.x < -10)
        {
            Destroy(gameObject);
        }

    }
}
