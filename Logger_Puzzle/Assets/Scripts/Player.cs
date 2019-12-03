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
    }
    [SerializeField]
    private MoveMode moveMode;

    enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }
    [SerializeField]
    private Direction direction;//向いている方向

    // Start is called before the first frame update
    void Start()
    {

        moveTargetPosition = transform.position;
        direction = Direction.Up;
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
    }
    /// <summary>
    /// 移動先設定
    /// </summary>
    private void SetTargetPosition()
    {
        movePreviousPosition = moveTargetPosition;


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveTargetPosition = transform.position + moveY;
            direction = Direction.Up;
            moveMode = MoveMode.MoveSet;
            return;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveTargetPosition = transform.position - moveY;
            direction = Direction.Down;
            moveMode = MoveMode.MoveSet;
            return;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveTargetPosition = transform.position + moveX;
            direction = Direction.Right;
            moveMode = MoveMode.MoveSet;
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveTargetPosition = transform.position - moveX;
            direction = Direction.Left;
            moveMode = MoveMode.MoveSet;
            return;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        if (moveMode != MoveMode.Moving)
            return;

        transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, moveSpeed * Time.deltaTime);
    }
    
    private void MoveModeUpdate()
    {
        if(moveMode == MoveMode.Moving)
        {
            if (transform.position == moveTargetPosition)
                moveMode = MoveMode.Stop;
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
    public int GetDirection()
    {
        return (int)direction;
    }

    /// <summary>
    /// 移動状態をマップマネージャーから変更できるように
    /// </summary>
    /// <returns></returns>
    public int GetMoveMode()
    {
        return (int)moveMode;
    }
    /// <summary>
    /// 移動状態取得
    /// </summary>
    /// <param name="modeNum"></param>
    public void SetMoveMode(int modeNum)
    {
        moveMode = (MoveMode)modeNum;
    }
}
