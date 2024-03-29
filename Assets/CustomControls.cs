using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CustomControls : MonoBehaviour
{
    public GameObject player;
    private GameObject clock;
    private Toggle flat;
    private Toggle straight;
    private Toggle rotateTime;
    private Toggle zoom;
    private Toggle twist;
    public float ThrustForce;
    //public Rigidbody NaviBase;
    private Slider mainSlider;
    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        // Rewrite this to navigate player with standard WASD controls, SHIFT for boost, and arrow keys for scaling
        if (Input.GetKey("a"))
            player.transform.position += player.transform.right * -ThrustForce;
        if (Input.GetKey("d"))
            player.transform.position += player.transform.right * ThrustForce;
        if (Input.GetKey("w"))
            player.transform.position += player.transform.forward * ThrustForce;
        if (Input.GetKey("s"))
            player.transform.position += player.transform.forward * -ThrustForce;
        if (Input.GetKey("q"))
            player.transform.position += player.transform.up * -ThrustForce;
        if (Input.GetKey("e"))
            player.transform.position += player.transform.up * ThrustForce;
        if (Input.GetKey("z"))
            player.transform.Rotate(0, 0, 1);
        if (Input.GetKey("x"))
            player.transform.Rotate(0, 0, -1);
        if (Input.GetKey("c"))
            player.transform.Rotate(0, 1, 0);
        if (Input.GetKey("v"))
            player.transform.Rotate(0, -1, 0);
        if (Input.GetKey("b"))
            player.transform.Rotate(1, 0, 0);
        if (Input.GetKey("n"))
            player.transform.Rotate(-1, 0, 0);
        if (Input.GetKey("r"))
            player.transform.position = new Vector3(0, 0, 0);


    }
}