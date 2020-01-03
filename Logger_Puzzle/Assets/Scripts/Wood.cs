﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    [SerializeField]
    private int length;//木の長さ

    [SerializeField]
    private Vector2 rootPoint;//根元のマス目のポイント
    [SerializeField]
    private Vector2[] mapPoints;//使用しているマップチップのポイント

    private Direction.DirectionState direction;//方向

    private float breakTimer;
    [SerializeField]
    private float breakTime;
    private enum WoodState
    {
        Nomal,//通常状態
        Breaking,//壊れる
        MoveSet,//移動待機
        Moving,//移動中
        None,//無くなった
    }

    [SerializeField]
    private WoodState state;

    private Vector3 MoveX = new Vector3(0.64f, 0, 0);//X移動量
    private Vector3 MoveY = new Vector3(0, 0.64f, 0);//Y移動量
    private Vector3 moveTargetPosition;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        BreakedUpdate();
        DeadChack();
    }

    public int GetLength()
    {
        return length;
    }
    public void SetLength(int length)
    {
        this.length = length;
    }

    public Vector2 GetRootPoint()
    {
        return rootPoint;
    }
    public void SetRootPoint(Vector2 point)
    {
        rootPoint = point;
        SetMapPoints(point);
    }
    private void SetMapPoints(Vector2 point)
    {
        mapPoints = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            mapPoints[i] = GetDistancePoint(point, i);
        }
    }
    public void ChangeMapPoints(int num,Vector2 point)
    {
        if(num == 0)
        {
            rootPoint = point;
        }
        mapPoints[num] = point;
    }
    private Vector2 GetDistancePoint(Vector2 point, int length)
    {
        if (direction == Direction.DirectionState.Up)
            return point + new Vector2(0, -length);
        if (direction == Direction.DirectionState.Down)
            return point + new Vector2(0, length);
        if (direction == Direction.DirectionState.Right)
            return point + new Vector2(length, 0);
        if (direction == Direction.DirectionState.Left)
            return point + new Vector2(-length, 0);

        return point;
    }

    /// <summary>
    /// 移動設定
    /// </summary>
    /// <param name="moveDir"></param>
    public void MoveSet(Direction.DirectionState moveDir)
    {
        if (state != WoodState.Nomal)
            return;

        if (moveDir == Direction.DirectionState.Up)
        {
            moveTargetPosition = transform.position + MoveY;
        }
        if (moveDir == Direction.DirectionState.Down)
        {
            moveTargetPosition = transform.position - MoveY;
        }
        if (moveDir == Direction.DirectionState.Right)
        {
            moveTargetPosition = transform.position + MoveX;
        }
        if (moveDir == Direction.DirectionState.Left)
        {
            moveTargetPosition = transform.position - MoveX;
        }

        state = WoodState.Moving;
    }

    private void Move()
    {
        if (state != WoodState.Moving)
            return;

        transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, 5 * Time.deltaTime);
        MoveStop();
    }

    private void MoveStop()
    {
        if(transform.position == moveTargetPosition)
        {
            state = WoodState.Nomal;
        }
    }


    public bool IsIncludedMapPoint(Vector2 point)
    {
        foreach (Vector2 p in mapPoints)
        {
            if (p == point)
            {
                return true;
                Debug.Log("Wood");
            }
        }
        return false;
    }

    public Direction.DirectionState GetDirection()
    {
        return direction;
    }

    public void SetDirection(Direction.DirectionState direction)
    {
        this.direction = direction;
        DirectionLook();
    }

    /// <summary>
    /// 自分の方向に応じて向きを変える
    /// </summary>
    private void DirectionLook()
    {
        if (direction == Direction.DirectionState.Up || direction == Direction.DirectionState.Down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (direction == Direction.DirectionState.Right || direction == Direction.DirectionState.Left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    /// <summary>
    /// 斬られたとき乗れなければ壊れる
    /// </summary>
    public void Breaked()
    {
        state = WoodState.Breaking;
    }

    private void BreakedUpdate()
    {
        if (state != WoodState.Breaking)
            return;

        breakTimer += Time.deltaTime;
        if (breakTimer >= breakTime)
        {
            state = WoodState.None;
        }
    }

    private void DeadChack()
    {
        if (state == WoodState.None)
            Destroy(gameObject);
    }

    public int GetState()
    {
        return (int)state;
    }
}
