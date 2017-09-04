using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public GameObject player;//プレイヤーオブジェクト
    public GameObject outFrameLeft;
    public GameObject outFrameRight;

    private GameObject gameManager;
    private Camera camera;
    private const float FRAME_HALF_WIDTH = 0.5f;
    Vector3 p1,p2,cameraPos;
    private float distans;
	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager");
        camera = gameObject.GetComponent<Camera>();
        cameraPos = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        camera.transform.position = cameraPos;//初期カメラ位置をプレイヤー座標に設定
        p1 = camera.ViewportToWorldPoint(new Vector3(0, 0.5f, camera.nearClipPlane));//ビューポイント座標をワールド座標に変更
        
        distans = player.transform.position.x-p1.x;//プレイヤー座標と画面端の距離を取得
    }
	
	// Update is called once per frame
	void Update () {
        if (gameManager.GetComponent<GameManager>().gameMode == GameManager.GAME_MODE.PLAY)
        {
            cameraPos.x = player.transform.position.x;//デフォルトではプレイヤー座標に合わせてカメラ座標を更新

            if ((player.transform.position.x - distans) <
                (outFrameLeft.transform.position.x + FRAME_HALF_WIDTH))//プレイヤーが左端に位置する場合
            {
                cameraPos.x = outFrameLeft.transform.position.x + FRAME_HALF_WIDTH + distans;//カメラを左枠から離れた位置に固定　
            }
            else if ((outFrameRight.transform.position.x - FRAME_HALF_WIDTH) <
               (player.transform.position.x + distans))//プレイヤーが右端に位置する場合
            {
                cameraPos.x = outFrameRight.transform.position.x - FRAME_HALF_WIDTH - distans;//カメラを右枠から離れた位置に固定
            }
        }
        else
        {
            //何もしない
        }


        camera.transform.position = cameraPos;//カメラ座標の更新
	}
}
