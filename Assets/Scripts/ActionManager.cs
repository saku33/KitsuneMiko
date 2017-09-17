using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {
    protected List<Action> doingActions;
    protected List<Action> blockedActions;

    protected static SortedDictionary<int, List<Action>> SortActions (Action[] actions) {
        SortedDictionary<int, List<Action>> orderedActions
            = new SortedDictionary<int, List<Action>>();
        foreach (Action action in actions) {
            int order = action.order;
            if (orderedActions.ContainsKey(order)) {
                orderedActions[order].Add(action);
            } else {
                orderedActions.Add(order, new List<Action> {action});
            }
        }
        return orderedActions;
    }

    protected virtual void RemoveNeedless (List<Action> actions) {
        for (int i = actions.Count - 1; i >= 0; i--) {
            if (blockedActions.Contains(actions[i]) || !actions[i].IsRequired()) {
                actions.RemoveAt(i);
            }
        }
    }

    protected virtual Action SelectRandom (List<Action> actions) {
        Action selectedAction = actions[actions.Count - 1];
        int totalWeight = 0;
        foreach (Action action in actions) {
            totalWeight += action.weight;
        }
        float rnd = Random.value * totalWeight;
        foreach (Action action in actions) {
            rnd -= action.weight;
            if (rnd <= 0) {
                selectedAction = action;
            }
        }
        return selectedAction;
    }

    protected virtual void DoAction (Action action) {
        action.Act();
        if (!doingActions.Contains(action)) {
            doingActions.Add(action);
            blockedActions.AddRange(action.blockedActions);
        }
    }

    protected virtual void UpdateState () {
        blockedActions = new List<Action>();
        for (int i = doingActions.Count - 1; i >= 0; i--) {
            if (doingActions[i].IsDone()) {
                doingActions.RemoveAt(i);
            } else {
                blockedActions.AddRange(doingActions[i].blockedActions);
            }
        }
    }

    protected virtual void FixedUpdate () {
        UpdateState();
        foreach (List<Action> actions in SortActions(GetComponents<Action>()).Values) {
            RemoveNeedless(actions);
            switch (actions.Count) {
                case 0:
                    break;
                case 1:
                    DoAction(actions[0]);
                    break;
                default:
                    DoAction(SelectRandom(actions));
                    break;
            }
        }
    }
}
