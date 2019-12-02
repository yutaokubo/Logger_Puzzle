using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private Player player;//プレイヤー

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

    public void SetPlayerMoveDistance(Vector2 distance)
    {
        player.SetMoveDistance(distance);
    }
}
