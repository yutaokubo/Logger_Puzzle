using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManager : MonoBehaviour
{
    [SerializeField]
    private Wood wood;

    private List<Wood> woods = new List<Wood>();

    [SerializeField]
    private BreakTree breakTree;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RemoveNoneWood();
    }

    public void WoodCreate(Vector2 Pos, Direction.DirectionState dir, int length)
    {
        Wood w = Instantiate(wood, Pos, Quaternion.identity);
        w.SetDirection(dir);
        w.SetLength(length);
        w.WoodChipsSet();
        woods.Add(w);
    }

    public void WoodBreak(int num)
    {
        if (num >= woods.Count)
            return;

        woods[num].Breaked();
    }

    private void RemoveNoneWood()
    {
        woods.RemoveAll(w => w.GetState() == 5);
    }

    public void SetWoodRootPoint(int num, Vector2 point)
    {
        woods[num].SetRootPoint(point);
    }

    public int GetWoodsLastNumber()
    {
        return woods.Count - 1;
    }

    public Direction.DirectionState GetWoodDirection(int num)
    {
        return woods[num].GetDirection();
    }

    public Wood GetIncludedPointWood(Vector2 point)
    {
        foreach (Wood w in woods)
        {
            if (w.IsIncludedMapPoint(point))
            {
                return w;
            }
        }
        return null;
    }

    public List<Wood> GetWoods()
    {
        return woods;
    }

    public void ChangeWoodsLayer()
    {
        //foreach(Wood w in woods)
        //{
        //    w.ChangeLayer()
        //}
    }

    public void CreateBreakWood(int lenght, Direction.DirectionState dir, Vector2 pos)
    {
        BreakTree bt = Instantiate(breakTree, pos, Quaternion.identity);
        bt.SetLenght(lenght);
        bt.SetSpinDirection(dir);
    }

    public void LastWoodsBornFromTree()
    {
        woods[woods.Count - 1].BornFromTree();
    }
}
