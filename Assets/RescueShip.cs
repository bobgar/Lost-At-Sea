using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueShip : MonoBehaviour
{
    public float interactDistance = 10f;
    public WorldManager worldManager;
    // Update is called once per frame
    void Update()
    {
        if (worldManager.character != null && (worldManager.character.transform.position - transform.position).magnitude < interactDistance
            || worldManager.boat.attached && (worldManager.boat.transform.position - transform.position).magnitude < interactDistance)
        {
            worldManager.Win();
        }
    }
}
