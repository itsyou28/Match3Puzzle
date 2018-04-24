﻿using System;
using UnityEngine;


public interface iBlock
{
    int BlockType { get; }
    BlockState eState { get; }
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

public interface iBlockForGO
{
    BlockState eState { get; }
    event Action OnTransitionState;
    void TransitionState(BlockState state);
}

public enum BlockState
{
    InPool,
    Initializing,
    Ready,
    SwapMoving,
    Moving,
    MatchingGlow,
    MatchingDissolve,
    Pushback,
}

/// <summary>
/// 게임 시간동안 빈번하게 생성/해제가 발생하므로 pool로 관리한다. 
/// 블럭 타입 관리 및 블럭타입 비교를 구현한다. 
/// 블럭이 위치하고 있는 필드, 이동할 필드 등을 관리한다. 
/// </summary>
[Serializable]
public class Block : iBlock, iBlockForGO
{
    iBlockField curField;

    [NonSerialized]
    iBlockGO blockGO;

    public event Action OnTransitionState;

    int blockType;
    public int BlockType { get { return blockType; } }

    public BlockState eState { get; private set; }

    public void InitByEditor(BlockField field, int blockType)
    {
        eState = BlockState.InPool;
        SetField(field);
        this.blockType = blockType;
    }

    #region Block Reset

    public void Reset(BlockField field, int blockType)
    {
        TransitionState(BlockState.Initializing);

        SetField(field);
        this.blockType = blockType;

        DeployScreen();
    }

    public void ResetRand(BlockField field, int randMax)
    {
        TransitionState(BlockState.Initializing);

        SetField(field);
        blockType = UnityEngine.Random.Range(1, randMax);

        DeployScreen();
    }

    public void ResetAnotherBlockType(BlockField field, int randMax)
    {
        TransitionState(BlockState.Initializing);

        SetField(field);
        blockType = BK_Function.Random(1, randMax, blockType);

        DeployScreen();
    }

    public void DeployScreen()
    {
        if (blockGO == null)
            blockGO = BlockGOPool.pool.Pop();
        blockGO.SetBlock(this, curField.X, curField.Y);

        TransitionState(BlockState.Ready);
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

    public void SwapMove(BlockField target, Action callback)
    {
        if (eState != BlockState.Ready)
            return;

        TransitionState(BlockState.Moving);

        blockGO.Move(target.X, target.Y, () =>
        {
            blockGO.SwapStop();

            if (callback != null)
                callback();

            TransitionState(BlockState.Ready);
        });
    }

    public void MoveToNextField()
    {
        //이동중에 호출 받았을 때 next필드가 변경되서 블럭위치가 튀거나 이상한 움직임을 보이지 않도록 블럭해야 한다. 
        //Debug.Log("MoveToNextField");
        if (eState != BlockState.Ready)
            return;

        if (curField.next.IsPlayable && curField.next.IsEmpty)
        {
            curField.SetBlock(null);
            SetField(curField.next);
            curField.SetBlock(this);

            blockGO.Move(curField.X, curField.Y, CallbackMove);

            TransitionState(BlockState.Moving);
        }
        else
            Debug.Log("Block curField : " + curField.X + " " + curField.Y + 
                " // curField.next : " + curField.next.X + " " + curField.next.Y + " // " + curField.next.IsEmpty);
    }

    public void CallbackMove()
    {
        if (curField.next.IsPlayable && curField.next.IsEmpty)
        {
            curField.SetBlock(null);
            SetField(curField.next);
            curField.SetBlock(this);

            blockGO.Move(curField.X, curField.Y, CallbackMove);
        }
        else
        {
            blockGO.Stop();
            TransitionState(BlockState.Ready);
        }
    }

    public void Match()
    {
        TransitionState(BlockState.MatchingGlow);
    }


    public void CleanUp()
    {
        if (blockGO != null)
        {
            blockGO.CleanUp();
            BlockGOPool.pool.Push(blockGO);
            blockGO = null;
        }

        eState = BlockState.InPool;
        curField.OnPushbackBlock();
    }

    public void TransitionState(BlockState toState)
    {
        eState = toState;

        switch (eState)
        {
            case BlockState.Ready:
                BlockMng.Inst.UpdateAllReady();
                break;
            case BlockState.Pushback:
                CleanUp();
                break;
        }

        if (OnTransitionState != null)
            OnTransitionState();
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
