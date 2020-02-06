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

    private List<BreakTree> breakTrees = new List<BreakTree>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void WoodsUpdate()
    {
        foreach(Wood w in woods)
        {
            w.WoodUpdate();
        }
        RemoveNoneWood();
    }
    public void BreakTreesUpdate()
    {
        foreach(BreakTree bt in breakTrees)
        {
            bt.BreakTreeUpdate();
        }
        RemoveNoneBreakTree();
    }

    public void WoodCreate(Vector2 Pos, Direction.DirectionState dir, int length)
    {
        Wood w = Instantiate(wood, Pos, Quaternion.identity);
        w.SetDirection(dir);
        w.SetLength(length);
        w.WoodChipsSet();
        //w.ChangeLayer();
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
    private void RemoveNoneBreakTree()
    {
        breakTrees.RemoveAll(bt => bt.IsEnd());
    }

    public void SetWoodRootPoint(int num, Vector2 point)
    {
        woods[num].SetRootPoint(point);
        woods[num].ChangeLayer();
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
        foreach (Wood w in woods)
        {
            w.ChangeLayer();
        }
    }

    public void CreateBreakWood(int lenght, Direction.DirectionState dir, Vector2 pos,Vector2 point)
    {
        BreakTree bt = Instantiate(breakTree, pos, Quaternion.identity);
        bt.SetLenght(lenght);
        bt.SetSpinDirection(dir);
        bt.ChangeLayer((int)point.y+lenght);
        breakTrees.Add(bt);
    }

    public void LastWoodsBornFromTree()
    {
        woods[woods.Count - 1].BornFromTree();
    }
}
