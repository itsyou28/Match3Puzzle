using UnityEngine;
using System;
using System.Collections;

public interface iBlockGO
{
    void SetBlock(Block block, float x, float y);
    void Move(float x, float y, Action callback);
    void CleanUp();
    void Stop();
    void SwapStop();
    void MakeOverDissolve(float x, float y);
}

public class BlockGO : MonoBehaviour, iBlockGO
{
    [SerializeField]
    SpriteRenderer sprite;

    iBlockForGO block;

    public void SetBlock(Block block, float x, float y)
    {
        this.block = block;
        this.block.OnTransitionState += OnTransitionBlockState;

        SetSprite();

        if (block.IsCreateField)
            accumeTime = accelerationTime * 0.5f;

        transform.localPosition = new Vector3(x, y);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    private void SetSprite()
    {
        if (block.BlockType < 1 || block.BlockType > 12)
            Debug.LogError("blockType Error " + block.BlockType);
        sprite.sprite = BlockGOPool.Inst.arrBlockSprite[block.BlockType - 1];
    }

    private void OnTransitionBlockState()
    {
        switch (block.eState)
        {
            case BlockState.MakeOver:
                StartCoroutine(MakeOver());
                break;
            case BlockState.MatchingGlow:
                StartCoroutine(MatchingGlow());
                break;
            case BlockState.MatchingDissolve:
                StartCoroutine(Dissolve());
                break;
        }
    }

    IEnumerator MakeOver()
    {
        float elapse = 0;
        float aniTime = 0.5f;
        float reverse = 1 / aniTime;
        float scale = 0;
        Vector3 vScale;
        while (elapse < aniTime)
        {
            elapse += Time.deltaTime;
            scale = 1 - Ease.InOutBack(elapse * reverse);
            vScale.x = vScale.y = vScale.z = scale;
            transform.localScale = vScale;

            yield return true;
        }

        SetSprite();

        yield return true;

        elapse = 0;

        while (elapse < aniTime)
        {
            elapse += Time.deltaTime;
            scale = Ease.OutElastic(elapse * reverse);
            vScale.x = vScale.y = vScale.z = scale;
            transform.localScale = vScale;

            yield return true;
        }

        block.TransitionState(BlockState.Ready);
    }

    IEnumerator MatchingGlow()
    {
        transform.localScale = Vector3.one * 1.1f;

        yield return new WaitForSeconds(0.2f);

        block.TransitionState(BlockState.MatchingDissolve);
    }

    IEnumerator Dissolve()
    {
        float elapse = 0;
        float aniTime = 0.2f;
        float reverseTime = 1 / aniTime;

        while (elapse < aniTime)
        {
            elapse += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, elapse * reverseTime);
            yield return true;
        }

        block.TransitionState(BlockState.Pushback);
    }

    public void MakeOverDissolve(float x, float y)
    {
        startPos = transform.localPosition;
        EndPos = new Vector3(x, y);

        block.TransitionState(BlockState.MakeOverDissolve);

        StartCoroutine(MakeOverDissolve());
    }

    IEnumerator MakeOverDissolve()
    {
        float elapse = 0;
        float aniTime = 0.3f;
        float reverseTime = 1 / aniTime;

        while (elapse < aniTime)
        {
            elapse += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, elapse * reverseTime);
            transform.localPosition = Vector3.Lerp(startPos, EndPos, elapse * reverseTime);
            yield return true;
        }

        block.TransitionState(BlockState.Pushback);
    }

    Action callbackMove;

    public void Move(float x, float y, Action callback)
    {
        startPos = transform.localPosition;
        EndPos = new Vector3(x, y);

        elapseTime = 0;

        callbackMove = callback;
        isStoping = false;
    }

    public void Stop()
    {
        bouncePower = BK_Function.ConvertRange(MaxSpeed, MinSpeed, minPower, maxPower, MinSpeed - aniTime);
        bounceNum = BK_Function.ConvertRange(minPower, maxPower, minBounce, maxBounce, bouncePower);
        //stopAniTme = BK_Function.ConvertRange(minPower, maxPower, minStopTime, maxStopTime, bouncePower);
        //bounceNum = BK_Function.ConvertRange(minStopTime, maxStopTime, minBounce, maxBounce, stopAniTme);
        elapseTime = 0;
        accumeTime = 0;

        isStoping = true;
    }

    public void SwapStop()
    {
        elapseTime = 0;
        accumeTime = 0;
    }

    public void CleanUp()
    {
        callbackMove = null;
        isStoping = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (block.eState == BlockState.Moving)
            Moving();
        else if (isStoping)
            Stoping();
    }

    const float accelerationTime = 0.8f;
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
            aniTime = BK_Function.ConvertRangeValue(MaxSpeed, MinSpeed, 1 - Ease.InOutQuad(accumeTime));
            reverseTime = 1 / aniTime;
        }
        else
        {
            aniTime = MaxSpeed;
            reverseTime = 1 / aniTime;
        }

        if (elapseTime >= aniTime)
        {
            transform.localPosition = EndPos;

            if (callbackMove != null)
                callbackMove();
        }
        else
            transform.localPosition = Vector3.Lerp(startPos, EndPos, elapseTime * reverseTime);
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
    bool isStoping = false;

    void Stoping()
    {
        elapseTime += Time.deltaTime;

        if (elapseTime < stopAniTme)
        {
            vBuffer.y = Ease.Bounce(elapseTime * stopReverseTime, bounceNum) * bouncePower;
            transform.localPosition = EndPos + vBuffer;
        }
        else
        {
            transform.localPosition = EndPos;
            isStoping = false;
            //block.TransitionState(BlockState.Ready);
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

    public void CleanUp()
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

    public void MakeOverDissolve(float x, float y)
    {
    }
}