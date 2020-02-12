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
        state = SceneState.CanSelect;
        GetMaxStageNumber();
        StageNumberTextChange();
        StageSelectPointsSet();
    }

    // Update is called once per frame
    void Update()
    {
        StageNumberChange();
        StageSelectMoveUpdate();
        SceneChangeChack();
    }

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

    private void StageNumberTextChange()
    {
        StageNumberLimiter();
        stageNumText.text = "Stage:" + MapManager.nowStageNumber;
    }

    private void GetMaxStageNumber()
    {
        string stageFilePath = Application.dataPath + "/Stage";
        maxStageNumber = Directory.GetFiles(stageFilePath, "*.csv").Length;
    }

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

    private Vector3 GetSelectPointPositionFromNum(int num)
    {
        Vector3 selectPointPosition;
        selectPointPosition = new Vector3((num - MapManager.nowStageNumber) * selectPointSpaceWidth, selectPointSpaceHeight, 0);
        return selectPointPosition;
    }

    private void StageSelectMoveStart()
    {
        int num = 0;
        foreach(StageSelectPoint sp in selectPoints)
        {
            num++;
            sp.MoveStart(GetSelectPointPositionFromNum(num), selectTime);
        }
    }

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

    private void SceneChangeChack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("StageScene");
    }
}
