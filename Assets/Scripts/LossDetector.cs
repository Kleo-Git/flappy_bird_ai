using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects when a bird hits a pipe or goes outside of the playable area, then destroys it
/// </summary>
public class LossDetector : MonoBehaviour
{
    public float score;
    public Rigidbody2D myRigidBody;
    public Vector2 originPosition = new Vector2(0, 0);
    public BirdSpawner birdSpawner;
    public AiBird aiBird;

    //Start is called before the first frame update
    void Start()
    {
        if (birdSpawner == null)
        {
            birdSpawner = FindObjectOfType<BirdSpawner>(); // Finds the BirdSpawner in the scene
        }

        if (aiBird == null)
        {
            aiBird = FindObjectOfType<AiBird>(); // Finds the AiBird in the scene
        }
    }

    void FixedUpdate()
    {
        //Keeps track of birds current position
        Vector2 myPosition = myRigidBody.position;

        //If bird goes too high or too low, destroy it
        if (Mathf.Abs(myPosition.y) > 5)
        {
            DestroyBird();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //If a bird hits a pipe it gets destroyed
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            DestroyBird();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Update score of bird
        if (collision.gameObject.layer == LayerMask.NameToLayer("Score"))
        {
            score += 1f;
        }
    }

    /// <summary>
    /// Handles passing useful information before destroying the bird
    /// </summary>
    private void DestroyBird()
    {
        //Find the neural network of the bird
        NeuralNetwork neuralNetwork = aiBird.neuralNetwork;

        //Adds neural network of specified bird to a list when destroyed
        birdSpawner.AddNeuralNetwork(neuralNetwork);
        //Removes bird from birds list
        birdSpawner.RemoveBird(gameObject);
        //Adds final score of bird to scores list
        birdSpawner.AddScore(score);

        //Destroy bird
        Destroy(gameObject);
    }
}
