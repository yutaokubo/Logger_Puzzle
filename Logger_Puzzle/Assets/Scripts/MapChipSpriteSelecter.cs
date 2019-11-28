using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChipSpriteSelecter : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> sprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetMapChipSprite(int spriteNum)
    {
        if(spriteNum>sprites.Count)
        {
            return sprites[0];
        }

        return sprites[spriteNum];
    }
}
