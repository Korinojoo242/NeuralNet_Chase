using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//Gabrielle Hollaender (100623554)
//Using a FeedForward Neural network

//References used: "Neural Networks in Unity" by Abhishek Nandy and Manisha Biswas (Apress, 2018).

public class NeuralNetwork : IComparable<NeuralNetwork>
{
	//array variables for our neural network layers

    private int [] layerList; //layers
	private float [][] n_neurons; //list of neurons
	private float [][][] n_weights; //list of weights
    private float n_fitness; //network fitness
  
    //constructor
    //to construct our neural network it is important to provide a list of layers
    public NeuralNetwork(int[] layerList)
	{
		this.layerList = new int[layerList.Length];
		
		for(int counter = 0; counter <layerList.Length; counter++)
		{
			this.layerList[counter] = layerList[counter];
		}

        //initialize our neurons 
        InitializeNeurons();
		
		//initialize our weights 
		InitializeWeights();
	}

    //deep copy of the neural network for the weights
    public NeuralNetwork (NeuralNetwork copyNeuralNetwork)
    {
        this.layerList = new int[copyNeuralNetwork.layerList.Length];

        for (int counter = 0; counter < copyNeuralNetwork.layerList.Length; counter++)
        {
            this.layerList[counter] = copyNeuralNetwork.layerList[counter];
        }

        //create neuron matrix
        InitializeNeurons();

        //create weight matrix
        InitializeWeights();
        
        //copy dem weights
        CopyWeights(copyNeuralNetwork.n_weights);
    }

    private void CopyWeights(float[][][] copy_Weights)
    {
        //copy weights from each row and set our neural network weights to the copied weights
        for (int f = 0; f < n_weights.Length; f++)
        {
            for (int g = 0; g < n_weights[f].Length; g++)
            {
                for (int h = 0; h < n_weights[f][g].Length; h++)
                {
                    n_weights[f][g][h] = copy_Weights[f][g][h];
                }
            }
        }
    }
    private void InitializeNeurons()
    {
        //neuron matrix
        //create a new list to hold our neurons 
        List<float[]> neurons = new List<float[]>();

        for(int counter = 0; counter < layerList.Length; counter++)
        {
            //add a new float to our list based off our layer list
            neurons.Add(new float[layerList[counter]]);
        }

        //helps generate our neuron matrix
        n_neurons = neurons.ToArray();
    }

    private void InitializeWeights()
    {
        //later converting into a 3D array
        //create matrix to hold the weight
        List<float[][]> weights = new List<float[][]>();

        //go through every neuron that has a connection to weight
        //has to contain the weights of every neuron
        for (int counter = 1; counter < layerList.Length; counter++)
        {
            List<float[]> weightsList = new List<float[]>();

            //how many neurons were in the previous layer
            int neuronPrior = layerList[counter - 1];

            //another for loop :(
            for (int i = 0; i < n_neurons[counter].Length; i++)
            {
                float[] neu_Weights = new float[neuronPrior];
                
                //does this do it "for" you XD
                for(int j = 0; j < neuronPrior ; j++)
                {
                    //get random function working for this
                    neu_Weights[j] = UnityEngine.Random.Range(-0.5f, 0.5f); 
                }
                weightsList.Add(neu_Weights);
            }
            weights.Add(weightsList.ToArray());
        }
        n_weights = weights.ToArray();
    }

    public float[] FeedMeForward(float[] n_inputs)
    {
        for (int l = 0; l < n_inputs.Length; l++)
        {
            n_neurons[0][l] = n_inputs[l];
        }

        //go through neurons and get feedforward values
        //this is done by iterating through our layers, uerons and calculating the value based on the gathered weights
        for (int l = 1; l < layerList.Length; l++)
        {
            for(int m = 0; m < n_neurons[l].Length; m++)
            {
                //set our value to zero
                float val = 0f;

                //
                for(int n = 0; n < n_neurons[l-1].Length; n++)
                {
                    val += n_weights[l - 1][m][n] * n_neurons[l - 1][n];
                }

                //hyperbolic tangent 
                n_neurons[l][m] = (float)Mathf.Tan(val);
            }
        }

        return n_neurons[n_neurons.Length - 1];
    }

    //mutate the neural network 
    public void neuralMutation()
    {
        for(int x = 0; x < n_weights.Length; x++)
        {
            for (int y = 0; y < n_weights[x].Length; y++)
            {
                for (int z = 0; z < n_weights[x][y].Length; z++)
                {
                    float neural_Weight = n_weights[x][y][z];

                    //change our weight value 
                    float rando_Num = UnityEngine.Random.Range(0f, 100f);

                    if (rando_Num <= 2.0f)
                    {
                        // flip the weight sign
                        neural_Weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }

                    else if(rando_Num <= 4.0f)
                    {
                        neural_Weight *= -1.0f;
                    }
                    else if (rando_Num <= 6.0f)
                    {
                        // random increase it to the max

                        float mult_factor = UnityEngine.Random.Range(0f, 1.0f) + 1;

                        neural_Weight *= mult_factor;
                    }

                    else if (rando_Num <= 8.0f)
                    {
                        //if the number is 4, randomly decrease

                        float mult_factor = UnityEngine.Random.Range(0f, 1.0f);

                        neural_Weight *= mult_factor;
                    }
                    n_weights[x][y][z] = neural_Weight;
                }
            }
        }
    }

    //lets compare a network if we need to :D
    public int CompareTo(NeuralNetwork otherNetwork)
    {
        //if there is no neural network
        if(otherNetwork == null)
        {
            return 1; 
        }

        //if our network is greater than the other network's fitness
        if (n_fitness > otherNetwork.n_fitness)
        {
            return 1;
        }

        //if the neural network is less than the other network
        else if (n_fitness < otherNetwork.n_fitness)
        {
            return -1;
        }

        //if all else fails return zero
        else
        {
            return 0;
        }
    }

    public void addNeuralFitness(float fitness)
    {
        n_fitness += fitness;
    }

    //set and get functions for fitness
    public void setNeuralFitness(float fitness)
    {
        n_fitness = fitness;
    }

    public float getNeuralFitness()
    {
        return n_fitness;
    }
}
