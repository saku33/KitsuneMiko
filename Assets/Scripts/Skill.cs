using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {
    public float cost;

    protected virtual void Awake () {
        enabled = false;
    }

    protected abstract void OnEnable ();
    protected abstract void OnDisable ();
}
