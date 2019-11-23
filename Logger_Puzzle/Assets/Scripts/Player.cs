using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float mapChipDistance;

    private Vector3 moveX;
    private Vector3 moveY;

    private Vector3 moveTargetPosition;
    private Vector3 movePreviousPosition;

    // Start is called before the first frame update
    void Start()
    {
        moveX = new Vector3(mapChipDistance, 0, 0);
        moveY = new Vector3(0, mapChipDistance, 0);

        moveTargetPosition = transform.position;
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

    private void SetTargetPosition()
    {
        movePreviousPosition = moveTargetPosition;


        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveTargetPosition = transform.position + moveY;
            return;
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveTargetPosition = transform.position - moveY;
            return;
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveTargetPosition = transform.position + moveX;
            return;
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveTargetPosition = transform.position - moveX;
            return;
        }
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveTargetPosition, moveSpeed * Time.deltaTime);
    }
}
