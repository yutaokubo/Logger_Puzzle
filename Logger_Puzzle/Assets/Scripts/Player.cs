using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;//移動速度

    private Vector2 mapChipDistance;//1マスごとの移動距離

    private Vector3 moveX;//X方向移動量
    private Vector3 moveY;//Y方向移動量
    [SerializeField]
    private Vector3 moveTargetPosition;//移動先
    private Vector3 movePreviousPosition;//元の位置

    [SerializeField]
    private Direction.DirectionState direction;
    private SpriteRenderer renderer;
    
    [SerializeField]
    private float slashTime;
    private float slashTimer;

    [SerializeField]
    private float fallingTime;
    private float fallingTimer;

    enum PlayerMode
    {
        Nomal,//通常状態
        MoveWeit,//移動待機
        Moving,//移動中
        AutoMoveWeit,//自動移動待機
        AutoMoving,//自動移動
        SlashWeit,//切る待機
        Slashing,//切っている
        Falling,//落ちている
        Falled,//落ちた
    }
    [SerializeField]
    private PlayerMode playerMode;
    private PlayerMode previousPlayerMode;
    
    enum AnimationMode
    {
        Nomal,//通常
        Walk,//移動
        Slash,//切る
    }
    private AnimationMode animationMode;

    [SerializeField]
    private PlayerSpriteChanger spriteChanger;

    private float walkSpriteTimer;
    [SerializeField]
    private float walkSpriteSpeed;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {

        moveTargetPosition = transform.position;
        direction = Direction.DirectionState.Down;
        animator = GetComponent<Animator>();
        animationMode = AnimationMode.Nomal;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == moveTargetPosition)
        {
            SetTargetPosition();
        }
        FallingUpdate();
        Move();
        MoveModeUpdate();
        Slash();
        SlashingUpdate();
        Animation();
        PreviousModeUpdate();
        //SpriteChange();
        Debug.Log(playerMode);
    }
    /// <summary>
    /// 移動先設定
    /// </summary>
    private void SetTargetPosition()
    {
        if (playerMode != PlayerMode.Nomal)
            return;
        if (previousPlayerMode != PlayerMode.Nomal)
            return;

        movePreviousPosition = moveTargetPosition;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveTargetPosition = transform.position + moveY;
            direction = Direction.DirectionState.Up;
            playerMode = PlayerMode.MoveWeit;
            //transform.rotation = Quaternion.Euler(0,0,0);
            return;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveTargetPosition = transform.position - moveY;
            direction = Direction.DirectionState.Down;
            playerMode = PlayerMode.MoveWeit;
            //transform.rotation = Quaternion.Euler(0, 0, 180);
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveTargetPosition = transform.position + moveX;
            direction = Direction.DirectionState.Right;
            playerMode = PlayerMode.MoveWeit;
            //transform.rotation = Quaternion.Euler(0, 0, 270);
            return;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveTargetPosition = transform.position - moveX;
            direction = Direction.DirectionState.Left;
            playerMode = PlayerMode.MoveWeit;
            //transform.rotation = Quaternion.Euler(0, 0, 90);
            return;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        if (playerMode != PlayerMode.Moving && playerMode != PlayerMode.AutoMoving)
            return;
        
        if (playerMode == PlayerMode.Moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, moveSpeed * Time.deltaTime);
            animationMode = AnimationMode.Walk;
        }
        if (playerMode == PlayerMode.AutoMoving)
            transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, 3 * Time.deltaTime);
    }

    private void MoveModeUpdate()
    {
        if (playerMode == PlayerMode.Moving || playerMode == PlayerMode.AutoMoving)
        {
            if (transform.position == moveTargetPosition)
            {
                playerMode = PlayerMode.Nomal;
                animationMode = AnimationMode.Nomal;
            }
        }
    }

    public void AutoMoveStart(Direction.DirectionState dir)
    {
        //if (playerMode != PlayerMode.Nomal && playerMode != PlayerMode.Slashing)
        //    return;

        if (dir == Direction.DirectionState.Up)
            moveTargetPosition = transform.position + moveY;
        if (dir == Direction.DirectionState.Down)
            moveTargetPosition = transform.position - moveY;
        if (dir == Direction.DirectionState.Right)
            moveTargetPosition = transform.position + moveX;
        if (dir == Direction.DirectionState.Left)
            moveTargetPosition = transform.position - moveX;

        
        playerMode = PlayerMode.AutoMoving;
    }

    /// <summary>
    /// 前方を切る
    /// </summary>
    private void Slash()
    {
        if (playerMode != PlayerMode.Nomal && playerMode != PlayerMode.MoveWeit)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerMode == PlayerMode.MoveWeit)
            {
                SetTargetPosition(transform.position);
            }
            playerMode = PlayerMode.SlashWeit;
            animationMode = AnimationMode.Slash;
        }
    }
    private void SlashingUpdate()
    {
        if (animationMode != AnimationMode.Slash)
            return;

        slashTimer += Time.deltaTime;
        if (slashTimer > slashTime)
        {
            slashTimer = 0;
            animationMode = AnimationMode.Nomal;
            if(playerMode == PlayerMode.Slashing)
            {
                playerMode = PlayerMode.Nomal;
            }
        }
    }

    /// <summary>
    /// 外部から移動先を設定
    /// </summary>
    /// <param name="target"></param>
    public void SetTargetPosition(Vector3 target)
    {
        moveTargetPosition = target;
    }

    public void SetMoveDistance(Vector2 distance)
    {
        mapChipDistance = distance;
        moveX = new Vector3(mapChipDistance.x, 0, 0);
        moveY = new Vector3(0, mapChipDistance.y, 0);
    }

    /// <summary>
    /// 現在の向いている方向を返す。
    /// </summary>
    /// <returns></returns>
    public Direction.DirectionState GetDirection()
    {
        return direction;
    }

    public int GetPlayerMode()
    {
        return (int)playerMode;
    }
    public void SetPlayerMode(int modeNumber)
    {
        playerMode = (PlayerMode)modeNumber;
    }

    public void Fall()
    {
        playerMode = PlayerMode.Falling;
    }
    private void FallingUpdate()
    {
        if (playerMode != PlayerMode.Falling)
            return;

        fallingTimer += Time.deltaTime;
        transform.localScale -= new Vector3(0.5f, 0.5f, 0) * Time.deltaTime;
        if (fallingTimer >= fallingTime)
        {
            playerMode = PlayerMode.Falled;
            this.gameObject.SetActive(false);
        }
    }

    private void Animation()
    {
        animator.SetInteger("Direction", (int)(direction));

        //if (playerMode == PlayerMode.Moving)
        //{
        //    animator.SetBool("Walk", true);
        //}
        //else
        //{
        //    animator.SetBool("Walk", false);
        //}
        //if (playerMode == PlayerMode.Slashing)
        //{
        //    animator.SetBool("Slash", true);
        //}
        //else
        //{
        //    animator.SetBool("Slash", false);
        //}
        if(animationMode == AnimationMode.Walk)
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Slash", false);
        }
        else if(animationMode == AnimationMode.Slash)
        {
            animator.SetBool("Slash", true);
            animator.SetBool("Walk", false);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Slash", false);
        }
    }

    private void PreviousModeUpdate()
    {
        previousPlayerMode = playerMode;
    }

    public void ChangeLayer(int num)
    {
        if (renderer != null)
            renderer.sortingOrder = num;
    }
}
