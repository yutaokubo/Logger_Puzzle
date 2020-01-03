using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    private int length;//木の長さ

    [SerializeField]
    private Vector2 rootPoint;//根元のマス目のポイント

    private Direction.DirectionState direction;//方向

    private float breakTimer;
    [SerializeField]
    private float breakTime;
    private enum WoodState
    {
        Nomal,//通常状態
        Breaking,//壊れる
        None,//無くなった
    }

    [SerializeField]
    private WoodState state;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BreakedUpdate();
        DeadChack();
    }

    public int GetLength()
    {
        return length;
    }

    public Vector2 GetRootPoint()
    {
        return rootPoint;
    }
    public void SetRootPoint(Vector2 point)
    {
        rootPoint = point;
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
}
