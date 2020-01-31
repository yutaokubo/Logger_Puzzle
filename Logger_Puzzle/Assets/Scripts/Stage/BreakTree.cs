using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTree : MonoBehaviour
{
    [SerializeField]
    private float endTime;
    private float endTimer;
    [SerializeField]
    private Direction.DirectionState spinDir;

    [SerializeField]
    private Sprite[] treeSprites;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Spin();
        EndCount();
    }

    private void Spin()
    {
        float spinAngle;
        switch (spinDir)
        {
            case Direction.DirectionState.Left:
                spinAngle = 90 / endTime * Time.deltaTime;
                transform.Rotate(new Vector3(0, 0, 1), spinAngle);
                transform.position += new Vector3(-5 * Time.deltaTime, 0, 0);
                break;

            case Direction.DirectionState.Right:
                spinAngle = -90 / endTime * Time.deltaTime;
                transform.Rotate(new Vector3(0, 0, 1), spinAngle);
                transform.position += new Vector3(5 * Time.deltaTime, 0, 0);
                break;

            case Direction.DirectionState.Down:
                spinAngle = 180 / endTime * Time.deltaTime;
                transform.Rotate(new Vector3(0, 0, 1), spinAngle);
                transform.position += new Vector3(0, -5 * Time.deltaTime, 0);
                break;

            case Direction.DirectionState.Up:
                transform.position += new Vector3(0, 5 * Time.deltaTime, 0);
                break;

        }
    }
    private void EndCount()
    {
        endTimer += Time.deltaTime;
        if (endTimer > endTime)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetLenght(int lenght)
    {
        GameObject sp = transform.GetChild(0).gameObject;

        if (sp.GetComponent<SpriteRenderer>() != null)
        {
            sp.GetComponent<SpriteRenderer>().sprite = treeSprites[lenght];
        }
    }
    public void SetSpinDirection(Direction.DirectionState dir)
    {
        spinDir = dir;
    }

    public void ChangeLayer(int num)
    {
        Debug.Log("BT:" + num);
        GameObject tree = transform.GetChild(0).gameObject;
        tree.transform.GetComponent<SpriteRenderer>().sortingOrder = (num + 1) * 10 + 3;
    }

}
