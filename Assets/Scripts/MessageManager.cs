using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public float fadeDuration = 2f;

    [Serializable]
    public struct NamedMessage
    {
        public string name;
        public GameObject message;
    }
    public NamedMessage[] messages;

    private Dictionary<string, GameObject> messageDictionary = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < messages.Length; i++)
        {
            messages[i].message.SetActive(false);
            messageDictionary.Add(messages[i].name, messages[i].message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(activeMessage != null)
        {
            float p = ( stopTime - Time.timeSinceLevelLoad) / fadeDuration;            
            activeMessage.GetComponent<TextMeshProUGUI>().alpha = p;

            if( p <= 0)
            {
                activeMessage.SetActive(false);
                activeMessage = null;
            }
        }       
    }

    private GameObject activeMessage;
    private float stopTime = 0;
    public void ShowMessage(string key)
    {
        if(!messageDictionary.ContainsKey(key) || activeMessage == messageDictionary[key])
        {
            return;
        }
        activeMessage = messageDictionary[key];        
        activeMessage.SetActive(true);
        stopTime = Time.timeSinceLevelLoad + fadeDuration;
    }
}
