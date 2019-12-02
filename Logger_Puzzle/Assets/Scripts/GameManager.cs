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
        mapManager.SetPlayer(playerManager.GetPlayer());
        mapManager.MapCreate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
