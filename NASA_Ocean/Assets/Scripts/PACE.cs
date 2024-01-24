using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PACE : MonoBehaviour
{
    private GameObject pace;
    private GameObject earth, data;
    public Vector3 pacePose;
    public float time;
    private float radius;
    Material dataMaterial;

    // Start is called before the first frame update
    void Start()
    {
        pace = GameObject.Find("PACE");
        earth = GameObject.Find("sphere for earth");
        data = GameObject.Find("sphere for data");
        time = 0;
        radius = 24;
        dataMaterial = data.GetComponent<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {

        pacePose = new Vector3( radius * Mathf.Sin(time), radius * Mathf.Cos(time), 0 );
        //pace.transform.position = pacePose;
        earth.transform.Rotate(new Vector3(0, time/300, 0));
        data.transform.Rotate(new Vector3(0, time/300, 0));
        dataMaterial.SetFloat("_Metallic", Mathf.Sin(time*0.5f));
        time += Time.deltaTime;
    }
}
