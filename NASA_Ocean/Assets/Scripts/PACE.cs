using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PACE : MonoBehaviour
{
    private GameObject pace;
    public Vector3 pacePose;
    public float time;
    private float radius;
    // Start is called before the first frame update
    void Start()
    {
        pace = GameObject.Find("PACE");
        time = 0;
        radius = 24;
    }  

    // Update is called once per frame
    void Update()
    {

        pacePose = new Vector3( radius * Mathf.Sin(time), radius * Mathf.Cos(time), 0 );
        pace.transform.transform.position = pacePose;
        time += Time.deltaTime;
    }
}
