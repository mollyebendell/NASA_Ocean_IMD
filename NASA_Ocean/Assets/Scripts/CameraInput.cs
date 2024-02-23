// This script is used in the underwater scene in order to get the camera input and find the average 
//red, green, and blue rgb value of the whole scene. This value is then set as a varaible on each respective 
//photoplanktin object
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;


public class CameraInput : MonoBehaviour
{
    //get gameobjects so we can later set how many of them we want in var compoment
    public GameObject rhizosolenia;
    public GameObject emiliana;
    public GameObject protoperidinium;

    //webcam setup code
    const int waitWidth = 16;
    public int width = 640;
    public int height = 360;
    Color32[] pixels;
    WebCamTexture cam;    
    Action update;

    // Start is called before the first frame update
    void Start()
    {
        update = WaitingForCam;

        cam = new WebCamTexture(WebCamTexture.devices[0].name, width, height);
        cam.Play();
    }

    // Update is called once per frame
    void Update()
    {
        update();
    }

    void WaitingForCam()
    {
        if (cam.width > waitWidth)
        {
            width = cam.width;
            height = cam.height;
            pixels = new Color32[cam.width * cam.height];
            update = CamIsOn;
        }
    }

    void CamIsOn()
    {
        if (cam.didUpdateThisFrame)
        {
            cam.GetPixels32(pixels);
            int red = 0;
            int blue = 0;
            int green = 0;


            // get average of red, green, and blue pixel values
            for (int i = 0; i < pixels.Length; i++)
            {
                red += pixels[i].r;
                green += pixels[i].g;
                blue += pixels[i].b;
            }

            red = red/pixels.Length;
            green = green/pixels.Length;
            blue = blue/pixels.Length;

            //normalize pixel values from 0-50 and set var for each gameobject
            red = (int) normalizedColor(red);
            Variables.Object(rhizosolenia).Set("red", red);
            
            green = (int) normalizedColor(green);
            Variables.Object(emiliana).Set("green", green);

            
            blue = (int) normalizedColor(blue);
            Variables.Object(protoperidinium).Set("blue", blue);


        }

        double normalizedColor(int color){
            return (50.0 * (((double) color / 255.0)));
        }
    }
}
