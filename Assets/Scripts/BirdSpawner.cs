using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

/// <summary>
/// Handles spawning birds at each generation, responsible for assigning NN's to birds in a way that allows improving performance
/// </summary>
public class BirdSpawner : MonoBehaviour
{
    public int numberBirds;
    public GameObject AIBird;
    public int generationNumber;
    public Vector2 originPosition = new Vector2(0, 0);
    public List<GameObject> birds = new List<GameObject>();
    public List<float> scores = new List<float>();
    public List<NeuralNetwork> neuralNetworks = new List<NeuralNetwork>();

    public float mutationRate = 0.15f;
    public float mutationAmount = 0.2f;
    public float smallmutationRate = 0.05f;
    public float smallmutationAmount = 0.1f;

    //Start is called before the first frame update
    void Start()
    {
        //Initialize generation number
        generationNumber = 0;

        //Spawn birds up to the desired number of birds
        for (int i = 0; i < numberBirds; i++)
        {
            //Instantiate bird object
            GameObject bird = Instantiate(AIBird, originPosition, Quaternion.identity);
            //Add to birds list
            birds.Add(bird);
        }
    }

    //Update is called once per frame
    void Update()
    {
        //Debug.Log($"Birds remaining: {birds.Count}");
        //If not birds remain, spawn next generation and clear obstacles
        if (birds.Count == 0)
        {
            NextGeneration();
            ClearObstacles();
        }
    }
    public void RemoveBird(GameObject bird)
    {
        //Remove bird from birds list
        birds.Remove(bird);
    }
    public void AddScore(float score)
    {
        //Add score of bird to scores list
        scores.Add(score);
    }
    public void AddNeuralNetwork(NeuralNetwork neuralNetwork)
    {
        //Add neural network to neuralNetworks list
        neuralNetworks.Add(neuralNetwork);
    }

    /// <summary>
    /// Handles spawning next generation with appropriately designated neural networks
    /// </summary>
    public void NextGeneration()
    {
        //Number of NN's to generate randomly
        int randomBrains = (int)Mathf.Floor(0.39f * numberBirds);

        //Number of NN's to copy exactly from best performing birds
        int exactBestBrains = (int)Mathf.Floor(0.01f * numberBirds);

        //Number of NN's that will be subject to mutations
        int slightlyMutatedBrains = (int)Mathf.Floor(0.1f * numberBirds);


        //Debug.Log($"Scores: {string.Join(", ", scores)}");

        //Find max score in scores list for given generation
        float maxScore = scores.Max();

        //Find average score
        float averageScore = scores.Sum() / scores.Count();

        Debug.Log($"Average Score: {averageScore}");
        Debug.Log($"Max Score: {maxScore}");

        //Find top half best performing neural networks
        //This works since NN's are added after birds die, this naturally means the better performing NN's
        //will be placed later in the list
        List<NeuralNetwork> bestNetworks = neuralNetworks.Skip(neuralNetworks.Count / 2).ToList();

        //Update generation number
        generationNumber++;

        for (int i = 0; i < randomBrains; i++) //Initialize random brains
        {
            //Instaniate bird like normal, will have random weights and biases
            GameObject bird = Instantiate(AIBird, originPosition, Quaternion.identity);
            birds.Add(bird);
        }

        for (int i = 0; i < exactBestBrains; i++) //Initialize birds with best brains
        {
            //Pick best brains from the end of the list
            NeuralNetwork bestBrain = bestNetworks[bestNetworks.Count - 1 - i];

            //Instantiate bird
            GameObject bird = Instantiate(AIBird, originPosition, Quaternion.identity);
            AiBird aiBird = bird.GetComponent<AiBird>(); // Get the BirdAI component

            //Set the birds brain to the best brain
            aiBird.SetNetwork(bestBrain);
            birds.Add(bird);
        }

        for (int i = 0; i < slightlyMutatedBrains; i++) //Initialize with best brains slightly mutated
        {
            //Pick best brains from end of list, will overlap slightly with exact best brains currently
            //This shouldn't be too big of a problem as exactBestBrains is a very small sample size, although may need addressing
            NeuralNetwork topBrain = bestNetworks[bestNetworks.Count - 1 - i];

            //Create a copy of the chosen brain
            NeuralNetwork mutatedBrain = topBrain.Clone();

            //Apply small chance of mutations to the current brain
            mutatedBrain.Mutate(smallmutationRate, smallmutationAmount);

            GameObject bird = Instantiate(AIBird, originPosition, Quaternion.identity);
            AiBird aiBird = bird.GetComponent<AiBird>(); // Get the BirdAI component

            //Set birds brain to mutated brain
            aiBird.SetNetwork(mutatedBrain);
            birds.Add(bird);
        }

        for (int i = 0; i < 0.50f * numberBirds; i++) //Initialize with top % brains mutated and crossovered
        {
            //Pick random indexes from top half of NN's list
            int indexA = Random.Range(0, bestNetworks.Count);
            int indexB = Random.Range(0, bestNetworks.Count);

            //Randomly selected brains from top 50% performers
            NeuralNetwork selectedBrainA = bestNetworks[indexA];
            NeuralNetwork selectedBrainB = bestNetworks[indexB];

            //Create a new brain which is a crossover of selected brains
            NeuralNetwork newBrain = NeuralNetwork.Crossover(selectedBrainA, selectedBrainB);
            NeuralNetwork mutatedBrain = newBrain.Clone();

            //Mutate the crossover brain
            mutatedBrain.Mutate(mutationRate, mutationAmount);

            GameObject bird = Instantiate(AIBird, originPosition, Quaternion.identity);
            AiBird aiBird = bird.GetComponent<AiBird>();

            //Assign the crossovered and mutated brain to the bird
            aiBird.SetNetwork(mutatedBrain);
            birds.Add(bird);
        }

        //Clear lists of data from previous generation
        neuralNetworks.Clear();
        scores.Clear();
        bestNetworks.Clear();
    }
    public void ClearObstacles()
    {
        //Search all gameobjects
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();

        //Check for all pipes and score lines, wipe them all when called
        foreach (GameObject obj in allGameObjects)
        {

            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            int scoreLayer = LayerMask.NameToLayer("Score");

            // Check if the GameObject is in the "Obstacle" layer
            if (obj.layer == obstacleLayer || obj.layer == scoreLayer)
            {
                // Destroy the GameObject
                Destroy(obj);
            }
        }
    }

}
