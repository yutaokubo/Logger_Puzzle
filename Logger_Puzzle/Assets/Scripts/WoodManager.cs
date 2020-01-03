using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManager : MonoBehaviour
{
    [SerializeField]
    private Wood wood;

    private List<Wood> woods = new List<Wood>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WoodCreate(Vector2 Pos,Direction.DirectionState dir)
    {
        Wood w = Instantiate(wood, Pos, Quaternion.identity);
        w.SetDirection(dir);
        woods.Add(w);
    }

    public void WoodBreak(int num)
    {
        if (num >= woods.Count)
            return;

        woods[num].Breaked();
    }

    public void SetWoodRootPoint(int num,Vector2 point)
    {
        woods[num].SetRootPoint(point);
    }

    public int GetWoodsLastNumber()
    {
        return woods.Count-1;
    }
}
