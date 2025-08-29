using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


/// <summary>
/// Moves score lines left across the screen and destroys them when off-screen
/// </summary>
public class ScoreLine : MonoBehaviour
{
    //Speed at which score line moves, matches pipe speed
    float horizontalSpeed = -5f;
    float verticalSpeed = 0f;
    public Rigidbody2D scoreLineRigidBody;
    private Vector2 velocity;

    //Fixed update to keep consistent physics, independent of frame rate
    void FixedUpdate()
    {
        //Move left at constant speed
        velocity = new Vector2(horizontalSpeed, verticalSpeed);
        scoreLineRigidBody.velocity = velocity;

        //Destroy when sufficiently far off screen
        if (scoreLineRigidBody.position.x < -10)
        {
            Destroy(gameObject);
        }
    }
}
