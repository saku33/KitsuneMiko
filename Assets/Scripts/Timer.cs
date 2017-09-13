using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

	private float integratedTime;
	private bool isTimerWorking;

	public float ElapsedTime
	{
		internal get
		{
			return integratedTime;
		}
		set
		{
			this.integratedTime = value;
		}
	}

	// Use this for initialization
	void Start () {
		integratedTime = 0f;
		isTimerWorking = false;
	}
	
	public void Begin(bool startOver = true)
	{
		if (startOver)
        {
            integratedTime = 0f;
            isTimerWorking = true;
        }
		else
		{
            isTimerWorking = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if (isTimerWorking)
		{
			integratedTime += Time.deltaTime;
		}
	}

	public void Stop()
	{
		isTimerWorking = false;
	}
}
