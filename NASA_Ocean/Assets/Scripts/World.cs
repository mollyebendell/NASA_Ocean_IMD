using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance;
    float frequency;
    public Vector3 startingScale;
    float startingDistanceBetweenHandRayPoints;
    Vector3 startingLeftHandRayPoint;
    Vector3 startingRightHandRayPoint;
    float startingDistanceBetweenHands;
    //FacilitiesGet facilities;
    bool currentlyScaling;
    bool currentlyRotating;
    bool hoveringOverGlobe;
    public Transform tempWorldMap;
    private void Awake()
    {
        if(!instance)
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        startingScale = transform.localScale;
        //facilities = FacilitiesGet.instance;

        if (tempWorldMap)
        {
            transform.position = tempWorldMap.position;
            transform.rotation = tempWorldMap.rotation;
            transform.localScale = tempWorldMap.localScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hoveringOverGlobe)
        {
            FindNearestPlants();
        }
        if (currentlyScaling)
        {
            Vector3 pointBetweenStartingHandRayPoints = startingLeftHandRayPoint + (startingRightHandRayPoint - startingLeftHandRayPoint) / 2;

        }
    }
    public void GlobeSelected()
    {
        Debug.Log("Globe Selected");
    }
    public void GlobeHovering()
    {
        Debug.Log("GLOBE Hover");
        hoveringOverGlobe = true;
    }
    public void GlobeStopHovering()
    {
        hoveringOverGlobe = false;
        Debug.Log("StopHoveringOverGlobe");
    }
    public void FindNearestPlants()
    {
    }
    public void GlobeUnselected()
    {
        currentlyScaling = false;
        currentlyRotating = false;
        Debug.Log("GLOBE UNSELECTED");
    }
}
