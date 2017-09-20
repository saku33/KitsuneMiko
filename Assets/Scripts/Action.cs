using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Action : MonoBehaviour {
    public string actionName;

    public abstract bool IsDone ();
    public abstract void Act (Dictionary<string, object> args);
}
