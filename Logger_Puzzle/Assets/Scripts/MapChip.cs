using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChip : MonoBehaviour
{
    private Vector2 mapPosition;//マップ上での位置

    [SerializeField]
    private float mapSizeX;
    [SerializeField]
    private float mapSizeY;


    enum MapChipType//マップチップのタイプ
    {
        Nomal,//通常、
        Rock,//通れない
        Hole,//穴
        Rever,//川
    }

    MapChipType nowMapChipType;//現在のマップチップのタイプ

    enum PlayerEnterType//プレイヤーがどのように侵入できるか
    {
        All,//どの方向でも侵入できる
        AutoOnlyAll,//プレイヤーが自動的に移動する時のみ全方向から侵入できる
        VerticalOnly,//縦方向からのみプレイヤーが侵入できる
        HorizontalOnly,//横方向からのみプレイヤーが侵入できる
        None,//プレイヤーが侵入できない
    }

    private PlayerEnterType nowPlayerEnterType;
    
    private bool isGrowingTree;//木が生えているか
    private bool isOnWood;//丸太が乗っているか
    private bool isCanWoodEnter;//丸太が侵入できるか

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
    public void SetMapPosition(Vector2 pos)
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
                nowPlayerEnterType = PlayerEnterType.All;
                isCanWoodEnter = true;
                break;
        }
    }

    public void Positioning()
    {
        transform.position = new Vector3(mapPosition.x * mapSizeX, mapPosition.y * -mapSizeY, 0);
    }
}
