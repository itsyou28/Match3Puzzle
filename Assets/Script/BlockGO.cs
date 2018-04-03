using UnityEngine;
using System.Collections;

public class BlockGO : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sprite;

    public void SetBlock(Block block)
    {
        switch(block.BlockType)
        {
            case 1:
                sprite.color = Color.red;
                break;
            case 2:
                sprite.color = Color.blue;
                break;
            case 3:
                sprite.color = Color.green;
                break;
            case 4:
                sprite.color = Color.yellow;
                break;
            case 5:
                sprite.color = Color.magenta;
                break;
            case 6:
                sprite.color = Color.cyan;
                break;
            case 7:
                sprite.color = Color.gray;
                break;
            case 8:
                sprite.color = Color.black;
                break;
        }
    }
}
