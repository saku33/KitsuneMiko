using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : MonoBehaviour {
    public string conditionName;

    public abstract ConditionState Check ();
}

public struct ConditionState {
    public bool isSatisfied;
    public Dictionary<string, object> args;
}