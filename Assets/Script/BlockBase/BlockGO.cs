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
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    public void Match()
    {
        isMoving = false;
        isStoping = false;
        StartCoroutine(MatchAni());
    }

    IEnumerator MatchAni()
    {
        float elapse = 0;

        while(elapse < 1)
        {
            elapse += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, elapse);
            yield return true;
        }
        BlockGOPool.pool.Push(this);
        PushBack();
    }

    Action callbackMove;

    public void Move(float x, float y, Action callback)
    {
        startPos = transform.localPosition;
        EndPos = new Vector3(x, y);

        elapseTime = 0;
        isMoving = true;
        isStoping = false;
        callbackMove = callback;
    }

    public void Stop()
    {
        isStoping = true;
        bouncePower = BK_Function.ConvertRange(MaxSpeed, MinSpeed, minPower, maxPower, MinSpeed-aniTime);
        bounceNum = BK_Function.ConvertRange(minPower, maxPower, minBounce, maxBounce, bouncePower);
        //stopAniTme = BK_Function.ConvertRange(minPower, maxPower, minStopTime, maxStopTime, bouncePower);
        //bounceNum = BK_Function.ConvertRange(minStopTime, maxStopTime, minBounce, maxBounce, stopAniTme);
        elapseTime = 0;
        accumeTime = 0;
    }

    public void SwapStop()
    {
        elapseTime = 0;
        accumeTime = 0;
    }

    public void PushBack()
    {
        isMoving = false;
        isStoping = false;
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
    const float accelerationTime = 1;
    const float MinSpeed = 0.5f;
    const float MaxSpeed = 0.05f;
    float aniTime = 0;//=speed. 작을수록 빠르게 움직인다. 
    float reverseTime = 1;
    float elapseTime = 0;//필드 한 칸 이동 구간내의 경과시간
    float accumeTime = 0;//이동중인 시간 누적
    
    Vector3 startPos, EndPos, vBuffer;

    void Moving()
    {
        elapseTime += Time.deltaTime;
        accumeTime += Time.deltaTime;

        if (accumeTime < accelerationTime)
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

            if(callbackMove != null)
                callbackMove();
        }
        else
            transform.localPosition = Vector3.Lerp(startPos, EndPos, elapseTime*reverseTime);
    }

    const float minPower = 0.01f;
    const float maxPower = 0.1f;
    const float minStopTime = 0.1f;
    const float maxStopTime = 0.8f;
    const float minBounce = 1.0f;
    const float maxBounce = 4.0f;
    float stopAniTme = 1;
    float stopReverseTime = 1;
    float bouncePower = 0.3f;
    float bounceNum = 6.75f;

    void Stoping()
    {
        elapseTime += Time.deltaTime;

        if (elapseTime < stopAniTme)
        {
            vBuffer.y = Ease.Bounce(elapseTime* stopReverseTime, bounceNum) * bouncePower;
            transform.localPosition = EndPos + vBuffer;
        }
        else
        {
            transform.localPosition = EndPos;
            isStoping = false;
        }
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