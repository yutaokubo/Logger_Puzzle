using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectPlayerIcon : MonoBehaviour
{

    private enum State
    {
        Stop,//停止
        RightWalk,//右移動中
        LeftWalk,//左移動中
    }

    private State state;
    [SerializeField]
    private Sprite[] sprites;

    private SpriteRenderer renderer;

    private float moveTimer;
    [SerializeField]
    private float walkSpeed;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        WalkUpdate();
    }

    public void RightWalkStart()
    {
        if (state != State.Stop)
            return;

        state = State.RightWalk;
    }
    public void LeftWalkStart()
    {
        if (state != State.Stop)
            return;

        state = State.LeftWalk;
    }

    private void WalkUpdate()
    {
        if (state != State.RightWalk && state != State.LeftWalk)
            return;

        moveTimer += walkSpeed * Time.deltaTime;
        if(state == State.RightWalk)
        {
            renderer.sprite = sprites[(int)(moveTimer % 2 + 1)];
        }
        if (state == State.LeftWalk)
        {
            renderer.sprite = sprites[(int)(moveTimer % 2 + 3)];
        }
    }

    public void Stop()
    {

        if (state != State.RightWalk && state != State.LeftWalk)
            return;

        moveTimer = 0;
        renderer.sprite = sprites[0];
        state = State.Stop;
    }
}
