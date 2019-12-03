using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private Player player;//プレイヤー
    [SerializeField]
    private int[] playerMapPoint = new int[2]; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// プレイヤーの位置を変更、現在の移動ターゲットもその位置へ
    /// </summary>
    /// <param name="pos">位置</param>
    public void SetPlayerPosition(Vector3 pos)
    {
        player.transform.position = pos;
        player.SetTargetPosition(pos);
    }

    /// <summary>
    /// プレイヤーの移動量設定
    /// </summary>
    /// <param name="distance">移動量</param>
    public void SetPlayerMoveDistance(Vector2 distance)
    {
        player.SetMoveDistance(distance);
    }

    /// <summary>
    /// プレイヤーのスタート位置がどのマス目か設定
    /// </summary>
    /// <param name="mapPoint"></param>
    public void SetPlayerMapPoint(int[] mapPoint)
    {
        playerMapPoint[0] = mapPoint[0];
        playerMapPoint[1] = mapPoint[1];
    }

    public int[] GetPlayerMapPoint()
    {
        return playerMapPoint;
    }

    /// <summary>
    /// プレイヤーの移動状態を取得
    /// </summary>
    /// <returns></returns>
    public int GetPlayerMoveMode()
    {
        return player.GetMoveMode();
    }
    public void PlayerStop()
    {
        player.SetMoveMode(0);
        player.SetTargetPosition(player.transform.position);
    }

    public int GetPlayerDirection()
    {
        return player.GetDirection();
    }

    /// <summary>
    /// プレイヤー移動開始
    /// </summary>
    public void PlayerMoveStart()
    {
        if(player.GetDirection() == 0)
        {
            playerMapPoint[0] -= 1;
        }
        if (player.GetDirection() == 1)
        {
            playerMapPoint[0] += 1;
        }
        if (player.GetDirection() == 2)
        {
            playerMapPoint[1] += 1;
        }
        if (player.GetDirection() == 3)
        {
            playerMapPoint[1] -= 1;
        }

        player.SetMoveMode(2);
    }
}
