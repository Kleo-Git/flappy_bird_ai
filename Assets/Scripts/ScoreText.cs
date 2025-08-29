using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEditor.Build.Player;
public class ScoreText : MonoBehaviour
{
    public int birdLayer;
    public TextMeshProUGUI textMeshPro;
    public BirdSpawner birdSpawner;  
    public float maxScore = 0f;
    
    private void Start()
    {
        if (birdSpawner == null)
        {
            birdSpawner = FindObjectOfType<BirdSpawner>(); // Finds the BirdSpawner in the scene
        }

        //Display text
        textMeshPro.text = "Score: " + 0 + "\n" +
                   "Number of Birds: " + 0 + "\n" +
                   "Generation: " + 0;
    }

    // Update is called once per frame
    private void Update()
    {
        //Find current number of birds
        int numberBirds = birdSpawner.birds.Count();

        //Reser max score
        if (numberBirds == 0)
        {
            maxScore = 0;
        }

        //Find max score
        else
        {
            FindMaxScore();
        }

        //Find current gen number
        int genNumber = birdSpawner.generationNumber;

        //Display current stats
        textMeshPro.text = "Score: " + maxScore + "\n" + 
                   "Number of Birds: " + numberBirds + "\n" + 
                   "Generation: " + genNumber;
    }


    void FindMaxScore()
    {
        maxScore = 0;
        GameObject[] birdsObjects = GameObject.FindGameObjectsWithTag("Bird");

        //Search all birds, and find max score of all birds
        foreach (GameObject obj in birdsObjects)
        {
            var birdScript = obj.GetComponent<LossDetector>();

            if (birdScript != null)
            {
                if (birdScript.score > maxScore)
                {
                    maxScore = birdScript.score;
                }
            }
        }
    }
}
