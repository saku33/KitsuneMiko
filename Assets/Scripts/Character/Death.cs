using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {
    protected virtual void OnEnable () {
        GetComponent<Breakable>().enabled = false;
        ActionManager actionManager = GetComponent<ActionManager>();
        if (actionManager != null) {
            actionManager.enabled = false;
        }
    }

    protected virtual void FixedUpdate () {
        Destroy(gameObject);
    }
}
