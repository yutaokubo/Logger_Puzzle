using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class StageSelectManager : MonoBehaviour
{

    [SerializeField]
    private Text stageNumText;
    [SerializeField]
    private float selectTime;

    private int maxStageNumber;

    [SerializeField]
    FadeManager fadeManager;

    [SerializeField]
    private StageSelectPlayerIcon playerIcon;

    [SerializeField]
    private StageSelectPoint selectPoint;
    [SerializeField]
    private StageSelectPoint endSelectPoint;

    private StageSelectPoint[] selectPoints;

    [SerializeField]
    private float selectPointSpaceWidth;
    [SerializeField]
    private float selectPointSpaceHeight;

    private enum SceneState
    {
        FadeIn,//フェードイン
        CanSelect,//選択できる
        Selecting,//選択中
        FadeOut,//フェードアウト
    }

    private SceneState state;

    // Start is called before the first frame update
    void Start()
    {
        state = SceneState.FadeIn;
        MaxStageNumberSet();
        StageNumberTextChange();
        StageSelectPointsSet();
        fadeManager.FadeInStart();
    }

    // Update is called once per frame
    void Update()
    {
        FadeInUpdate();
        StageNumberChange();
        StageSelectMoveUpdate();
        SceneChangeChack();
        FadeOutUpdate();
    }

    /// <summary>
    /// ステージ番号を変更
    /// </summary>
    private void StageNumberChange()
    {
        if (state != SceneState.CanSelect)
            return;

        int currentStageNumber = MapManager.nowStageNumber;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MapManager.nowStageNumber -= 1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MapManager.nowStageNumber += 1;
        }
        StageNumberLimiter();
        StageNumberTextChange();
        if(currentStageNumber!= MapManager.nowStageNumber)
        {
            if (currentStageNumber < MapManager.nowStageNumber)
                playerIcon.RightWalkStart();
            if (currentStageNumber > MapManager.nowStageNumber)
                playerIcon.LeftWalkStart();

            state = SceneState.Selecting;
            StageSelectMoveStart();
        }
    }
    /// <summary>
    /// ステージ番号が制限を超えていたら修正
    /// </summary>
    private void StageNumberLimiter()
    {
        if (MapManager.nowStageNumber <= 0)
        {
            MapManager.nowStageNumber = 1;
        }
        if (MapManager.nowStageNumber > maxStageNumber)
        {
            MapManager.nowStageNumber = maxStageNumber;
        }
    }

    /// <summary>
    /// ステージ番号を表示するテキストを変更
    /// </summary>
    private void StageNumberTextChange()
    {
        StageNumberLimiter();
        stageNumText.text = "Stage:" + MapManager.nowStageNumber;
    }

    /// <summary>
    /// ステージファイルからステージ番号の最大値を取得
    /// </summary>
    private void MaxStageNumberSet()
    {
        string stageFilePath = Application.dataPath + "/Stage";
        maxStageNumber = Directory.GetFiles(stageFilePath, "*.csv").Length;
    }

    /// <summary>
    /// ステージ数に応じてステージのアイコンを作成
    /// </summary>
    private void StageSelectPointsSet()
    {
        selectPoints = new StageSelectPoint[maxStageNumber];
        for (int i = 0; i < maxStageNumber - 1; i++)
        {
            StageSelectPoint sp = Instantiate(selectPoint);
            sp.transform.position = GetSelectPointPositionFromNum(i + 1);
            selectPoints[i] = sp;
        }
        StageSelectPoint esp = Instantiate(endSelectPoint);
        esp.transform.position = GetSelectPointPositionFromNum(maxStageNumber);
        selectPoints[maxStageNumber - 1] = esp;
    }

    /// <summary>
    /// ステージアイコン番号と現在のステージ番号からステージアイコンの目的地を取得
    /// </summary>
    /// <param name="num">ステージアイコン番号</param>
    /// <returns></returns>
    private Vector3 GetSelectPointPositionFromNum(int num)
    {
        Vector3 selectPointPosition;
        selectPointPosition = new Vector3((num - MapManager.nowStageNumber) * selectPointSpaceWidth, selectPointSpaceHeight, 0);
        return selectPointPosition;
    }

    /// <summary>
    /// ステージ選択移動開始
    /// </summary>
    private void StageSelectMoveStart()
    {
        int num = 0;
        foreach(StageSelectPoint sp in selectPoints)
        {
            num++;
            sp.MoveStart(GetSelectPointPositionFromNum(num), selectTime);
        }
    }
    /// <summary>
    /// ステージ選択移動中
    /// </summary>
    private void StageSelectMoveUpdate()
    {
        if (state != SceneState.Selecting)
            return;
        foreach(StageSelectPoint sp in selectPoints)
        {
            if(sp.GetState()==1)
            {
                return;
            }
        }
        state = SceneState.CanSelect;
        playerIcon.Stop();
    }

    private void FadeInUpdate()
    {
        if (state != SceneState.FadeIn)
            return;

        if(fadeManager.IsFadeEnd())
        {
            state = SceneState.CanSelect;
        }
    }
    private void FadeOutUpdate()
    {
        if (state != SceneState.FadeOut)
            return;

        if(fadeManager.IsFadeEnd())
        {
            LoadNextScene();
        }
    }

    private void SceneChangeChack()
    {
        if (state != SceneState.CanSelect)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = SceneState.FadeOut;
            fadeManager.FadeOutStart();
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("StageScene");
    }
}
