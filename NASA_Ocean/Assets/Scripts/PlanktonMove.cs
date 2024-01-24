using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class PlanktonMove : MonoBehaviour
{
    const int planktonCounts = 100; // number of planktons
    const float fieldRange = 20;
    const float sponRange = 10;
    const float maxSpeed = 0.005f;
    const float randominitSpeed = 0.002f;
    public GameObject[] planktons;
    public Quantity[] planktonsProfile;
    public Vector3[] acceleration, velocity;
    public struct Quantity
    {
        public string name;
        public float mass;     
        public float scale;  
        public float speed;
    }

    void Awake()
    {
        planktons = new GameObject[planktonCounts];
        planktonsProfile = new Quantity[planktonCounts];
        acceleration = new Vector3[planktonCounts];
        velocity = new Vector3[planktonCounts];
    }
    // Start is called before the first frame update
    void Start()
    {
        // plankton profiles
        planktonsProfile[0].name = "rhizosolenia"; planktonsProfile[0].mass = 1f; planktonsProfile[0].scale = 1f; planktonsProfile[0].speed = 3f;
        planktonsProfile[1].name = "emiliana"; planktonsProfile[1].mass = 3f; planktonsProfile[1].scale = 10f; planktonsProfile[1].speed = 1f;
        planktonsProfile[2].name = "protoperidinium"; planktonsProfile[2].mass = 2f; planktonsProfile[2].scale = 0.5f; planktonsProfile[2].speed = 2f;

        for (int idx = 0; idx < planktonCounts ; idx++)
        {
            int randPlankton = (int)Random.Range(0, 2.9f);
            planktons[idx] = GameObject.Instantiate(GameObject.Find(planktonsProfile[randPlankton].name));
            planktons[idx].transform.localScale = new Vector3(1,1,1) * planktonsProfile[randPlankton].scale;
            planktons[idx].transform.position = new Vector3(Random.Range(-sponRange, sponRange), Random.Range(-sponRange, sponRange), Random.Range(-sponRange, sponRange));
            planktons[idx].transform.rotation = new Quaternion(Random.Range(0, 3.14f), Random.Range(0, 3.14f), Random.Range(0, 3.14f), Random.Range(0, 3.14f));
            velocity[idx] = new Vector3(Random.Range(-randominitSpeed, randominitSpeed), Random.Range(-randominitSpeed, randominitSpeed), Random.Range(-randominitSpeed, randominitSpeed));
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Acceleration should be neutralized every update
        for (int i = 0; i < planktonCounts; i++)
        {
            acceleration[i] = Vector3.zero; // important
        }
        // push and pull physics
        for (int i = 0; i < planktonCounts; i++)
        {
            for (int j = i + 1; j < planktonCounts; j++) // note this loop. 
            {
                Vector3 diff = planktons[j].transform.position - planktons[i].transform.position; // distance vector
                float dist = diff.magnitude;
                Vector3 attractDirection = diff.normalized;
                // Gravity equation
                float attraction = 0.0000002f / (dist);
                // Gravity is push and pull with same amount. Force: m1 <-> m2
                acceleration[i] += (attractDirection * attraction);  // pull
                acceleration[j] -= (attractDirection * attraction);  // push
                if (dist < 1f)
                {
                    acceleration[i] -= (attractDirection * 50 * attraction);  // push if too close. 
                }
            }
            velocity[i] += acceleration[i]; // fast forward
            planktons[i].transform.position = planktons[i].transform.position + velocity[i];
            planktons[i].transform.rotation = Quaternion.Euler(velocity[i].normalized * 100f);
        }

        // box 
        for (int i = 0; i < planktonCounts; i++)
        {
            // Constrain the max speed
            if (velocity[i].magnitude > maxSpeed)
            {
                velocity[i] = velocity[i].normalized * maxSpeed;
            }
            // constrain the plankton's movement range
            if (planktons[i].transform.position.magnitude > fieldRange)
            {
                velocity[i] = -velocity[i];
            }
        }
    }
 }
