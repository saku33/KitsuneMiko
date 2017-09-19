using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : MonoBehaviour {
    public string conditionName;

    public abstract bool Check ();
}
