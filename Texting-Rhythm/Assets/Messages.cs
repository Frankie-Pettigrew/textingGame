using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;

public class Messages : MonoBehaviour
{
    public string thisMessage;
    public int messageSize;

    public double thisCueTime;

    public Sprite[] sprites;
    public int thisSprite;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
