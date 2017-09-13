using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイマークラス。正確な時間は取得できないのでフレーム数取得を推奨。
/// </summary>
public class Timer : MonoBehaviour
{

    private float integratedTime;
    private uint integratedFrames;
    private bool isTimerWorking;

    /// <summary>
    /// 経過時間
    /// </summary>
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

    /// <summary>
    /// 経過フレーム数
    /// </summary>
    public uint Frames
    {
        internal get
        {
            return integratedFrames;
        }
        set
        {
            this.integratedFrames = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        integratedTime = 0f;
        isTimerWorking = false;
    }

    /// <summary>
    /// タイマーの開始
    /// <param name="startOver">経過時間をリセットして開始するかどうか。初期値はtrue</param>
    /// </summary>
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
    // frame doesn't refresh itself constantly
    // which causes trouble in increasing seconds
    void Update()
    {
        if (isTimerWorking)
        {
            integratedTime += Time.deltaTime;
            integratedFrames += 1;
        }
    }

    /// <summary>
    /// タイマーの(一時)停止
    /// </summary>
    public void Stop()
    {
        isTimerWorking = false;
    }
}
