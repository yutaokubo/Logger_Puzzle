﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapManager : MonoBehaviour
{
    private int[,] mapNumbers;//各マップチップの番号

    private MapChip[,] mapChips;//マップチップ格納用
    
    public static int nowStageNumber = 1;//現在のステージ番号

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

    private Camera mainCamera;

    [SerializeField]
    private Grid grid;//グリッド線
    [SerializeField]
    private GameObject grids;//グリッド線一括用

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MapchipsAnimation()
    {
        foreach(MapChip m in mapChips)
        {
            m.MapchipAnimation();
        }
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
                GridCreat(t, i);
            }
        }
        isPlayerSettingStartPosision = false;

        SetLayer();
        CameraReset();
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
    /// グリッド線生成
    /// </summary>
    /// <param name="posX">X位置</param>
    /// <param name="posY">Y位置</param>
    private void GridCreat(int posX,int posY)
    {
        Grid g = Instantiate(grid);
        g.SetPostion(new Vector2(posX, -posY) * mapChipSize);
        g.transform.parent = grids.transform;
    }

    public void GridLineChange()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            grids.SetActive(!grids.activeSelf);
        }
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

    public bool IsPlayerEnterMapchip(Direction.DirectionState direction, Vector2 playerPoint)
    {
        if (direction == Direction.DirectionState.Up)//上方向の場合
        {
            if (playerPoint.y - 1 < 0)
            {
                return false;
            }
        }
        if (direction == Direction.DirectionState.Down)//下方向の場合
        {
            if (playerPoint.y + 1 >= mapChips.GetLength(0))
            {
                return false;
            }
        }
        if (direction == Direction.DirectionState.Right)//右方向の場合
        {
            if (playerPoint.x + 1 >= mapChips.GetLength(1))
            {
                return false;
            }
        }
        if (direction == Direction.DirectionState.Left)//左方向の場合
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
    /// <param name="point">指定ポイント</param>
    /// <returns></returns>
    public bool IsGrowingTree(Vector2 point)
    {
        return mapChips[(int)point.y, (int)point.x].IsGrowingTree();
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
    /// 指定ポイントの木の長さを取得
    /// </summary>
    /// <param name="point">位置</param>
    /// <returns></returns>
    public int GetTreeLength(Vector2 point)
    {
        return mapChips[(int)point.y, (int)point.x].GetTreeLength();
    }
    /// <summary>
    /// 指定したポイントに木が侵入できるかどうか
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsCanEnterWood(Vector2 point)
    {
        return mapChips[(int)point.y, (int)point.x].IsCanWoodEnter();
    }

    /// <summary>
    /// マップチップ群の、指定した位置から指定方向に指定距離離れたマップチップを取得
    /// </summary>
    /// <param name="height">元位置の高さ</param>
    /// <param name="width">元位置の幅</param>
    /// <param name="direction">方向</param>
    /// <param name="length">距離</param>
    /// <returns></returns>
    private MapChip GetFindtMapChips(int height, int width, Direction.DirectionState direction, int length)
    {
        if (direction == Direction.DirectionState.Up)
        {
            return mapChips[height - length, width];
        }
        if (direction == Direction.DirectionState.Down)
        {
            return mapChips[height + length, width];
        }
        if (direction == Direction.DirectionState.Right)
        {
            return mapChips[height, width + length];
        }
        if (direction == Direction.DirectionState.Left)
        {
            return mapChips[height, width - length];
        }
        return mapChips[height, width];
    }
    public Vector2 GetFindPoint(int height, int width, Direction.DirectionState direction, int length)
    {
        if (direction == Direction.DirectionState.Up)
        {
            return new Vector2(width, height - length);
        }
        if (direction == Direction.DirectionState.Down)
        {
            return new Vector2(width, height + length);
        }
        if (direction == Direction.DirectionState.Right)
        {
            return new Vector2(width + length, height);
        }
        if (direction == Direction.DirectionState.Left)
        {
            return new Vector2(width - length, height);
        }
        return new Vector2(width, height);
    }

    /// <summary>
    /// 指定したポイントに丸太を置く
    /// </summary>
    /// <param name="point"></param>
    public void OnWood(Vector2 point, Direction.DirectionState woodDir)
    {
        mapChips[(int)point.y, (int)point.x].OnWood(woodDir);
    }
    public void RemoveWood(Vector2 point)
    {
        mapChips[(int)point.y, (int)point.x].RemoveWood();
    }
    public bool IsOnWood(Vector2 point)
    {
        //Debug.Log(point);
        return mapChips[(int)point.y, (int)point.x].IsOnWood();
    }

    public bool IsHole(Vector2 point)
    {
        return mapChips[(int)point.y, (int)point.x].IsHole();
    }
    public bool IsRiver(Vector2 point)
    {
        return mapChips[(int)point.y, (int)point.x].IsRiver();
    }
    public Direction.DirectionState GetRiverDirection(Vector2 point)
    {
        return mapChips[(int)point.y, (int)point.x].GetRiverDirection();
    }

    private void SetLayer()
    {
        for (int i = 0; i < mapHeight; i++)
        {
            for (int t = 0; t < mapWidth; t++)
            {
                mapChips[i, t].ChangeLayer(i * 10 + 1);
                if(mapNumbers[i,t]==5)
                {
                    mapChips[i, t].ChangeLayer(i * 10 + 2);
                }
            }
        }
    }

    public bool IsGoal(Vector2 point)
    {
        if(mapNumbers[(int)point.y,(int)point.x] == 5)
        {
            return true;
        }
        return false;
    }

    public void CameraReset()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        CameraPositonReset();
        CameraZoomReset();
    }

    private void CameraPositonReset()
    {
        float cameraX = mapWidth * mapChipSize.x / 2;
        float cameraY = -mapHeight * mapChipSize.y / 2;

        mainCamera.transform.position = new Vector3(cameraX, cameraY, -10);
    }
    private void CameraZoomReset()
    {
        if (mapHeight <= 19 && mapHeight > 15)
        {
            mainCamera.orthographicSize = 7;
        }
        if (mapHeight <= 15 && mapHeight > 11)
        {
            mainCamera.orthographicSize = 5.5f;
        }
        if (mapHeight <= 11 && mapHeight > 7)
        {
            mainCamera.orthographicSize = 4;
        }
        if (mapHeight <= 7)
        {
            mainCamera.orthographicSize = 3;
        }
        if(mapWidth>19)
        {
            mainCamera.orthographicSize = 5;
        }
    }
}
