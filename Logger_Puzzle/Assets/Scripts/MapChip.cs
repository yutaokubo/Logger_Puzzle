using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChip : MonoBehaviour
{
    private int[,] mapPosition;//マップ上での位置


    enum MapChipType//マップチップのタイプ
    {
        Nomal,//通常、
        Rock,//通れない
        Hole,//穴
        Rever,//川
    }

    MapChipType nowMapChipType;//現在のマップチップのタイプ

    private bool isCanPlayerEnter;//プレイヤーが意図的に侵入できるか
    private bool isCanPlayerAutoEnter;//プレイヤーが自動的に侵入できるか
    private bool isGrowingTree;//木が生えているか
    private bool isOnWood;//丸太が乗っているか
    private bool isCanWoodEnter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// どこのマス目か設定
    /// </summary>
    /// <param name="pos">マップ上の位置</param>
    public void SetMapPosition(int[,] pos)
    {
        mapPosition = pos;
    }

    /// <summary>
    /// 番号から状態を変化させる
    /// </summary>
    /// <param name="num"></param>
    public void SetMapChipType(int num)
    {
        switch(num)
        {
            case 0://通常地形
                nowMapChipType = MapChipType.Nomal;
                isCanPlayerEnter = true;
                isCanPlayerAutoEnter = true;
                isCanWoodEnter = true;
                break;
        }
    }
}
