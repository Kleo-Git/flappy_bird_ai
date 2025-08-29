using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controls the ai agents, sending live data to the neural network, and handling jump output from the neural network.
/// </summary>
public class AiBird : MonoBehaviour
{
    public NeuralNetwork neuralNetwork;
    public float jumpStrength = 5;
    public Animator animator;

    //Data being fed into the neural network
    public Vector2 closestTopPipePosition;
    public Vector2 closestBottomPipePosition;
    public Vector2 closestTopPipeDistance;
    public Vector2 closestBottomPipeDistance;
    public Vector2 birdPosition;

    private Rigidbody2D myRigidBody;

    //Start is called before the first frame update
    void Awake()
    {
        //Initialize neural network with 5 input neurons, 1 hidden layer also with 5 neurons, and output layer with 1 neuron
        //5 is a solid choice since it captures pipe x and y distances, (4 pieces of data total) + bird position = 5
        //1 Output layer since the agent only needs to decide between flap / don't flap.
        neuralNetwork = new NeuralNetwork(new int[] { 5, 5, 1 });

        //Asign rigidbody component
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    //Fixed update gives consistent behaviour to birds
    //Using regular update can be problematic, as the neural network can't deal well with unpredictability 
    //in frame drops etc.
    void FixedUpdate()
    {
        //Find birds current position
        birdPosition = myRigidBody.position;

        //Find all gameobjects
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allGameObjects)
        {
            //Search gameobjects list for pipes
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            if (obj.layer == obstacleLayer)
            {
                //If high enough, will always correspond to top pipe
                if (obj.transform.position.y >= 5 && obj.transform.position.x > 0)
                {
                    closestTopPipePosition = obj.transform.position;
                }
                //Else must be bottom pipe
                else if (obj.transform.position.y <= -5 && obj.transform.position.x > 0)
                {
                    closestBottomPipePosition = obj.transform.position;
                }
            }
        }

        if (closestTopPipeDistance != null)
        {
            //Find distance between pipe and bird
            closestTopPipeDistance = closestTopPipePosition - birdPosition;
            //Update distance by height of the pipe (7), to find bottom edge of top pipe
            //This informs bird where the gap between pipes is much better then measuring the top of the top pipe
            //which would be out of bounds anyway
            closestTopPipeDistance.y = Mathf.Abs(closestTopPipeDistance.y - 7);
        }
        if (closestBottomPipeDistance != null)
        {
            //Find distance between pipe and bird
            closestBottomPipeDistance = closestBottomPipePosition - birdPosition;
            //Update distance by height of the pipe (7), to find top of bottom pipe
            closestBottomPipeDistance.y = Mathf.Abs(closestBottomPipeDistance.y + 7);
        }

        //Create array of inputs for the neural network
        float[] inputs = new float[]
        {
            birdPosition.y,
            closestTopPipeDistance.x,
            closestTopPipeDistance.y,
            closestBottomPipeDistance.x,
            closestBottomPipeDistance.y
        };

        //Get the output of a forward pass of the neural network
        float[] output = neuralNetwork.Forward(inputs);

        //If flap probability >50%, flap
        if (output[0] > 0.5f)
        {
            Flap();
        }
    }

    /// <summary>
    /// Handles the flap of the object, in other words a small jump up for the bird
    /// </summary>
    public void Flap()
    {
        Vector2 velocity = myRigidBody.velocity;
        
        //Update the y velocity to jump strength
        velocity.y = jumpStrength;

        //Start animation
        animator.SetBool("Flap", true);
        
        //Handles animation, although birds can flap multiple times while animation is running
        StartCoroutine(ResetFlap());

        myRigidBody.velocity = velocity;
    }
    private IEnumerator ResetFlap()
    {
        //Play the animation over 0.6s, without constantly resetting
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("Flap", false);
    }
    public void SetNetwork(NeuralNetwork neuralNetwork)
    {
        //Set neural network of a given agent
        this.neuralNetwork = neuralNetwork;
    }
}
