﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private WoodManager woodManager;
    [SerializeField]
    private FadeManager fadeManager;
    [SerializeField]
    private PoseManager poseManager;

    private bool IsClear;

    private enum GameState
    {
        StartFadeIn,//フェードイン中
        StartDirect,//開始演出中
        Nomal,//通常時、プレイヤープレイ中
        Pose,//ポーズ
        Reset,//リセット
        PlayerFalling,//プレイヤーが落ちた
        PlayerFalledFadeOut,//プレイヤーが落ちた後のフェードアウト
        BackToStageSelectFadeOut,//ステージ選択へ戻るフェードアウト
        EndFadeOut,//フェードアウト中
        End,//シーン終了
    }
    [SerializeField]
    private GameState gameState;
    private GameState currentState;
    

    [SerializeField]
    private float cameraSpeed;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.StartFadeIn;
        //mapManager.SetPlayer(playerManager.GetPlayer());
        mapManager.MapCreate();
        playerManager.SetPlayerPosition(mapManager.GetPlayerStartPosition());//プレイヤーを初期位置に設定
        playerManager.SetPlayerMapPoint(mapManager.GetPlayerStartPoint());//マップ上でのプレイヤーの位置を渡す
        playerManager.ChangePlayerLayer();//プレイヤーのレイヤーを変更
        playerManager.SetPlayerMoveDistance(mapManager.GetMapChipSize());//プレイヤーの移動距離を設定
        playerManager.PlayerStartingDirectSet();
        fadeManager.FadeInStart();
        IsClear = false;//まだクリアしてない
    }

    // Update is called once per frame
    void Update()
    {
        StartFadeInUpdate();
        StartDirectUpdate();
        StagePlayUpdate();
        PoseUpdate();
        StageResetUpdate();
        PlayerFallingUpdate();
        PlayerFalledFadeOutUpdate();
        BackToStageSelectUpdate();
        EndFadeOutUpdate();
        EndUpdate();


        currentState = gameState;
    }

    /// <summary>
    /// フェードイン中
    /// </summary>
    private void StartFadeInUpdate()
    {
        if (gameState != GameState.StartFadeIn)
            return;

        if(fadeManager.IsFadeEnd())
        {
            gameState = GameState.StartDirect;
            playerManager.PlayerStartingDirectStart();
        }
        mapManager.MapchipsAnimation();
    }

    /// <summary>
    /// スタート演出中
    /// </summary>
    public void StartDirectUpdate()
    {
        if (gameState != GameState.StartDirect)
            return;

        playerManager.PlayerUpdate();
        mapManager.MapchipsAnimation();

        if (playerManager.GetPlayerMode() == 0)
        {
            gameState = GameState.Nomal;
        }
    }
    /// <summary>
    /// 通常プレイ中
    /// </summary>
    public void StagePlayUpdate()
    {
        if (gameState != GameState.Nomal)
            return;

        playerManager.PlayerUpdate();
        woodManager.WoodsUpdate();
        woodManager.BreakTreesUpdate();
        mapManager.MapchipsAnimation();

        WoodsChack();
        PlayerFallChack();
        PlayerSlashUpdate();
        PlayerMoveUpdate();
        PlayerGoalChack();
        GameClear();
        Pose();
        StageResetStart();
        GridLineChange();
        DebugCameraMove();
        //DebugSceneReload();
    }
    /// <summary>
    /// ポーズ中
    /// </summary>
    private void PoseUpdate()
    {
        if (gameState != GameState.Pose)
            return;

        poseManager.MenuSelect();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            switch(poseManager.GetNowMenuNumber())
            {
                case 0:
                    gameState = GameState.Nomal;
                    playerManager.PlayerAnimationRestart();
                    poseManager.DisappearPoseMenu();
                    return;

                case 1:
                    gameState = GameState.BackToStageSelectFadeOut;
                    fadeManager.FadeOutStart();
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.P) && currentState == GameState.Pose)
        {
            gameState = GameState.Nomal;
            playerManager.PlayerAnimationRestart();
            //poseMenu.SetActive(false);
            poseManager.DisappearPoseMenu();
        }
    }
    /// <summary>
    /// ステージリセット中
    /// </summary>
    private void StageResetUpdate()
    {
        if (gameState != GameState.Reset)
            return;

        playerManager.PlayerUpdate();
        woodManager.WoodsUpdate();
        woodManager.BreakTreesUpdate();
        mapManager.MapchipsAnimation();

        if(fadeManager.IsFadeEnd())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    /// <summary>
    /// プレイヤーが落ちた時
    /// </summary>
    private void PlayerFallingUpdate()
    {
        if (gameState != GameState.PlayerFalling)
            return;

        playerManager.PlayerUpdate();
        woodManager.WoodsUpdate();
        woodManager.BreakTreesUpdate();
        mapManager.MapchipsAnimation();
        if(playerManager.GetPlayerMode()==8)
        {
            fadeManager.FadeOutStart();
            gameState = GameState.PlayerFalledFadeOut;
        }
    }
    /// <summary>
    /// プレイヤーが落ちた後のフェードアウト中
    /// </summary>
    private void PlayerFalledFadeOutUpdate()
    {
        if (gameState != GameState.PlayerFalledFadeOut)
            return;
        if(fadeManager.IsFadeEnd())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    /// <summary>
    /// シーン終了時フェードアウト中
    /// </summary>
    private void EndFadeOutUpdate()
    {
        if (gameState != GameState.EndFadeOut)
            return;
        if(fadeManager.IsFadeEnd())
        {
            gameState = GameState.End;
        }
    }
    private void BackToStageSelectUpdate()
    {
        if (gameState != GameState.BackToStageSelectFadeOut)
            return;
        if(fadeManager.IsFadeEnd())
        {
            SceneManager.LoadScene("StageSelectScene");
        }
    }
    /// <summary>
    /// 終了中
    /// </summary>
    private void EndUpdate()
    {
        if (gameState != GameState.End)
            return;

        Invoke("LoadNextScene", 0.5f);
    }

    private void PlayerMoveUpdate()
    {
        if (playerManager.GetPlayerMode() == 1)//プレイヤーが移動待機状態なら
        {
            Vector2 playerDestination = mapManager.GetFindPoint((int)playerManager.GetPlayerMapPoint().y,
                                                                (int)playerManager.GetPlayerMapPoint().x,
                                                                playerManager.GetPlayerDirection(), 1);//プレイヤーの移動先のポイントを取得
            Wood nowPointWood = null;//現在地の木
            Wood destinationPointWood = null;//移動先の木
            if (mapManager.IsOnWood(playerManager.GetPlayerMapPoint()))//プレイヤーが木に乗っていたら
            {
                nowPointWood = woodManager.GetIncludedPointWood(playerManager.GetPlayerMapPoint());//乗っている木を取得
            }
            if (mapManager.IsOnWood(playerDestination))//移動先に木があるなら
            {
                destinationPointWood = woodManager.GetIncludedPointWood(playerDestination);//移動先の木を取得
            }



            //ここからそもそも移動が出来る状態か判断


            if (!mapManager.IsOnWood(playerManager.GetPlayerMapPoint()))//プレイヤーが丸太に乗っていない時
            {
                if (mapManager.IsOnWood(playerDestination) && mapManager.IsRiver(playerDestination))//移動先が丸太かつ川なら
                {
                    if(destinationPointWood.GetState()==3)//丸太が流れているなら
                    {
                        playerManager.PlayerStop();//プレイヤーを止めて
                        return;//移動しない
                    }

                    if (!Direction.IsSameAxis(playerManager.GetPlayerDirection(), destinationPointWood.GetDirection()))//方向軸が違うなら
                    {
                        foreach (Vector2 dwp in destinationPointWood.GetMapPoints())//その丸太が一か所でも
                        {
                            if (!mapManager.IsRiver(dwp))//川でないマスがあれば(全て川に入って無ければ)
                            {
                                playerManager.PlayerStop();//プレイヤーを止めて
                                return;//移動しない
                            }
                        }
                    }
                }
            }
            if (mapManager.IsOnWood(playerManager.GetPlayerMapPoint()))//プレイヤーが丸太に乗っていたら
            {
                foreach (Vector2 nwp in nowPointWood.GetMapPoints())//プレイヤーの乗る丸太が一か所でも
                {
                    if (!mapManager.IsRiver(nwp))//川でないマスがあれば(全て川に入って無ければ)
                    {
                        if (!Direction.IsSameAxis(playerManager.GetPlayerDirection(), nowPointWood.GetDirection()))//軸が違えば
                        {
                            playerManager.PlayerStop();//プレイヤーを止めて
                            return;//移動しない
                        }
                    }
                }
                
                if (mapManager.IsRiver(playerDestination))//移動先が川で
                {
                    if (mapManager.IsOnWood(playerDestination))//丸太があるなら
                    {
                        if (destinationPointWood != nowPointWood)//その丸太が今乗っている丸太と違うなら
                        {
                            if (destinationPointWood.GetState() == 3)//丸太が流れているなら
                            {
                                playerManager.PlayerStop();//プレイヤーを止めて
                                return;//移動しない
                            }
                            foreach (Vector2 dwp in destinationPointWood.GetMapPoints())//その丸太が一か所でも
                            {
                                if (!mapManager.IsRiver(dwp))//川でないマスがあれば(全て川に入って無ければ)
                                {
                                    if (!Direction.IsSameAxis(playerManager.GetPlayerDirection(), destinationPointWood.GetDirection()))//軸が違うなら
                                    {
                                        playerManager.PlayerStop();//プレイヤーを止めて
                                        return;//移動しない
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //ここから移動したいマスに侵入できるかの判断

            if (mapManager.IsPlayerEnterMapchip(playerManager.GetPlayerDirection(), playerManager.GetPlayerMapPoint()))//移動したいマスにプレイヤーが侵入できるなら
            {

                if (nowPointWood != destinationPointWood || (mapManager.IsOnWood(playerManager.GetPlayerMapPoint()) && mapManager.IsRiver(playerManager.GetPlayerMapPoint())))
                {
                    if (mapManager.IsOnWood(playerDestination) && !mapManager.IsRiver(playerDestination))//移動先に丸太があって川でないなら
                    {
                        //Debug.Log("PlayerDestinationOnWood");
                        destinationPointWood = woodManager.GetIncludedPointWood(playerDestination);//その丸太を取得
                                                                                                   //Debug.Log(dw);
                        if (!Direction.IsSameAxis(playerManager.GetPlayerDirection(), destinationPointWood.GetDirection()))//プレイヤーの移動方向軸と丸太の方向軸が違うなら
                        {
                            for (int i = 0; i < destinationPointWood.GetLength(); i++)//丸太の長さ分
                            {
                                Vector2 woodPoint = mapManager.GetFindPoint((int)destinationPointWood.GetRootPoint().y, (int)destinationPointWood.GetRootPoint().x, destinationPointWood.GetDirection(), i);//丸太のマス一つ分の現在地マス
                                Vector2 woodDistination = mapManager.GetFindPoint((int)woodPoint.y, (int)woodPoint.x, playerManager.GetPlayerDirection(), 1);//丸太のマスの移動先1マス
                                if (!mapManager.IsCanEnterWood(woodDistination))//1マスでも丸太が侵入できないますがあれば
                                {
                                    playerManager.PlayerStop();//プレイヤーを止めて
                                                               //Debug.Log("Return");
                                    return;//移動しない
                                }
                            }
                            //ここまでくると丸太移動が出来ることが確定
                            destinationPointWood.MoveSet(playerManager.GetPlayerDirection());//丸太をプレイヤーの移動方向へ移動させる
                            Vector2 dwRootPoint = destinationPointWood.GetRootPoint();//丸太の根本のマス目を取得
                            for (int i = 0; i < destinationPointWood.GetLength(); i++)//その丸太全てに対し
                            {
                                Vector2 woodPoint = mapManager.GetFindPoint((int)dwRootPoint.y, (int)dwRootPoint.x, destinationPointWood.GetDirection(), i);
                                Vector2 woodDistination = mapManager.GetFindPoint((int)woodPoint.y, (int)woodPoint.x, playerManager.GetPlayerDirection(), 1);
                                destinationPointWood.ChangeMapPoints(i, woodDistination);//丸太に自身の場所データを変更させる
                                mapManager.RemoveWood(woodPoint);//もともと乗っていたマス目を乗っていないことに
                                                                 //Debug.Log("woodPoint:" + woodPoint);
                                                                 //Debug.Log("woodDistination:" + woodDistination);
                                mapManager.OnWood(woodDistination, destinationPointWood.GetDirection());//移動先のマス目に乗っているように
                            }
                            if (playerManager.GetPlayerDirection() != Direction.DirectionState.Up)//上に行かないなら
                            {
                                destinationPointWood.ChangeLayer();//丸太のレイヤーを変更
                            }
                        }
                    }
                }
                if (mapManager.IsOnWood(playerManager.GetPlayerMapPoint()) && !mapManager.IsOnWood(playerDestination))
                {
                    playerManager.PlayerOffsetRemove();
                }
                if (mapManager.IsOnWood(playerDestination) && !mapManager.IsOnWood(playerManager.GetPlayerMapPoint()))
                {
                    playerManager.PlayerOffsetAdd();
                }
                playerManager.PlayerMoveStart();//プレイヤー移動開始
                if (playerManager.GetPlayerDirection() != Direction.DirectionState.Up)
                {
                    playerManager.ChangePlayerLayer();
                    if (destinationPointWood != null)
                        playerManager.SetPlayerLayer(destinationPointWood.GetMaxPointHeight());
                }
            }
            else//移動したいマスにプレイヤーが移動出来ないなら
            {
                playerManager.PlayerStop();//プレイヤーを止める
            }
        }
    }

    private void PlayerSlashUpdate()
    {
        //if (playerManager.GetPlayerSlashMode() == 1)
        if (playerManager.GetPlayerMode() == 5)
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
            woodManager.ChangeWoodsLayer();
            woodManager.LastWoodsBornFromTree();

            Vector2 bTreeCreatPostion = targetTreePoint * mapManager.GetMapChipSize();
            bTreeCreatPostion.y *= -1;
            woodManager.CreateBreakWood(mapManager.GetTreeLength(targetTreePoint) - 1, playerManager.GetPlayerDirection(), bTreeCreatPostion, targetTreePoint);

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

            mapManager.Felling(targetTreePoint);
        }
        else
        {
            if (playerManager.GetPlayerDirection() == Direction.DirectionState.Down)
            {
                Wood targetWood = woodManager.GetIncludedPointWood(targetTreePoint);
                if (targetWood != null)
                {
                    playerManager.SetPlayerLayer((int)targetWood.GetMaxPointHeight());
                }
                else
                {
                    playerManager.SetPlayerLayer((int)playerManager.GetPlayerMapPoint().y + 1);
                }
            }
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
                woodManager.ChangeWoodsLayer();
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
                    {
                        isAllRiver = false;
                        w.OutRiverSpriteChange();
                        //Debug.Log("woodPoint"+p);
                        break;
                    }
                }
                if (!isAllRiver)//1マスでも川マスに乗っていなければ
                    continue;//この丸太の処理を終了

                //Debug.Log("OnRiver");
                w.InRiverSpriteChange();
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

                //Debug.Log("FlowDir;" + distinationDir);
                Vector2[] woodDistainationPoints = new Vector2[w.GetLength()];//移動先のポイント
                Vector2[] woodPoints = w.GetMapPoints();//丸太の元にあった場所の記憶用
                bool isFlow = true;//流れるかどうか
                bool isPlayerOnDistanation = false;//移動先にプレイヤーがいるか
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
                        //Debug.Log("FalseP:" + wDP);
                        break;
                    }
                    if (wDP == playerManager.GetPlayerMapPoint() && !w.IsIncludedMapPoint(playerManager.GetPlayerMapPoint()) &&
                        !mapManager.IsRiver(wDP))
                    {
                        //isFlow = false;
                        //break;
                        isPlayerOnDistanation = true;
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
                    w.MoveSet(distinationDir);//丸太移動開始
                    if (distinationDir != Direction.DirectionState.Up)//上方向でなければ
                        w.ChangeLayer();//丸太のレイヤーを変える
                    foreach (Vector2 wp in woodPoints)//動かす前の丸太に
                    {
                        if (playerManager.GetPlayerMapPoint() == wp
                            && mapManager.GetFindPoint((int)wp.y, (int)wp.x, distinationDir, 1) != playerManager.GetPlayerMapPoint())//プレイヤーが乗っていたなら
                        {
                            PlayerFlow(distinationDir, mapManager.GetFindPoint((int)wp.y, (int)wp.x, distinationDir, 1));
                            playerManager.SetPlayerLayer(w.GetMaxPointHeight());
                            break;
                        }
                    }
                    if (isPlayerOnDistanation)
                    {
                        foreach (Vector2 wDP in woodDistainationPoints)
                        {
                            if (wDP == playerManager.GetPlayerMapPoint())
                            {
                                if (!mapManager.IsOnWood(mapManager.GetFindPoint((int)wDP.y, (int)wDP.x, distinationDir, 1)))
                                {
                                    Debug.Log("PFlow:" + wDP);
                                    PlayerPushedWood(distinationDir, mapManager.GetFindPoint((int)wDP.y, (int)wDP.x, distinationDir, 1));
                                }
                                else if (!w.IsIncludedMapPoint(mapManager.GetFindPoint((int)wDP.y, (int)wDP.x, distinationDir, 1)))
                                {
                                    PlayerPushedWood(distinationDir, mapManager.GetFindPoint((int)wDP.y, (int)wDP.x, distinationDir, 1));
                                }
                                break;
                            }
                        }
                        if (distinationDir != Direction.DirectionState.Up)
                            playerManager.SetPlayerLayer(w.GetMaxPointHeight());
                    }
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

    private void PlayerFlow(Direction.DirectionState moveDir, Vector2 Distination)
    {
        playerManager.SetPlayerMapPoint(Distination);
        playerManager.PlayerAutoMoveStart(moveDir);
        if (moveDir != Direction.DirectionState.Up)
            playerManager.ChangePlayerLayer();
    }
    private void PlayerPushedWood(Direction.DirectionState moveDir, Vector2 Distination)
    {
        playerManager.SetPlayerMapPoint(Distination);
        //if (playerManager.GetPlayerMoveMode() == 2)
        if (playerManager.GetPlayerMode() == 2)
        {
            playerManager.ForciblyPlayerAutoMoveStart();
        }
        else
        {
            playerManager.PlayerStop();
            playerManager.PlayerAutoMoveStart(moveDir);
        }
        if (moveDir != Direction.DirectionState.Up)
            playerManager.ChangePlayerLayer();
    }

    private void PlayerFallChack()
    {
        Vector2 PlayerPoint = playerManager.GetPlayerMapPoint();
        if (mapManager.IsHole(PlayerPoint) && !mapManager.IsOnWood(PlayerPoint))
        {
            playerManager.PlayerFall();
            gameState = GameState.PlayerFalling;
        }
    }

    private void PlayerGoalChack()
    {
        if (mapManager.IsGoal(playerManager.GetPlayerMapPoint()))
        {
            if (playerManager.GetPlayerMode() == 0)
            {
                playerManager.PlayerGoal();
                IsClear = true;
                Debug.Log("Goal");
            }
        }
    }

    private void GameClear()
    {
        if (playerManager.GetPlayerMode() == 10)
        {
            gameState = GameState.EndFadeOut;
            fadeManager.FadeOutStart();
        }
    }
    private void LoadNextScene()
    {
        SceneManager.LoadScene("TitleScene");
    }


    private void Pose()
    {
        if (gameState != GameState.Nomal)
            return;

        if (Input.GetKeyDown(KeyCode.P) && currentState == GameState.Nomal)
        {
            gameState = GameState.Pose;
            playerManager.PlayerAnimationStop();
            //poseMenu.SetActive(true);
            poseManager.AppearPoseMenu();
        }
    }

    private void StageResetStart()
    {
        if (gameState != GameState.Nomal)
            return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.Reset;
            fadeManager.FadeOutStart();
        }
    }

    private void GridLineChange()
    {
        mapManager.GridLineChange();
    }

    private void DebugCameraMove()
    {
        Camera camera = Camera.main;

        if (Input.GetKey(KeyCode.W))
        {
            camera.transform.position += new Vector3(0, cameraSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            camera.transform.position += new Vector3(0, -cameraSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            camera.transform.position += new Vector3(cameraSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            camera.transform.position += new Vector3(-cameraSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            camera.orthographicSize -= cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.C))
        {
            camera.orthographicSize += cameraSpeed * Time.deltaTime;
        }
    }
    //private void DebugSceneReload()
    //{
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //    }
    //}
}
