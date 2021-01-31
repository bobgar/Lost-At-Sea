using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PhysicsBoatController : MonoBehaviour
{
    public WorldManager worldManager;
    public ParticleSystem ps;

    protected Camera Camera;

    public bool attached = true;
    public Rigidbody rigidBody;

    public GameObject[] buoyancyPoints;
    public float buoyancyLevel = 0.5f;
    public float buoyancyForce = 20f;
    public float enginePower = 50f;
    public float turnPower = 50f;

    public void Awake()
    {
        Camera = Camera.main;
        rigidBody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {

        bool isUpsideDown = Vector3.Dot(transform.up, new Vector3(0, 1, 0)) < 0;
        
        bool touchingWater = false;

        if (isUpsideDown)
        {
            if(transform.position.y < 3)
            {
                Debug.Log("YOU'RE SUNK!");
            }
        }
        else
        {
            foreach (GameObject g in buoyancyPoints)
            {
                if (g.transform.position.y < buoyancyLevel)
                {
                    rigidBody.AddForceAtPosition(new Vector3(0, 1, 0) * (buoyancyLevel - transform.position.y) * buoyancyForce, g.transform.position);
                    touchingWater = true;
                }
            }
        }

        if (!attached) { return; }

        if (Input.GetKey(KeyCode.Space))
        {
            worldManager.SwitchToCharacter();
        }

        if (touchingWater)
        {
            //Left
            if (Input.GetKey(KeyCode.A))
            {
                rigidBody.AddTorque(new Vector3(0, -turnPower, 0));
            }

            //Right
            if (Input.GetKey(KeyCode.D))
            {
                rigidBody.AddTorque(new Vector3(0, turnPower, 0));
            }

            //Forward
            if (Input.GetKey(KeyCode.W))
            {
                rigidBody.AddForce(new Vector3(transform.forward.x, 0, transform.forward.z) * enginePower);
            }

            //Backwards
            if (Input.GetKey(KeyCode.S))
            {

            }
        }

        if (rigidBody.velocity.magnitude > 0.3f)
        {
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTime = (rigidBody.velocity.magnitude - 1f);
            emission.enabled = true;
        }
        else
        {
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.enabled = false;
        }

        Vector3 cameraTarget = transform.position - transform.forward * 10f;

        Camera.transform.position = Vector3.Lerp(Camera.transform.position, new Vector3(cameraTarget.x, Camera.transform.position.y, cameraTarget.z), .1f);
        Camera.transform.LookAt(transform);


    }



}