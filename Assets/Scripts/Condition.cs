using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  キャラクターの行動制御のための条件を定義するための抽象クラス
 *  + 実装はイベント駆動風
 */
public abstract class Condition : MonoBehaviour {

    /*  同一コンポーネントの識別に使用するためのCondition名
     *  + 主にInspectorで使用するため，継承の派生クラスで設定する必要はない
     */
    public string conditionName;

    /*  Conditionを満たした際にイベントを発火するためのメソッド
     *  + ActionManagerは必要であればこのメソッドで
     *    イベントが発火しているか確認する
     */
    public abstract ConditionState Check (object[] args);
}

/*  Condition.Checkメソッドの戻り値
 *  + isSatisfied: 条件を満たしているか
 *  + args: Action.Actに引数として渡す情報
 */
public class ConditionState {
    public bool isSatisfied = false;
    public Dictionary<string, object> args = new Dictionary<string, object>();

    public ConditionState (
            bool isSatisfied = false,
            Dictionary<string, object> args = null
        ) {
        this.isSatisfied = isSatisfied;
        if (args != null) {
            this.args = args;
        }
    }
}