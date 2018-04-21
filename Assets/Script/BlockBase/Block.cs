using System;
using UnityEngine;


public interface iBlock
{
    int BlockType { get; }
    bool IsStop { get; }
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

    int blockType;
    public int BlockType { get { return blockType; } }

    [NonSerialized]
    bool isMoving = false;

    public bool IsStop { get { return !isMoving; } }

    //이벤트 발생으로 매칭 과정이 진행된다. 코드 순서에 따라 참조 변환으로 null 예외가 발생할 수 있다. 
    void SetMovingFlag(bool bValue)
    {
        isMoving = bValue;

        BlockMng.Inst.UpdateStopFlag();
    }

    public void InitByEditor(BlockField field, int blockType)
    {
        SetField(field);
        this.blockType = blockType;
    }

    #region Block Reset
    //Reset에서 SetMovingFlag를 사용할 경우 스테이지 시작 단계가 완료 되기 전에 매칭이 발생한다. 

    public void Reset(BlockField field, int blockType)
    {
        SetField(field);
        this.blockType = blockType;
        isMoving = false;

        DeployScreen();
    }

    public void ResetRand(BlockField field, int randMax)
    {
        SetField(field);
        blockType = UnityEngine.Random.Range(1, randMax);
        isMoving = false;

        DeployScreen();
    }

    public void ResetAnotherBlockType(BlockField field, int randMax)
    {
        SetField(field);
        blockType = BK_Function.Random(1, randMax, blockType);
        isMoving = false;

        DeployScreen();
    } 
    #endregion

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
        //Debug.Log("MoveToNextField");
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
            blockGO.SwapStop();
            if (callback != null)
                callback();
            SetMovingFlag(false);
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
            blockGO.Stop();
            SetMovingFlag(false);
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
