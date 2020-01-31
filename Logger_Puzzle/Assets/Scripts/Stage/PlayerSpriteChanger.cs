using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteChanger : MonoBehaviour
{
    [SerializeField]
    private Sprite[] nomalSprites;
    [SerializeField]
    private Sprite[] slashSprites;
    [SerializeField]
    private Sprite[] walkSprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetNomalSprite(Direction.DirectionState dir)
    {
        return nomalSprites[(int)dir];
    }

    public Sprite GetSlashSprite(Direction.DirectionState dir)
    {
        return slashSprites[(int)dir];
    }

    public Sprite GetWalkSprite(Direction.DirectionState dir,int timing)
    {
        return walkSprites[timing * 4 + (int)dir];
    }
}
