using System.Collections.Generic;
using Beat;
using UnityEngine;

public class typingAnim : MonoBehaviour
{
    public Clock clock; 
    public GameManager manager; 
    public List<Sprite> sprites = new List<Sprite>();
    public int thisSpr = 0;
    SpriteRenderer sp;
    public bool frameChanged = false;
    public double nextChange;
    double prevChange;

    double prevDSPTime;
    Sprite lastSpr;

    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        clock = FindObjectOfType<Clock>();
        manager = FindObjectOfType<GameManager>();
        nextChange = Clock.Instance.AtNextMeasure();
    }

    // Update is called once per frame
    void Update()
    {




        // check that we're past the next quantized time
        if (Clock.Instance.TimeMS >= nextChange)
        {

            prevChange = nextChange;
            if (AudioSettings.dspTime == prevDSPTime)
            {
                Debug.Log("duplicate dsp time");
                return;
            }

            else prevDSPTime = AudioSettings.dspTime;
            
            //check that we haven't held the dspTime over multiple updates
            
           
            nextChange = Clock.Instance.AtNextHalf();

            if (nextChange == prevChange)
            {
                Debug.Log("duplicate beat time");
                return;
            }
            
            nextFrame();
            


        }
    }

    public void nextFrame()
    {
        if(thisSpr >= sprites.Count-1)
        {
            thisSpr = 0;
        } else
        {
            thisSpr++;
        }
        lastSpr = sp.sprite;
        sp.sprite = sprites[thisSpr];
        Debug.Log("Running");
        
        


    }
}
