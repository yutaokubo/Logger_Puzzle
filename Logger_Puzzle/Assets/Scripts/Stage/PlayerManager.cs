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
        //ChangePlayerLayer();
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
        Direction.DirectionState playerDirection = player.GetDirection();
        if (playerDirection == Direction.DirectionState.Up)
        {
            return playerMapPoint + new Vector2(0, -length);
        }
        if (playerDirection == Direction.DirectionState.Down)
        {
            return playerMapPoint + new Vector2(0, length);
        }
        if (playerDirection == Direction.DirectionState.Right)
        {
            return playerMapPoint + new Vector2(length, 0);
        }
        if (playerDirection == Direction.DirectionState.Left)
        {
            return playerMapPoint + new Vector2(-length, 0);
        }
        return playerMapPoint;
    }

    
    public int GetPlayerMode()
    {
        return player.GetPlayerMode();
    }
    public void PlayerStop()
    {
        player.SetPlayerMode(0);
        player.SetTargetPosition(player.transform.position);
    }

    public Direction.DirectionState GetPlayerDirection()
    {
        return player.GetDirection();
    }

    /// <summary>
    /// プレイヤー移動開始
    /// </summary>
    public void PlayerMoveStart()
    {
        if (player.GetDirection() == Direction.DirectionState.Up)
        {
            playerMapPoint.y -= 1;
        }
        if (player.GetDirection() == Direction.DirectionState.Down)
        {
            playerMapPoint.y += 1;
        }
        if (player.GetDirection() == Direction.DirectionState.Right)
        {
            playerMapPoint.x += 1;
        }
        if (player.GetDirection() == Direction.DirectionState.Left)
        {
            playerMapPoint.x -= 1;
        }
        
        player.SetPlayerMode(2);
    }

    public void PlayerAutoMoveStart(Direction.DirectionState moveDir)
    {
        player.AutoMoveStart(moveDir);
    }
    public void ForciblyPlayerAutoMoveStart()
    {
        player.SetPlayerMode(4);
        player.SetTargetPosition(new Vector3(playerMapPoint.x * 0.64f, playerMapPoint.y * -0.64f, 0));
    }

    public void PlayerSlashStart()
    {
        player.SetPlayerMode(6);
    }

    public void PlayerFall()
    {
        player.Fall();
    }

    public void ChangePlayerLayer()
    {
        player.ChangeLayer((int)(playerMapPoint.y * 10 + 5));
    }
    public void SetPlayerLayer(int num)
    {
        player.ChangeLayer((int)(num * 10 + 5));
    }
}
