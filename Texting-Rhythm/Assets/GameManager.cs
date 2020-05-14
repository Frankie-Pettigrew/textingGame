using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;

public class RhythmInput
{
    public KeyCode inputKey;
    public double inputTime;
}

public class GameManager : MonoBehaviour
{

    List<KeyCode> keyCodesRight = new List<KeyCode>() {KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.A, KeyCode.S, KeyCode.D,
    KeyCode.F,KeyCode.G,KeyCode.Z,KeyCode.X,KeyCode.C,KeyCode.V,KeyCode.B};
    List<KeyCode> keyCodesLeft = new List<KeyCode>() {KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.H, KeyCode.J, KeyCode.K,
    KeyCode.L,KeyCode.Semicolon,KeyCode.Quote,KeyCode.N,KeyCode.M,KeyCode.Comma,KeyCode.Period};

    List<string> firstMessages = new List<string>() { "Hey, how are you doing today?", "Turnip Prices?",
        "U online?","sup brah","onlyfans?" };
    List<string> normalResponse = new List<string>() { "nm hbu?",
        "thanks for reaching out, i'm actually at capacity right now", "sorry i'm playing COD",
        "i decided to 'disconnect' for a minute and really think about my life" };




    AudioHelm.HelmController controller;

    public static int lineLength = 6;
    public int numLines;

    public Clock clock;
    public GameObject messageFab;

    public GameObject EmojiFab;
    public GameObject EmojiCue;

    public int gameScore = 0;
    public List<RhythmInput> CachedInputs = new List<RhythmInput>();


    public GameObject beatFab;
    public bool beatMade;

    public List<TypeBeat> bmEvents = new List<TypeBeat>();
    public List<KeyCode> playerKeys = new List<KeyCode>() { KeyCode.Space,KeyCode.Return};

   
    public List<Sprite> emoji = new List<Sprite>();

    public List<GameObject> currentMessage = new List<GameObject>();

    public int lastMeasure;
    public BeatMapEvent nextMeasure;
    public int typingWait = 2;
    public bool isMyTurn = false;

    public List<GameObject> cpuTyping = new List<GameObject>();
    public int gameLength = 8;
    public int turnLength = 8;


    public double OkWindowMillis = 200d;
    public double GoodWindowMillis = 100d;
    public double PerfectWindowMillis = 50d;

