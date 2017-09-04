using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour {

	public GameObject[] stageButtons;	//ステージ選択ボタン配列

	// Use this for initialization
	void Start () {
		int clearStageNo = PlayerPrefs.GetInt ("CLEAR", 0);		//どのステージまでクリアしているのかをロード（セーブされていなければ「０」）

		//ステージボタンを有効化
		for (int i = 0; i <= stageButtons.GetUpperBound(0); i++) {
			bool buttonEnable;

			if (clearStageNo < i) {
				buttonEnable = false;	//前ステージをクリアしていなければ無効
			}else{
				buttonEnable = true;	//前ステージをクリアしていれば有効
			}

			stageButtons[i].GetComponent<Button>().interactable = buttonEnable;	//ボタンの有効/無効を設定
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//ステージ選択ボタンを押した
	public void PushStageSelectButton (int stageNo) {
		SceneManager.LoadScene ("GameScene" + stageNo);	//ゲームシーンへ
	}
}
