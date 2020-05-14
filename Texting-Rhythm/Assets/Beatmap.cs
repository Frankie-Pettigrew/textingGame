﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;

/// <summary>
/// Data holder for our Beat Map events - basically the things that make up our level
/// In this case, we're doing a falling gems interface similar to guitar hero or DDR
/// </summary>
[System.Serializable]
public class BeatMapEvent
{
    //a container for our Measure-Beat-Tick settings for our beatmap events
    //we will be editing these when making our beatmaps
    public MBT eventMBT;

    //these need to be public because other events will change them, but we don't want to mess with them in the inspector
    [HideInInspector] public bool cueCalled = false;
    [HideInInspector] public bool cueActive = false;

    //Using Keyboard for now, eventually we'll use the input manager;
    [Tooltip("Make sure this matches one of your PlayerInputKeys!")]
    public KeyCode inputKey;


    //this will be assigned at runtime;
    [HideInInspector] public double cueTime;
    private GameObject _cueObject;

}

/// <summary>
/// This class manages the different events in the beatmap.  Right now it's designed to go sequentially:
/// once the cueTime is reached, it instantiates the cue 
/// </summary>
public class Beatmap : MonoBehaviour
{
    [Header("Our Beatmap Level")]
    public List<BeatMapEvent> beatEvents = new List<BeatMapEvent>();

    //the "cue" here can be a number of things
    //for now it's just the spawn time offset (in number of beats)
    //right now, this assumes that each beatEvent will have the same cue offset.
    //if you don't want this to be the case, have a seperate beatmap/input evaluator pair for these other events
    //(example - rhythm heaven has varying cue lengths)
    [Header("Cue Offset in Beats")]
    public int cueBeatOffset;

    //Make sure the OkWindow > GoodWindow > PerfectWindow!!!  Also make sure that you don't have successive beatmap at shorter timespans than your OkWindow
    [Header("Window Sizes in MS")]
    public double OkWindowMillis = 200d;
    public double GoodWindowMillis = 100d;
    public double PerfectWindowMillis = 50d;


    int beatEventIndex = 0;
    int cueIndex = 0;

    //these were for debugging
    double nextCueTime;
    double nextBeatEventTime;
    bool cueEndReached = false;



    //These should all be the same length, and should correspond to the different inputs, starting locations, and prefabs
    public KeyCode[] playerInputKeys;
    public GameObject[] cueStartLocations;
    public GameObject[] cuePrefabs;

    bool levelEndReached = false;

    //this is for loading data from Midi files
    //public SongSource songSource;

    //private Song currentSong;

    //this should match the name of your song in your Resources folder
    //public string songName;

    private void Awake()
    {
        //PrepareData(songName);
    }

    void Start()
    {

        for (int i = 0; i < beatEvents.Count; i++)
        {
            //set the cue times for each beat event            
            beatEvents[i].cueTime = beatEvents[i].eventMBT.GetMilliseconds() - (cueBeatOffset * Clock.Instance.BeatLengthD() * 1000d);

        }

        beatEventIndex = 0;


        levelEndReached = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (beatEventIndex >= beatEvents.Count)
        {
            levelEndReached = true;
        }

        if (!levelEndReached)
        {
            //Debug.Log("Event Millis in Update: " + beatEvents[beatEventIndex].eventMBT.GetMilliseconds());
            if (Clock.Instance.TimeMS >= beatEvents[beatEventIndex].cueTime)
            {

                CreateCue();
                //Add something visueal here - juice particles, maybe start your animations, 
                Debug.Log("fire cue");


                beatEventIndex++;

            }
        }



    }

    public void Reset()
    {
        cueIndex = 0;
        beatEventIndex = 0;
    }

    public void CreateCue()
    {

        Debug.Log("create cue");
        //determining which cue lane we're in (which cue type we're using)
        int cueLaneIndex = int.MaxValue;
        for (int i = 0; i < playerInputKeys.Length; i++)
        {
            if (playerInputKeys[i] == beatEvents[beatEventIndex].inputKey)
            {
                cueLaneIndex = i;
            }
        }

        if (cueLaneIndex == int.MaxValue)
        {
            Debug.LogWarning("beatmap input key doesn't match current inputs!");
        }
        GameObject newCue = Instantiate(cuePrefabs[cueLaneIndex], cueStartLocations[cueLaneIndex].transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.bmEvent = beatEvents[beatEventIndex];

        //Set Window Timings
        fallingGem.OkWindowStart = fallingGem.bmEvent.eventMBT.GetMilliseconds() - (0.5d * OkWindowMillis);
        fallingGem.OkWindowEnd = fallingGem.bmEvent.eventMBT.GetMilliseconds() + (0.5d * OkWindowMillis);
        fallingGem.GoodWindowStart = fallingGem.bmEvent.eventMBT.GetMilliseconds() - (0.5d * GoodWindowMillis);
        fallingGem.GoodWindowEnd = fallingGem.bmEvent.eventMBT.GetMilliseconds() + (0.5d * GoodWindowMillis);
        fallingGem.PerfectWindowStart = fallingGem.bmEvent.eventMBT.GetMilliseconds() - (0.5d * PerfectWindowMillis);
        fallingGem.PerfectWindowEnd = fallingGem.bmEvent.eventMBT.GetMilliseconds() + (0.5d * PerfectWindowMillis);

    }

    //this makes our beatmap from the json file
    //void PrepareData(string name)
    //{
    //    currentSong = SongSource.getSong(name);
    //    if (currentSong.tracks.Length > 0)
    //    {

    //        for (int i = 0; i < currentSong.tracks.Length; i++)
    //        {
    //            //go through each track first
    //            for (int j = 0; j < currentSong.tracks[i].notes.Length; j++)
    //            {

    //                //this will convert the note time from seconds to beats
    //                currentSong.tracks[i].notes[j].beatTime = currentSong.tracks[i].notes[j].time * currentSong.header.bpm / 60f;
    //                //assuming 4/4 time for now
    //                int measure = Mathf.FloorToInt(currentSong.tracks[i].notes[j].beatTime / 4f);
    //                int beat = Mathf.FloorToInt(currentSong.tracks[i].notes[j].beatTime - (measure * 4));
    //                int tick = Mathf.FloorToInt((currentSong.tracks[i].notes[j].beatTime - (measure * 4) - beat) * 96);
    //                currentSong.tracks[i].notes[j].mbtValue = new MBT(measure, beat, tick);

    //                //assign this midi note to a beatmap event
    //                BeatMapEvent bmEvent = new BeatMapEvent();
    //                bmEvent.eventMBT = currentSong.tracks[i].notes[j].mbtValue;
    //                bmEvent.eventMBT.Measure += 1;
    //                bmEvent.eventMBT.Beat += 1;
    //                bmEvent.eventMBT.Tick += 1;

    //                //depending on which note it is, assign the prefab accordingly
    //                switch (currentSong.tracks[i].notes[j].name)
    //                {
    //                    case "C1":
    //                        bmEvent.inputKey = KeyCode.R;
    //                        break;
    //                    case "C#1":
    //                        bmEvent.inputKey = KeyCode.G;
    //                        break;
    //                    case "D1":
    //                        bmEvent.inputKey = KeyCode.B;
    //                        break;
    //                }

    //                beatEvents.Add(bmEvent);

    //            }
    //        }
    //    }


    //}




}
