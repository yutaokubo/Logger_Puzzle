using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        //mapManager.SetPlayer(playerManager.GetPlayer());
        mapManager.MapCreate();
        playerManager.SetPlayerPosition(mapManager.GetPlayerStartPosition());//プレイヤーを初期位置に設定
        playerManager.SetPlayerMapPoint(mapManager.GetPlayerStartPoint());//マップ上でのプレイヤーの位置を渡す
        playerManager.SetPlayerMoveDistance(mapManager.GetMapChipSize());//プレイヤーの移動距離を設定
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMoveUpdate();
        PlayerSlashUpdate();
    }

    private void PlayerMoveUpdate()
    {
        if(playerManager.GetPlayerMoveMode()==1)
        {
            if(mapManager.IsPlayerEnterMapchip(playerManager.GetPlayerDirection(),playerManager.GetPlayerMapPoint()))
            {
                playerManager.PlayerMoveStart();
            }
            else
            {
                playerManager.PlayerStop();
            }
        }
    }

    private void PlayerSlashUpdate()
    {
        if(playerManager.GetPlayerSlashMode()==1)
        {
            playerManager.PlayerSlashStart();
            if (mapManager.IsGrowingTree(playerManager.GetPlayerDirectionRemotePoint(1)))
            {
                mapManager.Felling(playerManager.GetPlayerDirectionRemotePoint(1));
            }
        }
    }
}
