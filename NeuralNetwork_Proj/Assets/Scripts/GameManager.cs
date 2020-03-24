using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//Egor Lukoyanov (100633412)

//References used: "Neural Networks in Unity" by Abhishek Nandy and Manisha Biswas (Apress, 2018).


public class GameManager : MonoBehaviour
{
    //public variables
    public GameObject EnemyPrefab;
    public GameObject player;

    //training of AI and population generation
    private bool Traning = false;
    private int populationSize = 40;
    private int generationNumber = 0;
    private int[] layers = new int[] { 1, 10, 10, 1 }; //1 input and 1 output

    //list to contain neural network 
    private List<NeuralNetwork> nets;

    //list to contain comets
    private List<Spaceship> spaceshipsList = null;

    void Update()
    {

        if (Traning == false)
        {
            if (generationNumber == 0)
            {
                InitCometNeuralNetworks();
            }

            else
            {
                nets.Sort();

                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i].neuralMutation();

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); //so just going to make a deepcopy lol
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].setNeuralFitness(0f);
                }
            }

            generationNumber++;

            Traning = true;
            Invoke("Timer", 15f);
            CreateCometBodies();
            
        }

    }


    //creating our asteroid AI to follow the spaceship
    private void CreateCometBodies()
    {
        if (spaceshipsList != null)
        {
            for (int i = 0; i < spaceshipsList.Count; i++)
            {
                GameObject.Destroy(spaceshipsList[i].gameObject);
            }

        }

        spaceshipsList = new List<Spaceship>();

        //create our asteroids passed on the specified population list
        for (int i = 0; i < populationSize; i++)
        {
            Spaceship spaceship = ((GameObject)Instantiate(EnemyPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), EnemyPrefab.transform.rotation)).GetComponent<Spaceship>();
            spaceship.Init(nets[i], player.transform);
            spaceshipsList.Add(spaceship);
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
    void InitCometNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        //instantiate our neural network 
        nets = new List<NeuralNetwork>();

        //create a new list for mutations to be applied to, the add this to our original neural network list
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.neuralMutation();
            nets.Add(net);
        }
    }

 
    void Timer()
    {
        Traning = false;
    }

}
