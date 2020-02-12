using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectPoint : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField]
    private float speed;
    
    private enum MoveState
    {
        Stop,//停止
        Moving,//移動中
    }
    private MoveState state;

    // Start is called before the first frame update
    void Start()
    {
        state = MoveState.Stop;
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
    }
    

    public int GetState()
    {
        return (int)state;
    }

    public void MoveStart(Vector3 target,float time)
    {
        if (state != MoveState.Stop)
            return;

        targetPosition = target;
        state = MoveState.Moving;
        speed = (targetPosition.x - transform.position.x)/time;
    }

    private void MoveUpdate()
    {
        if (state != MoveState.Moving)
            return;

        transform.position += new Vector3(speed * Time.deltaTime,0, 0);
        if(speed<0)
        {
            if(transform.position.x<targetPosition.x)
            {
                state = MoveState.Stop;
                transform.position = targetPosition;
            }
        }
        else
        {
            if (transform.position.x > targetPosition.x)
            {
                state = MoveState.Stop;
                transform.position = targetPosition;
            }
        }
    }
}
