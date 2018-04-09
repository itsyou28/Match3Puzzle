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
    void SwapStop();
}

public class BlockGO : MonoBehaviour, iBlockGO
{
    [SerializeField]
    bool isDebug;

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
        if (isDebug)
            Debug.Log("SetBlock " + transform.localPosition);
        gameObject.SetActive(true);
    }

    void OnEnable()
    {
        if (isDebug)
            Debug.Log("Enable " + transform.localPosition);
    }

    void OnDisable()
    {
        if (isDebug)
            Debug.Log("OnDisable " + transform.localPosition);
    }
    
    public void Match()
    {
        if (isDebug)
            Debug.Log("Match " + transform.localPosition);
        PushBack();
    }

    Action callbackMove;

    public void Move(float x, float y, Action callback)
    {
        startPos = transform.localPosition;
        EndPos = new Vector3(x, y);

        if (isDebug && Vector3.Distance(startPos, EndPos) > 1)
            Debug.LogWarning("long move  "+startPos + " " + EndPos);

        elapseTime = 0;
        isMoving = true;
        callbackMove = callback;
    }

    public void Stop()
    {
        isStoping = true;
        elapseTime = 0;
        accumeTime = 0;
    }

    public void SwapStop()
    {
        isStoping = true;
        elapseTime = 0;
        accumeTime = 0;
    }

    public void PushBack()
    {
        isMoving = false;
        isStoping = false;
        gameObject.SetActive(false);
        if (isDebug)
            Debug.Log("PushBack " + transform.localPosition);
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
    const float MinSpeed = 0.5f;
    const float MaxSpeed = 0.05f;
    float speed = MinSpeed;
    float aniTime = 0;
    float reverseTime = 1;
    float elapseTime = 0;
    float accumeTime = 0;
    
    Vector3 startPos, EndPos;

    void Moving()
    {
        elapseTime += Time.deltaTime;
        accumeTime += Time.deltaTime;

        if (accumeTime < 1)
        {
            aniTime = BK_Function.ConvertRangeValue(MaxSpeed, MinSpeed, 1-Ease.InOutQuad(accumeTime));
            reverseTime = 1 / aniTime;
        }
        else
        {
            aniTime = MaxSpeed;
            reverseTime = 1 / aniTime;
        }

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

    public void SwapStop()
    {
    }
}