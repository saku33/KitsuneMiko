using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour {
    public List<ActionConfig> actionConfigs;

    protected List<ActionConfig> doingActions = new List<ActionConfig>();
    protected List<System.Type> blockActionTypes = new List<System.Type>();
    protected SortedDictionary<int, List<ActionConfig>> orderedActions
        = new SortedDictionary<int, List<ActionConfig>>();

    protected virtual void Start () {
        foreach (ActionConfig action in actionConfigs) {
            action.Init(this);
        }
        SortActions();
    }

    protected virtual void SortActions () {
        orderedActions.Clear();
        foreach (ActionConfig action in actionConfigs) {
            int order = action.order;
            if (orderedActions.ContainsKey(order)) {
                orderedActions[order].Add(action);
            } else {
                orderedActions.Add(order, new List<ActionConfig> {action});
            }
        }
    }

    public virtual void AddActions (ActionConfig[] actions) {
        foreach (ActionConfig action in actions) {
            action.Init(this);
        }
        actionConfigs.AddRange(actions);
        SortActions();
    }

    public virtual void RemoveActions (ActionConfig[] actions) {
        actionConfigs.RemoveAll(action => actions.Contains(action));
        SortActions();
    }

    protected virtual List<ActionConfig> RemoveNeedless (List<ActionConfig> actions) {
        List<ActionConfig> availableActions = new List<ActionConfig>();
        foreach (ActionConfig action in actions) {
            System.Type actionType = action.action.GetType();
            if (!blockActionTypes.Any(
                    type => actionType == type || actionType.IsSubclassOf(type))
                && action.IsAvailable()
            ) {
                availableActions.Add(action);
            }
        }
        return availableActions;
    }

    protected virtual ActionConfig SelectRandom (List<ActionConfig> actions) {
        ActionConfig selectedAction = actions[actions.Count - 1];
        int totalWeight = 0;
        foreach (ActionConfig action in actions) {
            totalWeight += action.weight;
        }
        float rnd = Random.value * totalWeight;
        foreach (ActionConfig action in actions) {
            rnd -= action.weight;
            if (rnd <= 0) {
                selectedAction = action;
            }
        }
        return selectedAction;
    }

    protected virtual void DoAction (ActionConfig action) {
        action.Act();
        if (!doingActions.Contains(action)) {
            doingActions.Add(action);
            blockActionTypes.AddRange(action.blockActionTypes);
        }
    }

    protected virtual void UpdateBlock () {
        blockActionTypes.Clear();
        for (int i = doingActions.Count - 1; i >= 0; i--) {
            if (doingActions[i].action.IsDone()) {
                doingActions.RemoveAt(i);
            } else {
                blockActionTypes.AddRange(doingActions[i].blockActionTypes);
            }
        }
    }

    protected virtual void FixedUpdate () {
        UpdateBlock();
        foreach (List<ActionConfig> actions in orderedActions.Values) {
            List<ActionConfig> availableActions = RemoveNeedless(actions);
            switch (availableActions.Count) {
                case 0:
                    break;
                case 1:
                    DoAction(availableActions[0]);
                    break;
                default:
                    DoAction(SelectRandom(availableActions));
                    break;
            }
        }
    }
}

[System.Serializable]
public class ActionConfig {
    [System.Serializable]
    public class ConditionConfig {
        public bool not = false;
        public string conditionName;

        [System.NonSerialized]
        public Condition condition;

        public ConditionConfig (bool not, string conditionName) {
            this.not = not;
            this.conditionName = conditionName;
        }

        public virtual void Init (ActionManager manager) {
            condition = manager.GetComponents<Condition>().First(
                elm => elm.conditionName == conditionName);
        }
    }

    public string actionName;
    public int order = 1;
    public ConditionConfig[] conditions;
    public int weight = 1;
    public string[] blockActions;

    [System.NonSerialized]
    public Action action;
    [System.NonSerialized]
    public System.Type[] blockActionTypes;

    protected Dictionary<string, object> args = new Dictionary<string, object>();

    public ActionConfig (
            string actionName, int order, int weight,
            Dictionary<string, bool> conditions, string[] blockActions
        ) {
        this.actionName = actionName;
        this.order = order;
        this.weight = weight;
        this.blockActions = blockActions;

        int idx = 0;
        int len = conditions.Count;
        this.conditions = new ConditionConfig[len];
        foreach (KeyValuePair<string, bool> condition in conditions) {
            this.conditions[idx] = new ConditionConfig(condition.Value, condition.Key);
            idx += 1;
        }
    }

    public virtual void Init (ActionManager manager) {
        action = manager.GetComponents<Action>().First(
            elm => elm.actionName == actionName);

        foreach (ConditionConfig condition in conditions) {
            condition.Init(manager);
        }

        int len = blockActions.Length;
        blockActionTypes = new System.Type[len];
        for (int i = 0; i < len; i++) {
            blockActionTypes[i] = System.Type.GetType(blockActions[i]);
        }
    }

    public virtual bool IsAvailable () {
        args.Clear();
        ConditionState state;
        foreach (ConditionConfig condition in conditions) {
            state = condition.condition.Check();
            if (condition.not ? state.isSatisfied : !state.isSatisfied) {
                return false;
            }
            args = args.Union(state.args).ToDictionary(elm => elm.Key, elm => elm.Value);
        }
        return true;
    }

    public virtual void Act () {
        action.Act(args);
    }
}
