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
        WoodsChack();
    }

    private void PlayerMoveUpdate()
    {
        if (playerManager.GetPlayerMoveMode() == 1)
        {
            if (mapManager.IsOnWood(playerManager.GetPlayerMapPoint()))//木の上に乗っていたなら
            {
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
                            //Debug.Log("woodPoint:" + woodPoint);
                            //Debug.Log("woodDistination:" + woodDistination);
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
        WoodFlowChack();
    }
    private void WoodFallChack()
    {
        foreach (Wood w in woodManager.GetWoods())
        {
            if (w.GetState() == 0)
            {
                bool isFall = true;
                foreach (Vector2 p in w.GetMapPoints())
                {
                    if (!mapManager.IsHole(p))
                    {
                        isFall = false;
                        break;
                    }
                }

                if (!isFall)
                    continue;

                foreach (Vector2 p in w.GetMapPoints())
                {
                    mapManager.RemoveWood(p);
                }
                w.Fall();
            }
        }
    }

    private void WoodFlowChack()
    {
        foreach (Wood w in woodManager.GetWoods())//丸太全てに処理する
        {
            if (w.GetState() == 0)//丸太が通常状態なら
            {
                bool isAllRiver = true;
                foreach (Vector2 p in w.GetMapPoints())//その丸太のある全てのマス
                {
                    if (!mapManager.IsRiver(p))//川マスに乗っていなければ
                        isAllRiver = false;
                    break;
                }
                if (!isAllRiver)//1マスでも川マスに乗っていなければ
                    continue;//この丸太の処理を終了

                Debug.Log("OnRiver");
                Direction.DirectionState distinationDir = Direction.DirectionState.None;//丸太の移動方向用
                foreach (Vector2 p in w.GetMapPoints())//全ての丸太のマスに対して
                {
                    //↓川の向いている方向に丸太の乗っているマスが無ければ
                    bool isNotDistination = w.IsIncludedMapPoint(mapManager.GetFindPoint((int)p.y, (int)p.x, mapManager.GetRiverDirection(p), 1));
                    if (!isNotDistination)
                    {
                        distinationDir = mapManager.GetRiverDirection(p);//丸太の進行方向決定
                    }
                }
                //ここから丸太を移動させる処理

                Debug.Log("FlowDir;" + distinationDir);
                Vector2[] woodDistainationPoints = new Vector2[w.GetLength()];//移動先のポイント
                Vector2[] woodPoints = w.GetMapPoints();//丸太の元にあった場所の記憶用
                bool isFlow = true;//流れるかどうか
                for (int i = 0; i < w.GetLength(); i++)//移動先のポイントを設定
                {
                    Vector2 woodDistination = mapManager.GetFindPoint((int)woodPoints[i].y, (int)woodPoints[i].x, distinationDir, 1);//丸太の1マスの移動先
                    woodDistainationPoints[i] = woodDistination;
                }

                foreach (Vector2 wp in woodPoints)//一度丸太のあるマスから丸太が無いことにする。
                {
                    mapManager.RemoveWood(wp);
                }
                foreach (Vector2 wDP in woodDistainationPoints)//その上で進めるかどうか確かめる
                {
                    if (!mapManager.IsCanEnterWood(wDP))
                    {
                        isFlow = false;
                        Debug.Log("FalseP:" + wDP);
                        break;
                    }
                }
                //Debug.Log("isFlow:" + isFlow);
                if (isFlow)//進めるなら
                {
                    foreach (Vector2 wDP in woodDistainationPoints)//進む先全てのマス目に
                    {
                        mapManager.OnWood(wDP, w.GetDirection());//丸太を乗せる
                    }
                    w.SetRootPoint(woodDistainationPoints[0]);//丸太にも現在位置を把握させる
                    w.MoveSet(distinationDir);
                }
                else//進めないなら
                {
                    foreach (Vector2 wp in woodPoints)//元のマス目に
                    {
                        mapManager.OnWood(wp, w.GetDirection());//丸太を乗せる
                    }
                    continue;//次の丸太に
                }
            }
        }
    }
}
