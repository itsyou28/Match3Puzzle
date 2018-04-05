using UnityEngine;
using System.Collections;

public interface iBlockGO
{
    void SetBlock(Block block, float x, float y);
    void Match();
    void PushBack();
}

public class BlockGO : MonoBehaviour, iBlockGO
{
    [SerializeField]
    SpriteRenderer sprite;

    public void SetBlock(Block block, float x, float y)
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

        transform.localPosition = new Vector3(x, y);
        gameObject.SetActive(true);
    }
    
    public void Match()
    {
        gameObject.SetActive(false);
    }

    public void PushBack()
    {
        gameObject.SetActive(false);
    }
}

public class BlockGODummy : iBlockGO
{
    public void SetBlock(Block block, float x, float y)
    {
    }

    public void Match()
    {
    }

    public void PushBack()
    {
    }
}