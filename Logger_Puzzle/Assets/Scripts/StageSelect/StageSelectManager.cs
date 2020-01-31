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

    private int maxStageNumber;

    // Start is called before the first frame update
    void Start()
    {
        GetMaxStageNumber();
        StageNumberTextChange();
    }

    // Update is called once per frame
    void Update()
    {
        StageNumberChange();
        SceneChangeChack();
    }

    private void StageNumberChange()
    {

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
        maxStageNumber = Directory.GetFiles(stageFilePath,"*.csv").Length;
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
