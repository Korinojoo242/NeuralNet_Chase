using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour {
    private bool initilized = false;
    private Transform player;

    private NeuralNetwork net;
    private Rigidbody2D rBody;

    [SerializeField]
    Transform rotationCenter;

    [SerializeField]
    float rotationRadius = 15f, angularSpeed = 0.5f;

    float posX, posY, angle = 0f;

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        posX = rotationCenter.position.x + Mathf.Cos(angle) * rotationRadius;
        posY = rotationCenter.position.y + Mathf.Sin(angle) * rotationRadius;
        transform.position = new Vector2(posX, posY);
        angle = angle + Time.deltaTime * angularSpeed;

        if (angle >= 360f)
            angle = 0f;
    }

    void FixedUpdate ()
    {
        if (initilized == true)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance > 20f)
                distance = 20f;

            float[] inputs = new float[1];


            float angle = transform.eulerAngles.z % 360f;
            if (angle < 0f)
                angle += 360f;

            Vector2 deltaVector = (player.position - transform.position).normalized;
   

            float rad = Mathf.Atan2(deltaVector.y, deltaVector.x);
            rad *= Mathf.Rad2Deg;

            rad = rad % 360;
            if (rad < 0)
            {
                rad = 360 + rad;
            }

            rad = 90f - rad;
            if (rad < 0f)
            {
                rad += 360f;
            }
            rad = 360 - rad;
            rad -= angle;
            if (rad < 0)
                rad = 360 + rad;
            if (rad >= 180f)
            {
                rad = 360 - rad;
                rad *= -1f;
            }
            rad *= Mathf.Deg2Rad;
			
            inputs[0] = rad / (Mathf.PI);


            float[] output = net.FeedMeForward(inputs);

            rBody.velocity = 4.5f * transform.up;
            rBody.angularVelocity = 500f * output[0];

            net.addNeuralFitness((1f-Mathf.Abs(inputs[0])));
        }
	}

    public void Init(NeuralNetwork net, Transform hex)
    {
        this.player = hex;
        this.net = net;
        initilized = true;
    }

    
}
