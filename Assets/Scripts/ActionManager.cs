using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*  TODO
 *  + 現在のBlockActionsはそのActionがBlockするActionを設定するが，
 *    その逆のActionがどのActionにBlockされるかを設定するように変更する
 */

/*  Info
 *  + 現在Listで書かれているものの一部はhashsetに変更する可能性がある
 *  + 処理速度に問題があれば(Add|Remove)ActionsでSortActionsを呼ぶのをやめ，
 *    actionConfigsを変更せずに直接orderedActionsを設定するように変更する可能性がある
 */

public class ActionManager : MonoBehaviour {

    /*  各Actionの設定のリスト
     *  + InspectorやAddActionsメソッドによって設定される
     */
    public List<ActionConfig> actionConfigs;

    /*  順序化されたActionConfig
     *  + ActionConfigのorderをDictionaryのKeyとし，
     *    同じorderのものはリストにまとめられる
     *  + actionConfigsを変更した際に更新される
     */
    protected SortedDictionary<int, List<ActionConfig>> orderedActions
        = new SortedDictionary<int, List<ActionConfig>>();

    /*  現在実行中のActionConfigのリスト
     *  + ActionConfigはAction.Actを呼ぶ際にdoingActionsに追加される
     *  + doingActions内のActionConfigはFixedUpdateのはじめにAction.IsDoneが
     *    呼ばれ，trueだった場合はリストから削除される
     */
    protected List<ActionConfig> doingActions = new List<ActionConfig>();

    // 仕様変更予定
    protected List<System.Type> blockActionTypes = new List<System.Type>();

    /*
     *  + actionConfigsの初期化処理
     *  + orderedActionsの更新
     */
    protected virtual void Start () {
        foreach (ActionConfig action in actionConfigs) {
            action.Init(this);
        }
        SortActions();
    }

    /*  orderedActionsを更新するメソッド
     *  + actionConfigsを参照し，orderをKeyとして辞書化してorderedActionsに入れる
     */
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

    /*  ActionConfigを追加するためのメソッド
     *  + 追加されるActionConfigを初期化
     *  + 追加後はorderedActionsを更新する
     */
    public virtual void AddActions (ActionConfig[] actions) {
        foreach (ActionConfig action in actions) {
            action.Init(this);
        }
        actionConfigs.AddRange(actions);
        SortActions();
    }

    /*  ActionConfigを追加するためのメソッド
     *  + 引数は削除されるactionConfigを指定する
     *  + 削除後はorderedActionsを更新する
     */
    public virtual void RemoveActions (ActionConfig[] actions) {
        actionConfigs.RemoveAll(action => actions.Contains(action));
        SortActions();
    }

    /*  渡されたActionConfigのリストから実行不可のものを取り除くメソッド
     *  + 以下のものを取り除く
     *    - Action.enabled == false なもの
     *    - BlockActionsに指定されているもの（変更予定）
     *    - ActionConfig.IsAvailable == false なもの
     *  + TODO
     *    - コードが汚いためLinqなどで書き直す
     *    - BlockActionsの仕様変更
     */
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

    // リストからActionConfig.weightの重みで確率的にActionConfigを選択するメソッド
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

    /*  ActionConfigのActionを実行するためのメソッド
     *  + doingActionsとblockActionTypesに追加する（仕様変更予定）
     */
    protected virtual void DoAction (ActionConfig action) {
        action.Act();
        if (!doingActions.Contains(action)) {
            doingActions.Add(action);
            blockActionTypes.AddRange(action.blockActionTypes);
        }
    }

    /*  doingActionsとblockActionTypesの更新を行うためのメソッド
     *  + 各doingActionsのAction.IsDoneを調べて，trueなら削除する
     *  + 各doingActionsによってblockActionTypesを更新する（仕様変更予定）
     */
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

    /*
     *  1. doingActionsとblockActionsの更新
     *  2. 各orderのorderedActionsについて，order順に以下を実行
     *     1) 実行不可なActionConfigを除去
     *     2) 残ったActionConfigが複数あれば確率的に選択
     *     3) ActionConfigのActionを実行
     */
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
