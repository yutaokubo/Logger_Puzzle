using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapManager : MonoBehaviour
{
    private int[,] mapNumbers;//各マップチップの番号

    private MapChip[,] mapChips;//マップチップ格納用

    private int nowStageNumber;//現在のステージ番号

    private int mapHeight;//マップの高さ
    private int mapWidth;//マップの幅

    [SerializeField]
    private MapChip mapChip;//マップチップ

    [SerializeField]
    private Vector2 mapChipSize;//マップチップ1つ分のサイズ

    [SerializeField]
    private MapReader mapReader;//マップ読み込み用

    private Vector3 playerStartPosition;//プレイヤースタート位置保存用
    private int[] playerStartPoint = new int[2];//プレイヤースタート位置番号保存用
    private bool isPlayerSettingStartPosision;//プレイヤーがスタート位置に設定されているか

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// マップ作成
    /// </summary>
    public void MapCreate()
    {
        isPlayerSettingStartPosision = false;

        mapReader.SetStageNumber(nowStageNumber);//ステージ番号設定
        mapReader.ReadCSV();//CSV読み込み
        mapWidth = mapReader.GetMapWidth();//幅取得
        mapHeight = mapReader.GetMapHeight();//高さ取得
        mapNumbers = new int[mapHeight, mapWidth];//マップチップ番号初期化
        mapChips = new MapChip[mapHeight, mapWidth];//マップチップ一覧初期化
        for (int i = 0; i < mapHeight; i++)
        {
            for (int t = 0; t < mapWidth; t++)
            {
                mapNumbers[i, t] = int.Parse(mapReader.GetMapNumber(i, t));
                MapChipCreate(t, i);
            }
        }
        isPlayerSettingStartPosision = false;
    }

    /// <summary>
    /// マップチップ生成
    /// </summary>
    /// <param name="posX">X位置</param>
    /// <param name="posY">Y位置</param>
    private void MapChipCreate(int posX, int posY)
    {
        MapChip mc = Instantiate(mapChip);
        mc.SetMapPosition(new Vector2(posX, posY));
        mc.SetMapChipType(mapNumbers[posY, posX]);
        mc.Positioning(mapChipSize);
        SetPlayerPosition(posX, posY);
        mapChips[posY, posX] = mc;
    }


    /// <summary>
    /// プレイヤーの初期位置を設定
    /// </summary>
    /// <param name="posX">横</param>
    /// <param name="posY">縦</param>
    private void SetPlayerPosition(int posX, int posY)
    {
        if (isPlayerSettingStartPosision)
            return;
        if ((mapNumbers[posY, posX] == 3))
        {
            playerStartPosition = new Vector3(posX * mapChipSize.x, -posY * mapChipSize.y, 0);
            playerStartPoint[0] = posY;
            playerStartPoint[1] = posX;
            isPlayerSettingStartPosision = true;
        }
    }

    /// <summary>
    /// プレイヤーのスタート位置を返す
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerStartPosition()
    {
        return playerStartPosition;
    }
    /// <summary>
    /// マップ上でのスタート位置を返す
    /// </summary>
    /// <returns></returns>
    public int[] GetPlayerStartPoint()
    {
        return playerStartPoint;
    }
    /// <summary>
    /// マップチップのサイズを返す
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMapChipSize()
    {
        return mapChipSize;
    }

    public bool IsPlayerEniterMapchip(int direction, int[] playerPoint)
    {
        if (direction == 0)//上方向の場合
        {
            if (playerPoint[0] - 1 < 0)
            {
                return false;
            }
            return mapChips[playerPoint[0] - 1, playerPoint[1]].IsCanPlayerMoveSelf(direction);
        }
        if (direction == 1)//下方向の場合
        {
            if (playerPoint[0] + 1 >= mapChips.GetLength(0))
            {
                return false;
            }
            return mapChips[playerPoint[0] + 1, playerPoint[1]].IsCanPlayerMoveSelf(direction);
        }
        if (direction == 2)//右方向の場合
        {
            if (playerPoint[1] + 1 >= mapChips.GetLength(1))
            {
                return false;
            }
            return mapChips[playerPoint[0], playerPoint[1] + 1].IsCanPlayerMoveSelf(direction);
        }
        if (direction == 3)//左方向の場合
        {
            if (playerPoint[1] - 1 < 0)
            {
                return false;
            }
            return mapChips[playerPoint[0], playerPoint[1] - 1].IsCanPlayerMoveSelf(direction);
        }

        return false;
    }
}
