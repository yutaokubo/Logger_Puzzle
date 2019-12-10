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
}
