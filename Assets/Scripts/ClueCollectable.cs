using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueCollectable : Collectable
{
   protected override void Collect()
    {
        worldManager.FindClue();
    }   
}
