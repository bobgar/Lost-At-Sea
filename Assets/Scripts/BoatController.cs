using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoatController : MonoBehaviour
{
    public ParticleSystem ps;

    protected Camera Camera;
    public float dampen = .95f;
    public float acceleration = .5f;    
    protected Vector3 velocity = new Vector3();

    public float rotationAcceleration = .5f;
    public float rotationDampen = .7f;
    protected float rotationVelocity = 0;

    public bool attached = true;

    public void Awake()
    {       
        Camera = Camera.main;
    }

    public void FixedUpdate()
    {
        if (!attached) { return; }

        //Left
        if (Input.GetKey(KeyCode.A)) 
        {
            rotationVelocity -= rotationAcceleration;
        }

        //Right
        if (Input.GetKey(KeyCode.D)) 
        {
            rotationVelocity += rotationAcceleration;
        }

        //Forward
        if (Input.GetKey(KeyCode.W))
        {
            velocity = (transform.forward * acceleration);
        }

        //Backwards
        if (Input.GetKey(KeyCode.S))
        {

        }

        transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, rotationVelocity, 0));
        rotationVelocity *= rotationDampen;

        transform.position += velocity;
        velocity *= dampen;

        Vector3 cameraTarget = transform.position - transform.forward * 10f;

        Camera.transform.position = Vector3.Lerp(Camera.transform.position, new Vector3(cameraTarget.x, Camera.transform.position.y, cameraTarget.z) , .3f);
        Camera.transform.LookAt(transform);

        if(velocity.magnitude > 0.3f)
        {
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rate = 500 * (velocity.magnitude - .3f);
            emission.enabled = true;         
        }
        else
        {
            ParticleSystem.EmissionModule emission = ps.emission;            
            emission.enabled = false;
        }
    }



}