using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public GameObject player;

    private bool Traning = false;
    private int populationSize = 50;
    private int generationNumber = 0;
    private int[] layers = new int[] { 1, 10, 10, 1 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private bool leftMouseDown = false;
    private List<Spaceship> spaceshipsList = null;


    void Timer()
    {
        Traning = false;
    }


    void Update()
    {
        if (Traning == false)
        {
            if (generationNumber == 0)
            {
                InitSpaceshipNeuralNetworks();
            }
            else
            {
                nets.Sort();
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i].neuralMutation();

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].setNeuralFitness(0f);
                }
            }

            generationNumber++;

            Traning = true;
            Invoke("Timer", 15f);
            CreateSpaceshipBodies();
        }


        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
        }

        if (leftMouseDown == true)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            player.transform.position = mousePosition;
        }
    }


    private void CreateSpaceshipBodies()
    {
        if (spaceshipsList != null)
        {
            for (int i = 0; i < spaceshipsList.Count; i++)
            {
                GameObject.Destroy(spaceshipsList[i].gameObject);
            }

        }

        spaceshipsList = new List<Spaceship>();

        for (int i = 0; i < populationSize; i++)
        {
            Spaceship spaceship = ((GameObject)Instantiate(EnemyPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), EnemyPrefab.transform.rotation)).GetComponent<Spaceship>();
            spaceship.Init(nets[i], player.transform);
            spaceshipsList.Add(spaceship);
        }

    }

    void InitSpaceshipNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetwork>();


        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.neuralMutation();
            nets.Add(net);
        }
    }
}
