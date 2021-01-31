using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectable : Collectable
{
    // Update is called once per frame
    protected override void Collect()
    {
        worldManager.UpdateHealth(collectibleValue);
        worldManager.messageManager.ShowMessage("food");
    }
}
