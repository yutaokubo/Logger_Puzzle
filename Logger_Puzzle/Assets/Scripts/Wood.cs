using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    private int length;//木の長さ

    private int[] rootPoint = new int[2];//根元のマス目のポイント

    private enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }

    private Direction direction;//方向


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetLength()
    {
        return length;
    }

    public int[] GetRootPoint()
    {
        return rootPoint;
    }

    public int GetDirection()
    {
        return (int)direction;
    }
}
