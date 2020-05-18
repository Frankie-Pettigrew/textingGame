using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;

public class typingCues : MonoBehaviour
{
    private List<typingBeat> bubbles = new List<typingBeat>();
    public typingBeat beatFab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createCues(int length,float[] times)
    {
        if(length < 4)
        {
            return;
        }

        for(int i = 0; i < length; i++)
        {
            typingBeat newBeat = Instantiate(beatFab);
            newBeat.activeTime = times[i];
            bubbles.Add(newBeat);
        }
    }

    public void runCues()
    {
        foreach(typingBeat beat in bubbles)
        {

        }
    }
}
