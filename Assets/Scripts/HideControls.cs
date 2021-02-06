using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HideControls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float timestamp = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.H) && Time.timeSinceLevelLoad - timestamp > .3f )
        {
            timestamp = Time.timeSinceLevelLoad;
            TextMeshProUGUI tmp = this.GetComponent<TextMeshProUGUI>();
            tmp.enabled = !tmp.enabled;
        }
    }
}
