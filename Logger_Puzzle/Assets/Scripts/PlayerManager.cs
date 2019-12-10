using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private Player player;//プレイヤー
    [SerializeField]
    //private int[] playerMapPoint = new int[2];
    private Vector2 playerMapPoint;

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
    public void SetPlayerMapPoint(Vector2 point)
    {
        playerMapPoint = point;
    }

    public Vector2 GetPlayerMapPoint()
    {
        return playerMapPoint;
    }

    /// <summary>
    /// プレイヤーの方向へ指定分だけ離れたポイントを返す
    /// </summary>
    /// <param name="length">距離</param>
    /// <returns></returns>
    public Vector2 GetPlayerDirectionRemotePoint(int length)
    {
        int playerDirection = player.GetDirection();
        if (playerDirection == 0)
        {
            return playerMapPoint + new Vector2(0, -length);
        }
        if (playerDirection == 1)
        {
            return playerMapPoint + new Vector2(0, length);
        }
        if (playerDirection == 2)
        {
            return playerMapPoint + new Vector2(length, 0);
        }
        if (playerDirection == 3)
        {
            return playerMapPoint + new Vector2(-length, 0);
        }
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
        if (player.GetDirection() == 0)
        {
            playerMapPoint.y -= 1;
        }
        if (player.GetDirection() == 1)
        {
            playerMapPoint.y += 1;
        }
        if (player.GetDirection() == 2)
        {
            playerMapPoint.x += 1;
        }
        if (player.GetDirection() == 3)
        {
            playerMapPoint.x -= 1;
        }

        player.SetMoveMode(2);
    }

    public int GetPlayerSlashMode()
    {
        return player.GetSlashMode();
    }

    public void PlayerSlashStart()
    {
        player.SetSlashMode(0);
    }
}
