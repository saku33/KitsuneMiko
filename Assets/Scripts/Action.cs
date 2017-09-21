using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラクターの行動を定義するための抽象クラス
    // * 実装はイベント駆動風となっている
public abstract class Action : MonoBehaviour {

    // 同一コンポーネントの識別に使用するためのAction名
        // * 主にInspectorで設定するため，継承の派生クラスで設定する必要はない
    public string actionName;

    // ActionManagerにActionの終了イベントを知らせるためのメソッド
        // * Actが呼ばれた後，IsDoneがtrueを返すまでFixedUpdate毎に呼ばれる
        // * ActionManagerのBlockの判定に使われる
    public abstract bool IsDone ();

    // Conditionのイベントに対して，行動の開始や変化を行うためのイベントハンドラ
        // * argsにはActionManagerに設定されたConditionの
        //   Condition.checkの戻り値の和集合が渡される
        // * 処理が1フレームで終わらない場合はFixedUpdateで行う
    public abstract void Act (Dictionary<string, object> args);
}
