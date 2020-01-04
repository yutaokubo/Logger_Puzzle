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
        if (playerManager.GetPlayerMoveMode() == 1)
        {
            if (mapManager.IsOnWood(playerManager.GetPlayerMapPoint()))//木の上に乗っていたなら
            {
                Debug.Log(playerManager.GetPlayerMapPoint());
                Wood nowOnWood = woodManager.GetIncludedPointWood(playerManager.GetPlayerMapPoint());
                if (!IsPlayerAxisSameWoodAxis(playerManager.GetPlayerDirection(), nowOnWood.GetDirection()))
                {
                    playerManager.PlayerStop();
                    return;
                }
            }

            if (mapManager.IsPlayerEnterMapchip(playerManager.GetPlayerDirection(), playerManager.GetPlayerMapPoint()))
            {
                Vector2 playerDestination = mapManager.GetFindPoint((int)playerManager.GetPlayerMapPoint().y,
                                                                    (int)playerManager.GetPlayerMapPoint().x,
                                                                    playerManager.GetPlayerDirection(), 1);
                if (mapManager.IsOnWood(playerDestination))
                {
                    //Debug.Log("PlayerDestinationOnWood");
                    Wood dw = woodManager.GetIncludedPointWood(playerDestination);
                    //Debug.Log(dw);
                    if (!IsPlayerAxisSameWoodAxis(playerManager.GetPlayerDirection(), dw.GetDirection()))
                    {
                        for (int i = 0; i < dw.GetLength(); i++)
                        {
                            Vector2 woodPoint = mapManager.GetFindPoint((int)dw.GetRootPoint().y, (int)dw.GetRootPoint().x, dw.GetDirection(), i);
                            Vector2 woodDistination = mapManager.GetFindPoint((int)woodPoint.y, (int)woodPoint.x, playerManager.GetPlayerDirection(), 1);
                            if (!mapManager.IsCanEnterWood(woodDistination))
                            {
                                playerManager.PlayerStop();
                                Debug.Log("Return");
                                return;
                            }
                        }
                        dw.MoveSet(playerManager.GetPlayerDirection());
                        Vector2 dwRootPoint = dw.GetRootPoint();
                        for (int i = 0; i < dw.GetLength(); i++)
                        {
                            Vector2 woodPoint = mapManager.GetFindPoint((int)dwRootPoint.y, (int)dwRootPoint.x, dw.GetDirection(), i);
                            Vector2 woodDistination = mapManager.GetFindPoint((int)woodPoint.y, (int)woodPoint.x, playerManager.GetPlayerDirection(), 1);
                            dw.ChangeMapPoints(i, woodDistination);
                            mapManager.RemoveWood(woodPoint);
                            Debug.Log("woodPoint:"+woodPoint);
                            Debug.Log("woodDistination:" + woodDistination);
                            mapManager.OnWood(woodDistination);
                        }
                    }
                }
                playerManager.PlayerMoveStart();
            }
            else
            {
                playerManager.PlayerStop();
            }
        }
    }

    private bool IsPlayerAxisSameWoodAxis(Direction.DirectionState playerDirection, Direction.DirectionState woodDirection)
    {
        if ((playerDirection == Direction.DirectionState.Up || playerDirection == Direction.DirectionState.Down)
            && (woodDirection == Direction.DirectionState.Up || woodDirection == Direction.DirectionState.Down))
        {
            return true;
        }
        if ((playerDirection == Direction.DirectionState.Right || playerDirection == Direction.DirectionState.Left)
            && (woodDirection == Direction.DirectionState.Right || woodDirection == Direction.DirectionState.Left))
        {
            return true;
        }
        return false;
    }

    private void PlayerSlashUpdate()
    {
        if (playerManager.GetPlayerSlashMode() == 1)
        {
            Vector2 targetTreePoint = playerManager.GetPlayerDirectionRemotePoint(1);
            playerManager.PlayerSlashStart();
            if (mapManager.IsGrowingTree(targetTreePoint))
            {

                Vector2 woodCreatPoint = playerManager.GetPlayerDirectionRemotePoint(2);
                Vector2 woodCreatPostion = woodCreatPoint * mapManager.GetMapChipSize();
                woodCreatPostion.y *= -1;
                woodManager.WoodCreate(woodCreatPostion, playerManager.GetPlayerDirection(), mapManager.GetTreeLength(targetTreePoint));
                woodManager.SetWoodRootPoint(woodManager.GetWoodsLastNumber(), woodCreatPoint);

                for (int i = 0; i < mapManager.GetTreeLength(targetTreePoint); i++)
                {
                    Vector2 chackPoint = mapManager.GetFindPoint((int)woodCreatPoint.y, (int)woodCreatPoint.x, playerManager.GetPlayerDirection(), i);
                    if(!mapManager.IsCanEnterWood(chackPoint))
                    {
                        woodManager.WoodBreak(woodManager.GetWoodsLastNumber());
                        FellTree(targetTreePoint);
                        return;
                    }
                }
                for(int i = 0; i < mapManager.GetTreeLength(targetTreePoint); i++)
                {
                    Vector2 chackPoint = mapManager.GetFindPoint((int)woodCreatPoint.y, (int)woodCreatPoint.x, playerManager.GetPlayerDirection(), i);

                    if (!mapManager.IsOnWood(chackPoint))
                    {
                        mapManager.OnWood(chackPoint);
                    }
                }

                //if (!mapManager.IsCanEnterWood(woodCreatPoint))
                //{
                //    woodManager.WoodBreak(woodManager.GetWoodsLastNumber());
                //}
                //else
                //{
                //    if (!mapManager.IsOnWood(woodCreatPoint))
                //    {
                //        mapManager.OnWood(woodCreatPoint);
                //    }
                //}

                FellTree(targetTreePoint);
            }
        }
    }
    private void FellTree(Vector2 mapPoint)
    {
        mapManager.Felling(mapPoint);
    }
}
