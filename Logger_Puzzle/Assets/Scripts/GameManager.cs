using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private WoodManager woodManager;

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
            Vector2 targetTreePoint = playerManager.GetPlayerDirectionRemotePoint(1);
            playerManager.PlayerSlashStart();
            if (mapManager.IsGrowingTree(targetTreePoint))
            {
                FellTree(targetTreePoint);

                Vector2 woodCreatPoint = playerManager.GetPlayerDirectionRemotePoint(2);
                Vector2 woodCreatPostion = woodCreatPoint * mapManager.GetMapChipSize();
                woodCreatPostion.y *= -1;
                woodManager.WoodCreate(woodCreatPostion,playerManager.GetPlayerDirection());
                woodManager.SetWoodRootPoint(woodManager.GetWoodsLastNumber(), woodCreatPoint);
                if(!mapManager.IsCanEnterWood(woodCreatPoint))
                {
                    woodManager.WoodBreak(woodManager.GetWoodsLastNumber());
                }
                else
                {
                    if(!mapManager.IsOnWood(woodCreatPoint))
                    {
                        mapManager.OnWood(woodCreatPoint);
                    }
                }
            }
        }
    }
    private void FellTree(Vector2 mapPoint)
    {
        mapManager.Felling(mapPoint);
    }
}
