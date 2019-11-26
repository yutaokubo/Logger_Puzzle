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
    private MapReader mapReader;//マップ読み込み用

    // Start is called before the first frame update
    void Start()
    {
        MapCreate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// マップ作成
    /// </summary>
    private void MapCreate()
    {
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
                Debug.Log(mapReader.GetMapNumber(i, t));
                mapNumbers[i, t] = int.Parse(mapReader.GetMapNumber(i, t));
                MapChipCreate(t, i);
            }
        }
    }


    private void MapChipCreate(int posX,int posY)
    {
        MapChip mc = Instantiate(mapChip);
        mc.SetMapPosition(new Vector2(posX, posY));
        mc.SetMapChipType(mapNumbers[posY, posX]);
        mc.Positioning();
        mapChips[posX, posY] = mc;
    }
}
