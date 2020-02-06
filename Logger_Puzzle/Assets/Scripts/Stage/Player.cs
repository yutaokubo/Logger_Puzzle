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
        Goal,//ゴールした
        GoalEnd,//ゴールし終わった
        Starting,//開始時
    }
    [SerializeField]
    private PlayerMode playerMode;
    private PlayerMode previousPlayerMode;

    [SerializeField]
    private float walkStartTime;
    private float walkStartTimer;

    enum AnimationMode
    {
        Nomal,//通常
        Walk,//移動
        Slash,//切る
        Goal,//ゴール
    }
    private AnimationMode animationMode;

    [SerializeField]
    private Vector3 offsetValue;
    [SerializeField]
    private Vector3 nowOffset;
    

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        moveTargetPosition = transform.position;
        direction = Direction.DirectionState.Down;
        animator = GetComponent<Animator>();
        animationMode = AnimationMode.Nomal;
        playerMode = PlayerMode.Starting;
        walkStartTimer = walkStartTime;
    }

    // Update is called once per frame
    void Update()
    {
        StartingUpdate();
        SetTargetPosition();
        FallingUpdate();
        Move();
        MoveModeUpdate();
        Slash();
        SlashingUpdate();
        GoalUpdate();
        Animation();
        PreviousModeUpdate();
        //Debug.Log(playerMode);
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

        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            walkStartTimer = 0;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (direction == Direction.DirectionState.Up)
            {
                walkStartTimer += Time.deltaTime;
                if (walkStartTimer >= walkStartTime)
                {
                    moveTargetPosition = transform.position + moveY;
                    playerMode = PlayerMode.MoveWeit;
                }
            }
            else
            {
                direction = Direction.DirectionState.Up;
                //walkStartTimer = 0;
            }
            //transform.rotation = Quaternion.Euler(0,0,0);
            return;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (direction == Direction.DirectionState.Down)
            {
                walkStartTimer += Time.deltaTime;
                if (walkStartTimer >= walkStartTime)
                {
                    moveTargetPosition = transform.position - moveY;
                    playerMode = PlayerMode.MoveWeit;
                }
            }
            else
            {
                direction = Direction.DirectionState.Down;
                //walkStartTimer = 0;
            }
            //transform.rotation = Quaternion.Euler(0, 0, 180);
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (direction == Direction.DirectionState.Right)
            {
                walkStartTimer += Time.deltaTime;
                if (walkStartTimer >= walkStartTime)
                {
                    moveTargetPosition = transform.position + moveX;
                    playerMode = PlayerMode.MoveWeit;
                }

            }
            else
            {
                direction = Direction.DirectionState.Right;
                //walkStartTimer = 0;
            }
            //transform.rotation = Quaternion.Euler(0, 0, 270);
            return;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (direction == Direction.DirectionState.Left)
            {
                walkStartTimer += Time.deltaTime;
                if (walkStartTimer >= walkStartTime)
                {
                    moveTargetPosition = transform.position - moveX;
                    playerMode = PlayerMode.MoveWeit;
                }
            }
            else
            {
                direction = Direction.DirectionState.Left;
                //walkStartTimer = 0;
            }
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
            transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition + nowOffset, moveSpeed * Time.deltaTime);
            animationMode = AnimationMode.Walk;
        }
        if (playerMode == PlayerMode.AutoMoving)
            transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, 3 * Time.deltaTime);
    }

    private void MoveModeUpdate()
    {
        if (playerMode == PlayerMode.Moving || playerMode == PlayerMode.AutoMoving)
        {
            if (transform.position == moveTargetPosition + nowOffset)
            {
                playerMode = PlayerMode.Nomal;
                animationMode = AnimationMode.Nomal;
                OffSetReset();
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
            if (playerMode == PlayerMode.Slashing)
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

        transform.position -= offsetValue * Time.deltaTime / fallingTime;

        if (fallingTimer >= fallingTime)
        {
            playerMode = PlayerMode.Falled;
            renderer.enabled = false;
            //this.gameObject.SetActive(false);
        }
    }
    private void GoalUpdate()
    {
        if (playerMode != PlayerMode.Goal)
            return;
        fallingTimer += Time.deltaTime;
        transform.localScale -= new Vector3(0.5f, 0.5f, 0) * Time.deltaTime;
        if (fallingTimer >= fallingTime)
        {
            playerMode = PlayerMode.GoalEnd;
            renderer.enabled = false;
            //this.gameObject.SetActive(false);
        }

    }


    public void StartDirectSetting()
    {
        transform.position = new Vector3(transform.position.x, 5, 0);
        playerMode = PlayerMode.Starting;
    }
    private void StartingUpdate()
    {
        if (playerMode != PlayerMode.Starting)
            return;

        transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition + nowOffset, 15 * Time.deltaTime);
        if (transform.position == moveTargetPosition + nowOffset)
        {
            playerMode = PlayerMode.Nomal;
            animationMode = AnimationMode.Nomal;
            OffSetReset();
        }
    }

    public void OffSetReset()
    {
        nowOffset = Vector2.zero;
    }
    public void OffsetAdd()
    {
        nowOffset = offsetValue;
    }
    public void OffsetRemove()
    {
        nowOffset = -offsetValue;
    }

    private void Animation()
    {
        animator.SetInteger("Direction", (int)(direction));


        if (animationMode == AnimationMode.Walk)
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Slash", false);
        }
        else if (animationMode == AnimationMode.Slash)
        {
            animator.SetBool("Slash", true);
            animator.SetBool("Walk", false);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Slash", false);
        }

        if(playerMode == PlayerMode.Goal)
        {
            if(animator.GetBool("Goal")==false)
            {
                animationMode = AnimationMode.Goal;
                animator.SetBool("Walk", false);
                animator.SetBool("Slash", false);
                animator.SetBool("Goal", true);
            }
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
