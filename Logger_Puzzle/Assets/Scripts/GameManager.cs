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
        WoodFallChack();
    }

    private void PlayerMoveUpdate()
    {
        if (playerManager.GetPlayerMoveMode() == 1)
        {
            if (mapManager.IsOnWood(playerManager.GetPlayerMapPoint()))//木の上に乗っていたなら
            {
                Debug.Log(playerManager.GetPlayerMapPoint());
                Wood nowOnWood = woodManager.GetIncludedPointWood(playerManager.GetPlayerMapPoint());
                if (!Direction.IsSameAxis(playerManager.GetPlayerDirection(), nowOnWood.GetDirection()))
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
                    if (!Direction.IsSameAxis(playerManager.GetPlayerDirection(), dw.GetDirection()))
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
                            Debug.Log("woodPoint:" + woodPoint);
                            Debug.Log("woodDistination:" + woodDistination);
                            mapManager.OnWood(woodDistination, dw.GetDirection());
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

    private void PlayerSlashUpdate()
    {
        if (playerManager.GetPlayerSlashMode() == 1)
        {
            FellTree();
            CrackWood();
        }
    }
    /// <summary>
    /// 木を切る
    /// </summary>
    private void FellTree()
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
                if (!mapManager.IsCanEnterWood(chackPoint))
                {
                    woodManager.WoodBreak(woodManager.GetWoodsLastNumber());
                    mapManager.Felling(targetTreePoint);
                    return;
                }
            }
            for (int i = 0; i < mapManager.GetTreeLength(targetTreePoint); i++)
            {
                Vector2 chackPoint = mapManager.GetFindPoint((int)woodCreatPoint.y, (int)woodCreatPoint.x, playerManager.GetPlayerDirection(), i);

                if (!mapManager.IsOnWood(chackPoint))
                {
                    mapManager.OnWood(chackPoint, woodManager.GetWoodDirection(woodManager.GetWoodsLastNumber()));
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

            mapManager.Felling(targetTreePoint);
        }
    }
    /// <summary>
    /// 丸太を割る
    /// </summary>
    private void CrackWood()
    {
        Vector2 pPoint = playerManager.GetPlayerMapPoint();
        Vector2 fPoint = mapManager.GetFindPoint((int)pPoint.y, (int)pPoint.x, playerManager.GetPlayerDirection(), 1);
        if (mapManager.IsOnWood(pPoint) && mapManager.IsOnWood(fPoint))
        {
            Wood w1 = woodManager.GetIncludedPointWood(pPoint);
            Wood w2 = woodManager.GetIncludedPointWood(fPoint);
            if (w1 == w2)
            {
                Direction.DirectionState pDir = playerManager.GetPlayerDirection();
                Direction.DirectionState wDir = w1.GetDirection();
                int l1;
                int l2;
                Vector2 rPoint = w1.GetRootPoint();
                if (pDir == wDir)
                {
                    if (pDir == Direction.DirectionState.Up || pDir == Direction.DirectionState.Down)
                    {
                        l1 = (int)Mathf.Abs(pPoint.y - rPoint.y) + 1;
                        l2 = w1.GetLength() - l1;
                    }
                    else
                    {
                        l1 = (int)Mathf.Abs(pPoint.x - rPoint.x) + 1;
                        l2 = w1.GetLength() - l1;
                    }
                }
                else
                {
                    if (pDir == Direction.DirectionState.Up || pDir == Direction.DirectionState.Down)
                    {
                        l2 = (int)Mathf.Abs(pPoint.y - rPoint.y);
                        l1 = w1.GetLength() - l2;
                    }
                    else
                    {
                        l2 = (int)Mathf.Abs(pPoint.x - rPoint.x);
                        l1 = w1.GetLength() - l2;
                    }
                }
                woodManager.WoodCreate(pPoint * mapManager.GetMapChipSize() * new Vector2(1, -1), Direction.GetReverseDirection(pDir), l1);//自マス
                woodManager.SetWoodRootPoint(woodManager.GetWoodsLastNumber(), pPoint);
                woodManager.WoodCreate(fPoint * mapManager.GetMapChipSize() * new Vector2(1, -1), pDir, l2);//前マス
                woodManager.SetWoodRootPoint(woodManager.GetWoodsLastNumber(), fPoint);
                w1.Crack();
            }
        }
    }

    private void WoodsChack()
    {
        WoodFallChack();
    }
    private void WoodFallChack()
    {
        foreach(Wood w in woodManager.GetWoods())
        {
            if(w.GetState()==0)
            {
                foreach(Vector2 p in w.GetMapPoints())
                {
                    if (!mapManager.IsHole(p))
                        return;
                }
                foreach (Vector2 p in w.GetMapPoints())
                {
                    mapManager.RemoveWood(p);
                }
                w.Fall();
            }
        }
    }
}
