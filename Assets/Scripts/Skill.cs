using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {
    public float cost;

    bool _disableByAwake = true;
    protected bool disableByAwake {
        get {
            if (_disableByAwake) {
                _disableByAwake = false;
                return true;
            } else {
                return false;
            }
        }
    }

    protected virtual void Awake () {
        enabled = false;
    }

    protected abstract void OnEnable ();
    protected abstract void OnDisable ();
}
