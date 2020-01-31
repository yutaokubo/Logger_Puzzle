using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapReader : MonoBehaviour
{
    TextAsset csvFile;//csvファイル
    List<string[]> csvDatas = new List<string[]>();//csv格納用

    private int createStageNumber;//作成するステージのナンバー


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// CSV読み込み
    /// </summary>
    public void ReadCSV()
    {
        csvDatas.Clear();//中身消去

        //csvFile = Resources.Load("stage") as TextAsset;
        //StringReader reader = new StringReader(csvFile.text);

        //while (reader.Peek() != -1)
        //{
        //    string line = reader.ReadLine();
        //    csvDatas.Add(line.Split(','));
        //}
        
        string[] textLines = File.ReadAllLines(GetStagePath());
        foreach (string t in textLines)
        {
            csvDatas.Add(t.Split(','));
        }
    }

    //ステージパス取得
    private string GetStagePath()
    {
        string stageNumber = "001";
        if (createStageNumber < 10)
        {
            stageNumber = "00" + createStageNumber;
        }
        else if (createStageNumber < 100)
        {
            stageNumber = "0" + createStageNumber;
        }
        Debug.Log(createStageNumber);
        Debug.Log(stageNumber);
        string stagePath = Application.dataPath + "/Stage/" + stageNumber + ".csv";
        //string stagePath = Application.dataPath + "/Stage/001.csv";

        return stagePath;
    }

    /// <summary>
    /// ステージ番号を取得
    /// </summary>
    /// <param name="nowStageNum">現在のステージ番号</param>
    public void SetStageNumber(int nowStageNum)
    {
        createStageNumber = nowStageNum;
    }

    /// <summary>
    /// 指定された場所の数値を返す
    /// </summary>
    /// <param name="height">縦幅</param>
    /// <param name="width">横幅</param>
    /// <returns>マップチップ番号</returns>
    public string GetMapNumber(int height, int width)
    {
        return csvDatas[height][width];
    }

    public int GetMapHeight()
    {
        return csvDatas.Count;
    }

    public int GetMapWidth()
    {
        return csvDatas[0].Length;
    }
}
