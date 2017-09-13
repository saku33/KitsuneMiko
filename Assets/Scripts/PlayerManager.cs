using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    public GameObject gameManager;

    public LayerMask blockLayer;//ブロックレイヤー

    private Rigidbody2D rbody;//プレイヤー制御用Rigidbody2D

    private const float MOVE_SPEED = 3;//移動速度固定値
    private float moveSpeed;//移動速度

    private float jumpPowerTime = 0;//分岐時に使用するジャンプ力時間
    private float tmpJumpPowerTime = 0;//加算中のジャンプ力時間
    private float jumpPower = 400;//ジャンプ力
    private bool goJump = false;//ジャンプしたか否か
    private bool canJump = false;//ブロックに設置しているか否か
    private int stateJump = 0;
    //0.ジャンプしてない
    //1.ジャンプした瞬間
    //2.ジャンプ中かつジャンプボタンを押したまま
    //3.ジャンプ中かつジャンプボタンを離した
    private int jumpCount = 0;//ジャンプボタンを押下したときに増加するカウント（stateJumpと連動）

    private bool onAttackKey = false;//攻撃キーを押したか否か
    private int stateAttack = 0;
    //0.攻撃のキー受付状態
    //1.攻撃キーを押した瞬間
    //2.初撃時間中に攻撃キーを離した
    //3.2番目の攻撃時間中に攻撃キーをまだ押していない
    //4.2番目の攻撃時間中に攻撃キーを押した瞬間
    //5.2番目の攻撃時間中に攻撃キーを離した
    //6.3番目の攻撃時間中に攻撃キーをまだ押していない
    //7.3番目の攻撃時間中に攻撃キーを押した瞬間
    //8.3番目の攻撃時間中に攻撃キーを離した
    //何もしない
    private int attackCount = 0;//最初の攻撃キーを押してから最後の攻撃の終了まで増加する
    private bool onAttackCount;//攻撃カウント中か否か

    private int attackNum = 0;//現在の攻撃回数
    private const int ATTACK_NUM_MAX = 3;//攻撃回数の上限値

    private int stateCKey = 0;
    //0.Cキーが押されていない
    //1.Cキーが押された瞬間
    //2.Cキーが放された

    private bool stop = false;//ストップしているかどうか
    private Animator animator;  //アニメーター
    public enum MOVE_DIR
    {
        STOP,
        LEFT,
        RIGHT,
    };

    private MOVE_DIR moveDirection = MOVE_DIR.STOP;


    public AudioClip jumpSE;
    public AudioClip getSE;
    public AudioClip stampSE;

    private AudioSource audioSource;

    private GameObject touchedOrb;  // 最後に触ったオーブ 初期値は null

    private Timer timer;
    private bool isKeyReceiving = true;
    private bool isKeyRegistered = false;
    private int finishedAttackNum = 0;
    private int nextAttackNum = 0;

    void Start()
    {
        audioSource = gameManager.GetComponent<AudioSource>();
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = gameObject.GetComponent<Timer>();
    }
    void Update()
    {

        //押しているボタンで分岐
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            PushLeftButton();

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            PushRightButton();
        }
        else
        {
            ReleaseMoveButton();
        }

        canJump =
            Physics2D.Linecast(transform.position - (transform.right * 0.3f),
            transform.position - (transform.up * 0.1f), blockLayer) ||
            Physics2D.Linecast(transform.position + (transform.right * 0.3f),
            transform.position - (transform.up * 0.1f), blockLayer);

        animator.SetBool("onGround", canJump);
        //ストップしているかどうか（アニメーションに影響）
        if (moveDirection == MOVE_DIR.STOP)
        {
            stop = true;

        }
        else
        {
            stop = false;
        }
        animator.SetBool("stop", stop);

        //ジャンプできる場合は常にstateJumpを0にセット
        if (canJump)
        {
            stateJump = 0;
        }

        bool oneFrameCKey = false;
        if (isKeyReceiving)
        {
            oneFrameCKey = Input.GetKeyDown(KeyCode.C);//GetKeyDownはキーを1フレームだけ取得する
        }

        if (oneFrameCKey)
        {
            // first attack implementation
            if (finishedAttackNum == 0 && nextAttackNum == 0)
            {
                timer.Begin();
                animator.SetTrigger("attack");
                finishedAttackNum = 1;
                nextAttackNum = 2;
            }
            //second attack registration
            else if (finishedAttackNum == 1 && nextAttackNum == 2 && !isKeyRegistered)
            {
                if (timer.ElapsedTime * 12f < 6f)
                {
                    isKeyReceiving = false;
                    isKeyRegistered = true;
                }
            }
            //third attack registration
            else if (finishedAttackNum == 2 && nextAttackNum == 3 && !isKeyRegistered)
            {
                
            }
        }

        //second attack implementation
        if (finishedAttackNum == 1 && nextAttackNum == 2 && isKeyRegistered)
        {
            //when first attack finished
            if (timer.ElapsedTime * 12f > 6f)
            {
                animator.SetTrigger("attack");
                finishedAttackNum = 2;
                nextAttackNum = 3;
            }
        }
    }

    void FixedUpdate()
    {

        //moveDirectionの値によって速度を設定
        switch (moveDirection)
        {
            case MOVE_DIR.STOP:
                moveSpeed = 0;
                break;
            case MOVE_DIR.LEFT:
                moveSpeed = MOVE_SPEED * -1;
                transform.localScale = new Vector2(1, 1);
                break;
            case MOVE_DIR.RIGHT:
                moveSpeed = MOVE_SPEED;
                transform.localScale = new Vector2(-1, 1);
                break;
        }
        rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);

        //通常では重力は2
        rbody.gravityScale = 2f;

        if (Input.GetKey(KeyCode.Space))
        {
            jumpCount++;
        }
        else
        {
            jumpCount = 0;
        }
        //ジャンプボタンを押したときの処理
        if (stateJump == 0)
        {
            if (jumpCount == 1)
            {
                stateJump = 1;
            }
        }
        else if (stateJump == 2 && jumpCount > 1)
        {
            //空中にいて
            //スペースキーを押しているときは何もしない
            stateJump = 3;
        }
        else if (stateJump == 3 && jumpCount == 0 && rbody.velocity.y > 0)
        {
            //空中にいて
            //スペースキーを離したときは
            rbody.gravityScale = 5.0f;
        }
        else
        {
            //何もしない
        }
        //ジャンプ処理
        if (stateJump == 1)
        {
            audioSource.PlayOneShot(jumpSE);
            rbody.AddForce(Vector2.up * jumpPower);
            stateJump = 2;
        }
    }

    //衝突処理
    void OnTriggerEnter2D(Collider2D col)
    {
        if (gameManager.GetComponent<GameManager>().gameMode != GameManager.GAME_MODE.PLAY)
        {
            return;
        }
        if (col.gameObject.tag == "Trap")
        {
            gameManager.GetComponent<GameManager>().GameOver();
            DestroyPlayer();
        }
        if (col.gameObject.tag == "Goal")
        {
            gameManager.GetComponent<GameManager>().GameClear();
        }

        if (col.gameObject.tag == "Enemy")
        {
            if (transform.position.y > col.gameObject.transform.position.y + 0.4f)
            {
                audioSource.PlayOneShot(stampSE);
                rbody.velocity = new Vector2(rbody.velocity.x, 0);
                rbody.AddForce(Vector2.up * jumpPower);
                col.gameObject.GetComponent<EnemyManager>().DestroyEnemy();
            }
            else
            {
                gameManager.GetComponent<GameManager>().GameOver();
                DestroyPlayer();
            }
        }

        if (col.gameObject.tag == "Orb")
        {
            // スコア二重取得回避:もし最後に触ったOrbと今触ったOrbが重複してなかったら
            if (col.gameObject != touchedOrb)
            {
                touchedOrb = col.gameObject;
                audioSource.PlayOneShot(getSE);
                col.gameObject.GetComponent<OrbManager>().GetOrb();
            }
        }
    }

    //プレイヤーオブジェクト削除処理
    void DestroyPlayer()
    {
        gameManager.GetComponent<GameManager>().gameMode = GameManager.GAME_MODE.GAMEOVER;
        //コライダーを削除
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Destroy(circleCollider);
        Destroy(boxCollider);
        //死亡アニメーション
        Sequence animSet = DOTween.Sequence();
        animSet.Append(transform.DOLocalMoveY(1.0f, 0.2f).SetRelative());
        animSet.Append(transform.DOLocalMoveY(-10.0f, 1.0f).SetRelative());
        Destroy(this.gameObject, 1.2f);
    }

    public void PushLeftButton()
    {
        moveDirection = MOVE_DIR.LEFT;
    }

    public void PushRightButton()
    {
        moveDirection = MOVE_DIR.RIGHT;
    }

    public void ReleaseMoveButton()
    {
        moveDirection = MOVE_DIR.STOP;

    }
    public void PushJumpButton()
    {
        if (canJump)
        {
            goJump = true;

        }
    }

    void HitSpaceTime()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            tmpJumpPowerTime += Time.deltaTime;
        }
        else
        {
            jumpPowerTime = tmpJumpPowerTime;
        }
        //ジャンプした現在のフレーム時にjumpPowerTimeとtmpJumpPowerTimeは初期化する
    }

}
