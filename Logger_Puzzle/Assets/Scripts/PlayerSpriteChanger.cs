using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteChanger : MonoBehaviour
{
    [SerializeField]
    private Sprite[] NomalSprites;

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
        return NomalSprites[(int)dir];
    }
}