    List<int> notes = new List<int>() {36,39,41,46,48,51,36};

    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<AudioHelm.HelmController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        //Debug.Log(clock.GetMBT());
        if (isMyTurn)
        {
            typeMessage();

        }
        else
        {
            cpuTurn();
        }

        
             
        
    }


    void CreateTurnSwaps()
    {
        for (int i = 0; i < gameLength; i++)
        {
            MBT newMeasure = new MBT(turnLength*(i+1), 0, 0);
            TypeBeat newBeat = new TypeBeat();
            newBeat.bmEvent.eventMBT = newMeasure;
            newBeat.bmEvent.inputKey = KeyCode.Return;

            bmEvents.Add(newBeat);
        }
    }

    public void cpuTurn()
    {
        if (beatMade)
        {
            checkCPUInputs();
            
        }
        else
        {
            createCPUTurn();
        }
    }

    void checkCPUInputs()
    {
        TypeBeat[] activeBeats = GameObject.FindObjectsOfType<TypeBeat>();
        bmEvents.AddRange(activeBeats);
        for (int i = 0; i < bmEvents.Count; i++)
        {
            if (bmEvents[i].GetComponent<TypeBeat>().typeCue != TypeBeat.CueState.Early)
            {
                for (int j = 0; j < CachedInputs.Count; j++)
                {
                    if (CachedInputs[j].inputKey == activeBeats[i].bmEvent.inputKey)
                    {
                        ScoreBeat(activeBeats[i]);
                    }
                }
            }
        }


        if(activeBeats.Length == 0)
        {
            isMyTurn = true;
            beatMade = false;
        }
        bmEvents.Clear();
        CachedInputs.Clear();
    }


   void createCPUTurn()
    {
        for (int i = 0; i < turnLength / 2; i++)
        {
            BeatMapEvent newEvent = new BeatMapEvent();
            MBT newMeasure = new MBT(clock.GetMBT().Measure + 2 * (i + 1), 0, 0);
            newEvent.eventMBT = newMeasure;

            TypeBeat newBeat;
            newBeat = Instantiate(beatFab).GetComponent<TypeBeat>();
            newBeat.GetComponent<TypeBeat>().bmEvent = newEvent;
            newEvent.inputKey = KeyCode.Space;

            newBeat.OkWindowStart = newBeat.bmEvent.eventMBT.GetMilliseconds() - (0.5d * OkWindowMillis);
            newBeat.OkWindowEnd = newBeat.bmEvent.eventMBT.GetMilliseconds() + (0.5d * OkWindowMillis);
            newBeat.GoodWindowStart = newBeat.bmEvent.eventMBT.GetMilliseconds() - (0.5d * GoodWindowMillis);
            newBeat.GoodWindowEnd = newBeat.bmEvent.eventMBT.GetMilliseconds() + (0.5d * GoodWindowMillis);
            newBeat.PerfectWindowStart = newBeat.bmEvent.eventMBT.GetMilliseconds() - (0.5d * PerfectWindowMillis);
            newBeat.PerfectWindowEnd = newBeat.bmEvent.eventMBT.GetMilliseconds() + (0.5d * PerfectWindowMillis);
            beatMade = true;
        }
    }

    void CheckInput()
    {
        for (int i = 0; i < playerKeys.Count; i++)
        {
            if (Input.GetKeyDown(playerKeys[i]))
            {
                RhythmInput _rhythmInput = new RhythmInput();
                _rhythmInput.inputKey = playerKeys[i];
                _rhythmInput.inputTime = Clock.Instance.TimeMS;

                CachedInputs.Add(_rhythmInput);
            }
        }
    }

    void ScoreBeat(TypeBeat beat)
    {
        switch (beat.typeCue)
        {
            case TypeBeat.CueState.OK:
                gameScore += 1;
                Debug.Log("OK!");
                Destroy(beat.gameObject);
                break;
            case TypeBeat.CueState.Good:
                gameScore += 2;
                Debug.Log("Good!");
                Destroy(beat.gameObject);
                break;
            case TypeBeat.CueState.Perfect:
                gameScore += 3;
                Debug.Log("Perfect!");
                Destroy(beat.gameObject);
                break;
            case TypeBeat.CueState.Late:
                Destroy(beat.gameObject);
                Debug.Log("Missed!");
                break;
        }


    }

    public void clearCache()
    {
        currentMessage.Clear();
    }

    public void typeMessage()
    {
        if (inputChecker() >= 0)
        {
            if (currentMessage.Count < lineLength)
            {
                controller.NoteOn(notes[Random.Range(0, 7)], 0.3f, 0.1f);
                GameObject newMoj = Instantiate(EmojiFab, EmojiCue.transform);
                newMoj.GetComponent<SpriteRenderer>().sprite = emoji[Random.Range(0, emoji.Count - 1)];
                newMoj.transform.position = new Vector3(newMoj.transform.position.x + (0.5f * currentMessage.Count), newMoj.transform.position.y, newMoj.transform.position.z);
                currentMessage.Add(newMoj);
            }
        }



        if (Input.GetKeyDown(KeyCode.Return))
        {
            foreach (GameObject obj in currentMessage)
            {
                obj.SetActive(false);
            }
            createMessage();
            currentMessage.Clear();
            Debug.Log("Turn passed to CPU");
            isMyTurn = false;
        }
    }


    int inputChecker()
    {
        for (int i = 0; i < keyCodesLeft.Count; i++)
        {
            if (Input.GetKeyDown(keyCodesRight[i]))
            {
                return i;

            }
            if (Input.GetKeyDown(keyCodesLeft[i]))
            {
                return i;
            }
        }

         return -1;
        
    }


    public void createMessage()
    {
        
            GameObject newMess;
            newMess = Instantiate(messageFab);
            newMess.GetComponent<Messages>().messageSize = numLines;
    }
}
