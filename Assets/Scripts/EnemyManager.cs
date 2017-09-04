using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EnemyManager : MonoBehaviour {
    private const int ENEMY_POINT = 50;//敵の得点
    private GameObject gameManager;//ゲームマネージャー

    public LayerMask blockLayer;

    private Rigidbody2D rbody;

    private float moveSpeed = 1;

    public enum MOVE_DIR
    {
        LEFT,
        RIGHT,
    }

    private MOVE_DIR moveDirection = MOVE_DIR.LEFT;//移動方法

	// Use this for initialization
	void Start () {
        rbody = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager");

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        bool isBlock;

        switch (moveDirection)
        {
            case MOVE_DIR.LEFT:
                rbody.velocity = new Vector2(moveSpeed * -1, rbody.velocity.y);
                transform.localScale = new Vector2(1, 1);

                isBlock = Physics2D.Linecast(
                    new Vector2(transform.position.x, transform.position.y + 0.5f),
                    new Vector2(transform.position.x - 0.3f, transform.position.y + 0.5f),
                    blockLayer);

                if (isBlock)
                {
                    moveDirection = MOVE_DIR.RIGHT;
                }
                break;
            case MOVE_DIR.RIGHT:
                rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);
                transform.localScale = new Vector2(-1, 1);

                isBlock = Physics2D.Linecast(
                    new Vector2(transform.position.x, transform.position.y + 0.5f),
                    new Vector2(transform.position.x + 0.3f, transform.position.y + 0.5f),
                    blockLayer);

                if (isBlock)
                {
                    moveDirection = MOVE_DIR.LEFT;
                }
                break;
        }
    }

    public void DestroyEnemy()
    {
        gameManager.GetComponent<GameManager>().AddScore(ENEMY_POINT);

        rbody.velocity = new Vector2(0, 0);
        //コライダー削除
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Destroy(circleCollider);
        Destroy(boxCollider);
        //死亡モーション
        Sequence animSet = DOTween.Sequence();
        animSet.Append(transform.DOLocalMoveY(0.5f, 0.2f).SetRelative());
        animSet.Append(transform.DOLocalMoveY(-10.0f, 1.0f).SetRelative());

        Destroy(this.gameObject,1.2f);
    }

}
