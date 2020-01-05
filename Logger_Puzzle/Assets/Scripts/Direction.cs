using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Direction : MonoBehaviour
{

    public enum DirectionState
    {
        Up,
        Down,
        Right,
        Left,
    }

    public DirectionState state;

    public DirectionState State
    {
        set { this.state = value; }
        get { return this.state; }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static bool IsSameAxis(Direction.DirectionState dir1,Direction.DirectionState dir2)
    {
        if ((dir1 == Direction.DirectionState.Up || dir1 == Direction.DirectionState.Down)
            && (dir2 == Direction.DirectionState.Up || dir2 == Direction.DirectionState.Down))
        {
            return true;
        }
        if ((dir1 == Direction.DirectionState.Right || dir1 == Direction.DirectionState.Left)
            && (dir2 == Direction.DirectionState.Right || dir2 == Direction.DirectionState.Left))
        {
            return true;
        }
        return false;
    }

    public static Direction.DirectionState GetReverseDirection(Direction.DirectionState dir)
    {
        if (dir == DirectionState.Up)
            return DirectionState.Down;
        if (dir == DirectionState.Down)
            return DirectionState.Up;
        if (dir == DirectionState.Right)
            return DirectionState.Left;
        if (dir == DirectionState.Left)
            return DirectionState.Right;

        return DirectionState.Up;
    }

}
