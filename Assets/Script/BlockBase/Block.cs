using System;
using UnityEngine;

public class BlockPool
{
    private static BlockPool instance = null;
    private static ObjectPool<iBlock> pool;
    public static ObjectPool<iBlock> Pool
    {
        get
        {
            if (instance == null)
                instance = new BlockPool();
            return pool;
        }
    }

    private BlockPool()
    {
        pool = new ObjectPool<iBlock>(100, 20, CreateBlock);
    }

    iBlock CreateBlock()
    {
        return new Block();
    }
}


public interface iBlock
{
    int BlockType { get; }

    void Reset(BlockField field, int blockType);
    void ResetRand(BlockField field, int randMax);
    void ResetAnotherBlockType(BlockField field, int randMax);
    void CleanUp();
    void MoveToNextField();
    void Match();
    void DeployScreen();
    void SwapMove(BlockField target, Action callback);
    void SetSwapField(BlockField target);
    void CheckField(BlockField target);
}

/// <summary>
/// 게임 시간동안 빈번하게 생성/해제가 발생하므로 pool로 관리한다. 
/// 블럭 타입 관리 및 블럭타입 비교를 구현한다. 
/// 블럭이 위치하고 있는 필드, 이동할 필드 등을 관리한다. 
/// </summary>
[Serializable]
public class Block : iBlock
{
    iBlockField curField;

    [NonSerialized]
    iBlockGO blockGO;

    public int BlockType { get { return blockType; } }
    int blockType;

    //블럭이 이동중인지 체크한다. 
    // -블럭이 이동중일 때 매치가 발생할 경우 블럭 재사용에서 문제가 발생한다. 
    // -유저에게 불완전한 피드백을 줄 수 있기 때문에 블럭이 이동중일 때 이동의 완료를 보장해야 한다.
    // -이동을 시작한 순서대로 번호를 가져간다 마지막 번호가 멈추면 멈춘걸로 판단한다. 
    public static bool IsMoving { get; private set; }
    static int accumeMoving = 0;

    [NonSerialized]
    int movingNumber;

    [NonSerialized]
    bool isMoving = false;

    void SetMovingFlag(bool bValue)
    {
        //먼저 이동을 시작했지만 이동거리가 더 길어서 나중에 끝나는 경우를 처리하지 못한다. 
        isMoving = bValue;

        if (bValue)
        {
            accumeMoving++;
            movingNumber = accumeMoving;
            IsMoving = true;
        }
        else
        {
            if (movingNumber >= accumeMoving)
            {
                accumeMoving = 0;
                IsMoving = false;
            }
        }
    }

    public void InitByEditor(BlockField field, int blockType)
    {
        SetField(field);
        this.blockType = blockType;
    }

    public void Reset(BlockField field, int blockType)
    {
        SetField(field);
        this.blockType = blockType;
        SetMovingFlag(false);

        DeployScreen();
    }

    public void ResetRand(BlockField field, int randMax)
    {
        SetField(field);
        blockType = UnityEngine.Random.Range(1, randMax);
        SetMovingFlag(false);

        DeployScreen();
    }

    public void ResetAnotherBlockType(BlockField field, int randMax)
    {
        SetField(field);
        blockType = BK_Function.Random(1, randMax, blockType);
        SetMovingFlag(false);

        DeployScreen();
    }

    public void SetSwapField(BlockField field)
    {
        SetField(field);
    }

    void SetField(iBlockField target)
    {
        //Debug.Log("SetField " + target.X + " " + target.Y);
        curField = target;
    }

    public void CheckField(BlockField field)
    {
        if (curField.X != field.X || curField.Y != field.Y)
            Debug.LogError(curField.X + " " + curField.Y + " // " + field.X + " " + field.Y);
    }

    public void SetBlockType(int blockType)
    {
        this.blockType = blockType;
    }

    public void MoveToNextField()
    {
        //이동중에 호출 받았을 때 next필드가 변경되서 블럭위치가 튀거나 이상한 움직임을 보이지 않도록 블럭해야 한다. 
        if (isMoving)
            return;

        if (curField.next.IsPlayable && curField.next.IsEmpty)
        {
            curField.SetBlock(null);
            SetField(curField.next);
            curField.SetBlock(this);

            blockGO.Move(curField.X, curField.Y, Move);

            SetMovingFlag(true);
        }
    }

    public void SwapMove(BlockField target, Action callback)
    {
        SetMovingFlag(true);
        blockGO.Move(target.X, target.Y, () =>
        {
            SetMovingFlag(false);
            blockGO.SwapStop();
            if (callback != null)
                callback();
        });
    }

    void Move()
    {
        if (curField.next.IsPlayable && curField.next.IsEmpty)
        {
            curField.SetBlock(null);
            SetField(curField.next);
            curField.SetBlock(this);

            blockGO.Move(curField.X, curField.Y, Move);
        }
        else
        {
            SetMovingFlag(false);
            blockGO.Stop();
        }
    }

    public void DeployScreen()
    {
        if (blockGO == null)
            blockGO = BlockGOPool.pool.Pop();
        blockGO.SetBlock(this, curField.X, curField.Y);
    }

    public void Match()
    {
        if (blockGO != null)
        {
            //화면에서 제거되고 pool로 돌아간다. 
            blockGO.Match();
            blockGO = null;
        }
    }

    public void CleanUp()
    {
        if (blockGO != null)
        {
            blockGO.PushBack();
            BlockGOPool.pool.Push(blockGO);
            blockGO = null;
        }
    }

    #region override Equals
    public override int GetHashCode()
    {
        return BlockType.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is Block))
            return false;

        return Equals((Block)obj);
    }

    public bool Equals(Block other)
    {
        if (other == null)
            return false;

        if (BlockType != other.BlockType)
            return false;

        return true;
    }
    public static bool operator ==(Block lValue, Block rValue)
    {
        // Check for null on left side.
        if (ReferenceEquals(lValue, null))
        {
            if (ReferenceEquals(rValue, null))
            {
                // null == null = true.
                return true;
            }

            // Only the left side is null.
            return false;
        }

        return lValue.Equals(rValue);
    }

    public static bool operator !=(Block lValue, Block rValue)
    {
        return !(lValue == rValue);
    }
    #endregion
}
