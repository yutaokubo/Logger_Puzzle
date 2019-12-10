using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChip : MonoBehaviour
{
    private Vector2 mapPosition;//マップ上での位置

    private float mapSizeX;
    private float mapSizeY;

    private MapChipSpriteSelecter spriteSelecter;

    enum MapChipSprite//マップチップの絵
    {
        Nomal,//通常の草原
        Rock,//岩
        Tree,//木
    }

    private MapChipSprite nowSprite;//画像変更用
    private SpriteRenderer renderer;//画像変更


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
    [SerializeField]
    private bool isGrowingTree;//木が生えているか
    private bool isOnWood;//丸太が乗っているか
    private bool isCanWoodEnter;//丸太が侵入できるか

    // Start is called before the first frame update
    void Start()
    {
        MapChipSelecterSetting();
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
        MapChipSelecterSetting();
        switch (num)
        {
            case 0://通常地形
                nowMapChipType = MapChipType.Nomal;
                nowPlayerEnterType = PlayerEnterType.All;
                isCanWoodEnter = true;
                nowSprite = MapChipSprite.Nomal;
                ChangeSprite();
                break;

            case 1://壁用
                nowMapChipType = MapChipType.Rock;
                nowPlayerEnterType = PlayerEnterType.None;
                isCanWoodEnter = false;
                nowSprite = MapChipSprite.Rock;
                ChangeSprite();
                break;

            case 3://スタート位置
                nowMapChipType = MapChipType.Nomal;
                nowPlayerEnterType = PlayerEnterType.All;
                isCanWoodEnter = true;
                nowSprite = MapChipSprite.Nomal;
                ChangeSprite();
                break;

            case 11://木一つ分
                SetTree(1);
                break;
        }
    }
    /// <summary>
    /// 指定本数の木のマップチップを作成
    /// </summary>
    /// <param name="length"></param>
    private void SetTree(int length)
    {
        nowMapChipType = MapChipType.Rock;
        nowPlayerEnterType = PlayerEnterType.None;
        isGrowingTree = true;
        isCanWoodEnter = false;
        nowSprite = MapChipSprite.Tree;
        ChangeSprite();
    }

    /// <summary>
    /// 自分の位置に移動
    /// </summary>
    public void Positioning(Vector2 mapSize)
    {
        mapSizeX = mapSize.x;
        mapSizeY = mapSize.y;

        transform.position = new Vector3(mapPosition.x * mapSizeX, mapPosition.y * -mapSizeY, 0);
    }

    /// <summary>
    /// 画像を変更
    /// </summary>
    private void ChangeSprite()
    {
        renderer.sprite = spriteSelecter.GetMapChipSprite((int)nowSprite);
    }
    /// <summary>
    /// スプライトを変える準備ができていなければ準備
    /// </summary>
    private void MapChipSelecterSetting()
    {
        if (renderer == null)
        {
            renderer = gameObject.GetComponent<SpriteRenderer>();
            spriteSelecter = gameObject.GetComponent<MapChipSpriteSelecter>();
        }
    }

    /// <summary>
    /// プレイヤーの移動方向を取得し通過できるかを返す
    /// </summary>
    /// <param name="direction">プレイヤーの方向</param>
    /// <returns></returns>
    public bool IsCanPlayerMoveSelf(Direction.DirectionState direction)
    {
        if (nowPlayerEnterType == PlayerEnterType.None)
        {
            return false;
        }

        if (nowPlayerEnterType == PlayerEnterType.All)
        {
            return true;
        }

        if (nowPlayerEnterType == PlayerEnterType.VerticalOnly)
        {
            if (direction == Direction.DirectionState.Up ||
                direction == Direction.DirectionState.Down)
            {
                return true;
            }
        }
        if (nowPlayerEnterType == PlayerEnterType.HorizontalOnly)
        {
            if (direction == Direction.DirectionState.Right ||
                direction == Direction.DirectionState.Left)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 木が生えているか
    /// </summary>
    /// <returns></returns>
    public bool IsGrowingTree()
    {
        return isGrowingTree;
    }
    /// <summary>
    /// 木のマップチップを通常地形に
    /// </summary>
    public void Felling()
    {
        isGrowingTree = false;
        nowMapChipType = MapChipType.Nomal;
        nowPlayerEnterType = PlayerEnterType.All;
        isCanWoodEnter = true;
        nowSprite = MapChipSprite.Nomal;
        ChangeSprite();
    }
}
