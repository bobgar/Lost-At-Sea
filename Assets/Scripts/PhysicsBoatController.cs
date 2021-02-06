using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PhysicsBoatController : MonoBehaviour
{
    public WorldManager worldManager;

    protected Camera Camera;

    public bool attached = true;
    public Rigidbody rigidBody;

    public GameObject[] buoyancyPoints;
    public float buoyancyLevel = 0.5f;
    public float buoyancyForce = 20f;
    public float enginePower = 50f;
    public float turnPower = 50f;

    public bool sinking = false;
    public void Awake()
    {
        Camera = Camera.main;
        rigidBody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {

        bool isUpsideDown =  Vector3.Dot(transform.up, new Vector3(0, 1, 0)) < .3f;
        
        bool touchingWater = false;

        if (isUpsideDown)
        {
            if(transform.position.y < 0)
            {
                sinking = true;
            }
        }
        else
        {
            sinking = false;

            foreach (GameObject g in buoyancyPoints)
            {
                if (g.transform.position.y < buoyancyLevel)
                {
                    float b = (float) Math.Pow((buoyancyLevel - transform.position.y), 2) * buoyancyForce;
                    b = b < -Physics.gravity.y * 1.1f ? b : -Physics.gravity.y * 1.1f;
                    rigidBody.AddForceAtPosition(new Vector3(0, 1, 0) * b , g.transform.position);
                    touchingWater = true;
                }
            }
        }

        if (!attached) 
        {
            //You can roll your boat!  Roll roll roll your boat!
            if (Input.GetKey(KeyCode.E) && worldManager.character != null && (worldManager.character.transform.position - this.transform.position).magnitude < 4f )
            {
                Vector3 pushPoint = worldManager.character.transform.position + new Vector3(0, -2f, 0);
                rigidBody.AddForceAtPosition(worldManager.character.transform.up  * 5, pushPoint);
            }

            return; 
        }

        if (Input.GetKey(KeyCode.Space))
        {
            worldManager.SwitchToCharacter();
        }

        if (!touchingWater)
        {
            rigidBody.drag = .05f;
            rigidBody.angularDrag = .1f;
        } 
        else
        {
            rigidBody.drag = 1f;
            rigidBody.angularDrag = 4f;
        }
        
        
        if (touchingWater && !sinking)
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
                rigidBody.AddForce(- new Vector3(transform.forward.x, 0, transform.forward.z) * enginePower / 3.0f);
            }
        }

        Vector3 cameraTarget = transform.position - transform.forward * 10f;

        Camera.transform.position = Vector3.Lerp(Camera.transform.position, new Vector3(cameraTarget.x, cameraTarget.y + 5f, cameraTarget.z), .1f);
        Camera.transform.LookAt(transform);
    }
}