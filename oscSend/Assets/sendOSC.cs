using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sendOSC : MonoBehaviour
{
    public string address = "/robotState";
    public OSC osc;
    [Range(1,5)]
    public int robotState = 1;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    OscMessage message = new OscMessage();
    message.address = address;
    message.values.Add(robotState);
    osc.Send(message);

        
    }
}
