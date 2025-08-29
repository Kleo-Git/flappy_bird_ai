using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Handles spawning pipes and scorelines
/// </summary>
public class PipeSpawner : MonoBehaviour
{
    public float pipeSpawnTime = 2f;
    public GameObject pipePrefab;
    public GameObject scoreLine;
    public float counter = 0;

    //Reasonable central height point, on right side of screen to spawn from
    public Vector2 spawnPoint = new Vector2(9, -5);
    public int yOffset;

    //Spawn a pipe pair at start 
    void Start()
    {
        SpawnPipePair();
    }

    void Update()
    {
        //Update counter
        counter += Time.deltaTime;

        //Check if counter exceeds pipe spawn time
        if (counter >= pipeSpawnTime)
        {
            SpawnPipePair();
            counter = 0;
        }
    }

    /// <summary>
    /// Spawns pipe pair, and scoreline at the same position
    /// </summary>
    private void SpawnPipePair()
    {
        //Pick a random y-offset so pipes spawn at different heights
        yOffset = UnityEngine.Random.Range(-3, 3);

        //Assign pipe instance and create top pipe
        GameObject pipeInstance = Instantiate(pipePrefab, new Vector2(spawnPoint.x, -spawnPoint.y + yOffset), Quaternion.identity);

        //Flip the pipe upside down
        Vector2 pipeScale2 = pipeInstance.transform.localScale;
        pipeScale2.y = -pipeScale2.y;
        pipeInstance.transform.localScale = pipeScale2;

        //Create bottom pipe
        Instantiate(pipePrefab, new Vector2(spawnPoint.x, spawnPoint.y + yOffset), Quaternion.identity);

        //Instantiate scoreline at same x pos, and lower y pos. Score line covers whole screen so y pos doesn't matter
        Instantiate(scoreLine, new Vector2(9, -10), Quaternion.identity);
    }

}
