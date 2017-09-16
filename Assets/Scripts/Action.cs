using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour {
    public readonly string actionName;
    public readonly int order = 1;
    public readonly bool not = false;
    public readonly Condition condition;
    public readonly int weight = 1;
    public readonly List<Action> blockedActions = new List<Action>();

    public virtual bool IsRequired () {
        return not ? !condition.check() : condition.check();
    }

    public abstract bool IsDone ();
    public abstract void Act ();
}
