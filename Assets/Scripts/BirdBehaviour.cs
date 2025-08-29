using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SocialPlatforms.Impl;
using UnityEditor;

public class BirdBehaviour : MonoBehaviour
{
    public float score;
    public Vector2 originPosition = new Vector2(0,0);
    public float jumpStrength = 5f;
    public Rigidbody2D myRigidBody;
    public Animator animator;
    public int input;


    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("Flap", false);
    }

    // Update is called once per frame
    void Update()
    {

        input = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Flap();
            //Debug.Log("Flap set true");
        }

        
        Vector2 myPosition = myRigidBody.position;
        
        if (Mathf.Abs(myPosition.y) > 5)
        {
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            int scoreLayer = LayerMask.NameToLayer("Score");

            myPosition = originPosition;
            myRigidBody.position = originPosition;

            GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allGameObjects)
            {
                // Check if the GameObject is in the "Obstacle" layer
                if (obj.layer == obstacleLayer || obj.layer == scoreLayer)
                {
                    // Destroy the GameObject
                    Destroy(obj);
                }
            }
            
            myRigidBody.velocity = new Vector2(0, 0);
            score = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            int scoreLayer = LayerMask.NameToLayer("Score");

            Vector2 myPosition = myRigidBody.position;
            myPosition = originPosition;
            myRigidBody.position = originPosition;

            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allGameObjects)
            {
                // Check if the GameObject is in the "Obstacle" layer
                if (obj.layer == obstacleLayer || obj.layer == scoreLayer)
                {
                    // Destroy the GameObject
                    Destroy(obj);
                }
            }
            myRigidBody.velocity = new Vector2(0,0);
            score = 0;
        }
    }
    void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Score"))
        {
            score += 0.5f;
            //Debug.Log(score);
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
    
    private IEnumerator ResetFlap()
    {
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("Flap", false);
        //Debug.Log("Flap set false");
    }
    public void Flap()
    {
        Vector2 velocity = myRigidBody.velocity;
        velocity.y = jumpStrength;
        animator.SetBool("Flap", true);
        StartCoroutine(ResetFlap());
        input = 1;
        myRigidBody.velocity = velocity;
    }
}

