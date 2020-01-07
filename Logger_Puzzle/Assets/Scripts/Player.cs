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

    private Vector3 moveTargetPosition;//移動先
    private Vector3 movePreviousPosition;//元の位置

    enum MoveMode
    {
        Stop,//止まっている
        MoveSet,//移動しようとしている
        Moving,//移動中
        AutoMoveSet,//自動移動しようとしている
        AutoMoving,//自動移動
    }
    [SerializeField]
    private MoveMode moveMode;

    [SerializeField]
    private Direction.DirectionState direction;
    private SpriteRenderer renderer;

    enum SlashMode
    {
        None,//切ろうとしていない
        Wait,//切る待機中
        Slashing//切っている
    }
    private SlashMode slashMode;
    [SerializeField]
    private float slashTime;
    private float slashTimer;


    [SerializeField]
    private PlayerSpriteChanger spriteChanger;

    // Start is called before the first frame update
    void Start()
    {

        moveTargetPosition = transform.position;
        direction = Direction.DirectionState.Down;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == moveTargetPosition)
        {
            SetTargetPosition();
        }
        Move();
        MoveModeUpdate();
        Slash();
        SlashingUpdate();
        SpriteChange();
    }
    /// <summary>
    /// 移動先設定
    /// </summary>
    private void SetTargetPosition()
    {
        if (moveMode != MoveMode.Stop || slashMode != SlashMode.None)
            return;

        movePreviousPosition = moveTargetPosition;


        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveTargetPosition = transform.position + moveY;
            direction = Direction.DirectionState.Up;
            moveMode = MoveMode.MoveSet;
            //transform.rotation = Quaternion.Euler(0,0,0);
            return;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveTargetPosition = transform.position - moveY;
            direction = Direction.DirectionState.Down;
            moveMode = MoveMode.MoveSet;
            //transform.rotation = Quaternion.Euler(0, 0, 180);
            return;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveTargetPosition = transform.position + moveX;
            direction = Direction.DirectionState.Right;
            moveMode = MoveMode.MoveSet;
            //transform.rotation = Quaternion.Euler(0, 0, 270);
            return;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveTargetPosition = transform.position - moveX;
            direction = Direction.DirectionState.Left;
            moveMode = MoveMode.MoveSet;
            //transform.rotation = Quaternion.Euler(0, 0, 90);
            return;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        if (moveMode != MoveMode.Moving && moveMode != MoveMode.AutoMoving)
            return;
        if (moveMode == MoveMode.Moving)
            transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, moveSpeed * Time.deltaTime);
        if (moveMode == MoveMode.AutoMoving)
            transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, 5 * Time.deltaTime);
    }

    private void MoveModeUpdate()
    {
        if (moveMode == MoveMode.Moving || moveMode == MoveMode.AutoMoving)
        {
            if (transform.position == moveTargetPosition)
                moveMode = MoveMode.Stop;
        }
    }

    public void AutoMoveStart(Direction.DirectionState dir)
    {
        if (moveMode != MoveMode.Stop)
            return;

        if (dir == Direction.DirectionState.Up)
            moveTargetPosition = transform.position + moveY;
        if (dir == Direction.DirectionState.Down)
            moveTargetPosition = transform.position - moveY;
        if (dir == Direction.DirectionState.Right)
            moveTargetPosition = transform.position + moveX;
        if (dir == Direction.DirectionState.Left)
            moveTargetPosition = transform.position - moveX;

        moveMode = MoveMode.AutoMoving;
    }

    /// <summary>
    /// 前方を切る
    /// </summary>
    private void Slash()
    {
        if (moveMode != MoveMode.Stop&&moveMode != MoveMode.MoveSet&&moveMode != MoveMode.Moving)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            slashMode = SlashMode.Wait;
        }
    }
    private void SlashingUpdate()
    {
        if (slashMode != SlashMode.Slashing)
            return;

        slashTimer += Time.deltaTime;
        if (slashTimer > slashTime)
        {
            slashTimer = 0;
            slashMode = SlashMode.None;
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

    /// <summary>
    /// 移動状態取得
    /// </summary>
    /// <param name="modeNum"></param>
    public int GetMoveMode()
    {
        return (int)moveMode;
    }
    /// <summary>
    /// 移動状態をマップマネージャーから変更できるように
    /// </summary>
    /// <returns></returns>
    public void SetMoveMode(int modeNum)
    {
        moveMode = (MoveMode)modeNum;
    }

    public int GetSlashMode()
    {
        return (int)slashMode;
    }
    public void SetSlashMode(int modeNum)
    {
        slashMode = (SlashMode)modeNum;
    }

    private void SpriteChange()
    {
        if (renderer == null)
        {
            renderer = gameObject.GetComponent<SpriteRenderer>();
        }

        switch (moveMode)
        {
            case MoveMode.Stop:
            case MoveMode.MoveSet:
            case MoveMode.AutoMoveSet:
            case MoveMode.AutoMoving:
                renderer.sprite = spriteChanger.GetNomalSprite(direction);
                break;

        }

        switch (slashMode)
        {
            case SlashMode.Slashing:
                renderer.sprite = spriteChanger.GetSlashSprite(direction);
                break;
        }
    }

    public void ChangeLayer(int num)
    {
        if (renderer != null)
            renderer.sortingOrder = num;
    }
}
