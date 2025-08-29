using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class NeuralNetwork
{
    private int[] layers; // Number of neurons in each layer
    private float[][] neurons; // Neuron values
    private float[][][] weights; // Weights between neurons
    private float[][] biases;

    private System.Random random = new System.Random();

    public NeuralNetwork(int[] layers)
    {
        this.layers = layers;
        InitNeurons();
        InitWeights();
        InitBiases();
    }

    // Initialize neuron arrays
    private void InitNeurons()
    {
        neurons = new float[layers.Length][];
        for (int i = 0; i < layers.Length; i++)
            neurons[i] = new float[layers[i]];
    }

    // Initialize random weights
    private void InitWeights()
    {
        weights = new float[layers.Length - 1][][];
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = new float[layers[i + 1]][];
            for (int j = 0; j < layers[i + 1]; j++)
            {
                weights[i][j] = new float[layers[i]];
                for (int k = 0; k < weights[i][j].Length; k++)
                    weights[i][j][k] = (float)random.NextDouble() * 2 - 1; // Random weights between -1 and 1
            }
        }
    }

     // Initialize random biases
    private void InitBiases()
    {
        biases = new float[layers.Length - 1][];
        for (int i = 1; i < layers.Length; i++)
        {
            biases[i - 1] = new float[layers[i]];
            for (int j = 0; j < biases[i - 1].Length; j++)
                biases[i - 1][j] = (float)random.NextDouble() * 2 - 1; // Random biases between -1 and 1
        }
    }
    
     // Forward pass
    public float[] Forward(float[] inputs)
    {
        // Set input layer
        neurons[0] = inputs;

        // Loop through each layer
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                float value = biases[i - 1][j]; // Start with bias
                for (int k = 0; k < layers[i - 1]; k++)
                    value += neurons[i - 1][k] * weights[i - 1][j][k]; // Add weighted input

                neurons[i][j] = ReLU(value); // Apply activation function
            }
        }

        return neurons[layers.Length - 1]; // Return output layer
    }
    public void AdjustWeights(float learningRate)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float adjustment = UnityEngine.Random.Range(-learningRate, learningRate); 
                    weights[i][j][k] += adjustment;
                }
            }
        }
    }

    // Sigmoid activation function
    private float Sigmoid(float x)
    {
        return 1f / (1f + (float)Math.Exp(-x));
    }

    //ReLu activation function
    private float ReLU(float x)
    {
        return Mathf.Max(0,x);
    }

    public void Mutate(float mutationRate, float mutationAmount)
    {
        // Mutate weights
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    if (random.NextDouble() < mutationRate) // Check if we should mutate this weight
                    {
                        // Mutate by adding a random value to the weight
                        weights[i][j][k] += (float)(random.NextDouble() * 2 - 1) * mutationAmount; // Random change between -mutationAmount and mutationAmount
                    }
                }
            }
        }

        // Mutate biases
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                if (random.NextDouble() < mutationRate) // Check if we should mutate this bias
                {
                    // Mutate by adding a random value to the bias
                    biases[i][j] += (float)(random.NextDouble() * 2 - 1) * mutationAmount; // Random change between -mutationAmount and mutationAmount
                }
            }
        }
    }
    public static NeuralNetwork Crossover(NeuralNetwork networkA, NeuralNetwork networkB)
    {
        NeuralNetwork childNetwork = new NeuralNetwork((int[])networkA.layers.Clone());; //Initialzing network of equal size

        //Taking weights from parents randomly
        for (int i = 0; i < networkA.weights.Length; i++)
        {
            for (int j = 0; j < networkA.weights[i].Length; j++)
            {
                // Randomly take from either parent
                childNetwork.weights[i][j] = UnityEngine.Random.value > 0.5f ? networkA.weights[i][j] : networkB.weights[i][j];
            }
        }
        //Taking biases from parents randomly
        for (int i = 0; i < networkA.biases.Length; i++)
        {
            for (int j = 0; j < networkA.biases[i].Length; j++)
            {
                // Randomly take from either parent
                childNetwork.biases[i][j] = UnityEngine.Random.value < 0.5f ? 
                    networkA.biases[i][j] : networkB.biases[i][j];
            }
        }
        return childNetwork;
    }
    public NeuralNetwork Clone()
    {
        NeuralNetwork clone = new NeuralNetwork((int[])layers.Clone());
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                clone.weights[i][j] = (float[])weights[i][j].Clone();
            }
        }
        for (int i = 0; i < biases.Length; i++)
        {
            clone.biases[i] = (float[])biases[i].Clone();
        }
        return clone;
    }
}
