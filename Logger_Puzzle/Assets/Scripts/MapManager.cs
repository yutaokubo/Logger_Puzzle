﻿using System.Collections;
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
    //private int[] playerStartPoint = new int[2];//プレイヤースタート位置番号保存用
    private Vector2 playerStartPoint;
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
            //playerStartPoint[0] = posY;
            //playerStartPoint[1] = posX;
            playerStartPoint = new Vector2(posX, posY);
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
    public Vector2 GetPlayerStartPoint()
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

    public bool IsPlayerEnterMapchip(int direction, Vector2 playerPoint)
    {
        if (direction == 0)//上方向の場合
        {
            if (playerPoint.y - 1 < 0)
            {
                return false;
            }
        }
        if (direction == 1)//下方向の場合
        {
            if (playerPoint.y + 1 >= mapChips.GetLength(0))
            {
                return false;
            }
        }
        if (direction == 2)//右方向の場合
        {
            if (playerPoint.x + 1 >= mapChips.GetLength(1))
            {
                return false;
            }
        }
        if (direction == 3)//左方向の場合
        {
            if (playerPoint.x - 1 < 0)
            {
                return false;
            }
        }
        return GetFindtMapChips((int)playerPoint.y, (int)playerPoint.x, direction, 1).IsCanPlayerMoveSelf(direction);
        
    }


    /// <summary>
    /// 指定されたポイントに木が生えているかどうか
    /// </summary>
    /// <param name="Point">指定ポイント</param>
    /// <returns></returns>
    public bool IsGrowingTree(Vector2 Point)
    {
        return mapChips[(int)Point.y, (int)Point.x].IsGrowingTree();
    }
    /// <summary>
    /// 指定したポイントの木を切る
    /// </summary>
    /// <param name="point"></param>
    public void Felling(Vector2 point)
    {
        mapChips[(int)point.y, (int)point.x].Felling();
    }

    /// <summary>
    /// マップチップ群の、指定した位置から指定方向に指定距離離れたマップチップを取得
    /// </summary>
    /// <param name="height">元位置の高さ</param>
    /// <param name="weight">元位置の幅</param>
    /// <param name="direction">方向</param>
    /// <param name="length">距離</param>
    /// <returns></returns>
    private MapChip GetFindtMapChips(int height,int weight,int direction,int length)
    {
        if(direction == 0)
        {
            return mapChips[height - length, weight];
        }
        if(direction == 1)
        {
            return mapChips[height + length, weight];
        }
        if (direction == 2)
        {
            return mapChips[height, weight+length];
        }
        if (direction == 3)
        {
            return mapChips[height, weight-length];
        }
        return mapChips[height, weight];
    }
}
