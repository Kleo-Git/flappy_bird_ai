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
            FindMaxScoreInLayer();
        }

        //Find current gen number
        int genNumber = birdSpawner.generationNumber;

        //Display current stats
        textMeshPro.text = "Score: " + maxScore + "\n" + 
                   "Number of Birds: " + numberBirds + "\n" + 
                   "Generation: " + genNumber;
    }


    void FindMaxScoreInLayer()
    {
        maxScore = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        //Search all birds, and find max score of all birds
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == birdLayer)
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
}
