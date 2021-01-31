using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int collectibleValue = 50;
    public WorldManager worldManager;
    bool hasBeenCollected = false;

    void Update()
    {
        if (hasBeenCollected)
        {
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (worldManager.character != null && (worldManager.character.transform.position - transform.position).magnitude < 4f)
            {
                hasBeenCollected = true;
                Collect();
            }
        }
    }

    protected virtual void Collect()
    {
        //worldManager.UpdateHealth(collectibleValue);
        //worldManager.messageManager.ShowMessage("food");
    }
}
