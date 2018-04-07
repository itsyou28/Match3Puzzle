using UnityEngine;
using System;
using System.Collections;

public interface iBlockGO
{
    void SetBlock(Block block, float x, float y);
    void Match();
    void PushBack();
    void Move(float x, float y, Action callback);
    void Stop();
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

    Action callbackMove;

    public void Move(float x, float y, Action callback)
    {
        startPos = transform.localPosition;
        EndPos = new Vector3(x, y);
        elapseTime = 0;
        isMoving = true;
        callbackMove = callback;
    }

    public void Stop()
    {
        isStoping = true;
        elapseTime = 0;
    }

    public void PushBack()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isMoving)
            Moving();

        if (isStoping)
            Stoping();
    }

    bool isStoping = false;
    bool isMoving = false;
    const float aniTime = 0.2f;
    const float reverseTime = 1 / aniTime;
    float elapseTime = 0;
    
    Vector3 startPos, EndPos;

    void Moving()
    {
        elapseTime += Time.deltaTime;
        if (elapseTime >= aniTime)
        {
            isMoving = false;
            transform.localPosition = EndPos;

            if (callbackMove != null)
            {
                callbackMove();
            }
        }
        else
            transform.localPosition = Vector3.Lerp(startPos, EndPos, elapseTime*reverseTime);
    }

    void Stoping()
    {
        isStoping = false;
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

    public void Move(float x, float y, Action callback)
    {
    }

    public void Stop()
    {
    }
}