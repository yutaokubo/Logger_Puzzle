using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChipSpriteSelecter : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> sprites;

    [SerializeField]
    private List<Sprite> treeSprites;

    [SerializeField]
    private List<Sprite> riverSprites;

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
        if (spriteNum > sprites.Count)
        {
            return sprites[0];
        }

        return sprites[spriteNum];
    }
    public Sprite GetTreeSprite(int treeLength)
    {
        
        if (treeLength > treeSprites.Count)
        {
            return treeSprites[0];
        }
        return treeSprites[treeLength - 1];
    }
    public Sprite GetRiverSprite(int riverTiming)
    {

        if (riverTiming > riverSprites.Count)
        {
            return riverSprites[0];
        }
        return riverSprites[riverTiming];
    }
}
