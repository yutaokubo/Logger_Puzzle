using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;
    
    private Vector2 mapChipDistance;

    private Vector3 moveX;
    private Vector3 moveY;

    private Vector3 moveTargetPosition;
    private Vector3 movePreviousPosition;

    enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }
    [SerializeField]
    private Direction direction;

    // Start is called before the first frame update
    void Start()
    {

        moveTargetPosition = transform.position;
        direction = Direction.Up;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position == moveTargetPosition)
        {
            SetTargetPosition();
        }
        Move();
    }
    /// <summary>
    /// 移動先設定
    /// </summary>
    private void SetTargetPosition()
    {
        movePreviousPosition = moveTargetPosition;


        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveTargetPosition = transform.position + moveY;
            direction = Direction.Up;
            return;
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveTargetPosition = transform.position - moveY;
            direction = Direction.Down;
            return;
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveTargetPosition = transform.position + moveX;
            direction = Direction.Right;
            return;
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveTargetPosition = transform.position - moveX;
            direction = Direction.Left;
            return;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, moveSpeed * Time.deltaTime);
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
}
