using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;

public class TypeBeat : MonoBehaviour
{

    public double OkWindowStart;
    public double GoodWindowStart, PerfectWindowStart, PerfectWindowEnd, GoodWindowEnd, OkWindowEnd;
    public double crossingTime;

    public enum CueState { Early = 0, OK = 1, Good = 2, Perfect = 3, Late = 4 }
    public CueState typeCue;
    public BeatMapEvent bmEvent;
    private bool crossed;

    private void Start()
    {
        crossingTime = bmEvent.eventMBT.GetMilliseconds();
        typeCue = CueState.Early;
        crossed = false;
    }

    private void Update()
    {
        UpdateWindow();
        if (Clock.Instance.TimeMS >= crossingTime && !crossed)
        {
            Debug.Log("should cross now");
            crossed = true;
        }
    }


    public void UpdateWindow()
    {
        //check our cue state against the Clock Script

        //for this case (more-or-less typical japanese rhythm game style), our detection windows are 
        // early - ok - good - perfect - good - ok - late

        switch (typeCue)
        {

            case CueState.Early:
                //check to see if we've gotten to "ok"
                if (Clock.Instance.TimeMS > OkWindowStart)
                {
                    typeCue = CueState.OK;
                }
                break;
            case CueState.OK:
                //check to see if we've gotten to "good"...
                if (Clock.Instance.TimeMS > GoodWindowStart && Clock.Instance.TimeMS < PerfectWindowStart)
                {
                    typeCue = CueState.Good;

                }
                //... or maybe we're at the end of the last "ok" window
                else if (Clock.Instance.TimeMS > OkWindowEnd)
                {
                    typeCue = CueState.Late;

                }
                break;
            case CueState.Good:
                //check to see if we've gotten to "perfect"
                if (Clock.Instance.TimeMS > PerfectWindowStart && Clock.Instance.TimeMS < PerfectWindowEnd)
                {
                    typeCue = CueState.Perfect;
                }
                //
                else if (Clock.Instance.TimeMS > GoodWindowEnd)
                {
                    typeCue = CueState.OK;
                }
                break;
            case CueState.Perfect:
                if (Clock.Instance.TimeMS > PerfectWindowEnd)
                {
                    typeCue = CueState.Good;
                }
                break;
            default:
                //if we're "late" there are no more potential state changes
                break;


        }
    }
}
