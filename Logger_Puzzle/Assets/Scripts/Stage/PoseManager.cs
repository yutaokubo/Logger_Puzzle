using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseManager : MonoBehaviour
{

    [SerializeField]
    private GameObject pose;

    [SerializeField]
    private GameObject[] poseMenus;

    [SerializeField]
    private GameObject menuFrame;

    private int maxMenuNumber;
    private int nowMenuNumber;

    // Start is called before the first frame update
    void Start()
    {
        maxMenuNumber = poseMenus.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ポーズメニュー表示
    /// </summary>
    public void AppearPoseMenu()
    {
        pose.SetActive(true);
        FrameReset();
    }
    /// <summary>
    /// ポーズメニュー非表示
    /// </summary>
    public void DisappearPoseMenu()
    {
        FrameReset();
        pose.SetActive(false);
    }

    /// <summary>
    /// 選択位置初期化
    /// </summary>
    private void FrameReset()
    {
        nowMenuNumber = 0;
        menuFrame.transform.position = poseMenus[0].transform.position;
    }

    public void MenuSelect()
    {

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (nowMenuNumber > 0)
            {
                nowMenuNumber--;
            }
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (nowMenuNumber < maxMenuNumber)
            {
                nowMenuNumber++;
            }
        }

        menuFrame.transform.position = poseMenus[nowMenuNumber].transform.position;
    }

    public int GetNowMenuNumber()
    {
        return nowMenuNumber;
    }
}
