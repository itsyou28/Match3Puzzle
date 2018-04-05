using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

/// <summary>
/// 블럭의 이동 경로 관리
/// 블럭의 생성
/// </summary>
[Serializable]
public class BlockField
{
    public static BlockFieldManager fieldMng;

    public Block block;
    public int BlockType { get { return block == null ? -1 : block.BlockType; } }

    public BlockField prev { get; private set; }
    public BlockField next { get; private set; }

    public int row { get; private set; }
    public int col { get; private set; }
    public bool IsPlayable { get { return isPlayable; } }
    public bool IsEmpty { get { return isEmpty; } }
    public bool IsCreateField { get { return isCreateField; } }
    public bool IsMoveable { get { return isMoveable; } }
    public bool IsLast { get { return !next.isMoveable; } }
    public bool IsFirst { get { return !prev.isMoveable; } }
    public float X { get; private set; }
    public float Y { get; private set; }
    /// <summary>
    /// 0:down 1:left 2:up 3:right
    /// </summary>
    public int Direction { get { return direction; } }

    public event Action<BlockField> blockChange;

    bool isPlayable = true;
    bool isEmpty = true;
    bool isCreateField = false;
    bool isMoveable = true;
    int direction = 0; //0:down 1:left 2:up 3:right

    [NonSerialized]
    iBlockFieldGO blockFieldGO;

    #region Call By Editor
    public BlockField(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    /// <param name="dir">//0:down 1:left 2:up 3:right</param>
    public void Initialize(int dir)
    {
        direction = dir;

        next = fieldMng.GetNextByDir(this);
        prev = fieldMng.GetPrevByDir(this);

        X = col;
        Y = fieldMng.RowLength - row;
    }

    bool ValidateField()
    {
        if (isPlayable && prev == null)
        {
            Debug.LogError(row + " " + col + " // playable field 는 반드시 prev / next field가 존재해야 합니다. ");
            return false;
        }
        if (isPlayable && next == null)
        {
            Debug.LogError(row + " " + col + " // playable field 는 반드시 prev / next field가 존재해야 합니다. ");
            return false;
        }

        if (prev == next)
        {
            Debug.LogError(row + " " + col + " // 방향이 충돌합니다. ");
            return false;
        }

        if (next != fieldMng.GetNextByDir(this))
        {
            Debug.LogError(row + " " + col + " // 방향과 지정된 next 필드가 일치하지 않습니다. ");
            return false;
        }

        if (prev != fieldMng.GetPrevByDir(this))
        {
            Debug.LogError(row + " " + col + " // 방향과 지정된 prev 필드가 일치하지 않습니다. ");
            return false;
        }

        return true;
    }

    public void SetPlayable()
    {
        isPlayable = true;

        SetMoveable();
    }

    public void SetNonPlayable()
    {
        isPlayable = false;

        SetMoveable();
    }

    #endregion

    public void DeployScreen()
    {
        if (isPlayable)
        {
            blockFieldGO = BlockFieldGOPool.pool.Pop();
            blockFieldGO.SetBlockField(this);
        }
    }

    public void CleanUp()
    {
        block.CleanUp();
        BlockPool.Pool.Push(block);

        if (blockFieldGO != null)
        {
            blockFieldGO.PushBack();
            BlockFieldGOPool.pool.Push(blockFieldGO); 
        }
    }

    //nonPlayable 이거나 field가 특수 상태이거나 block이 특수 상태일경우 
    void SetMoveable()
    {
        if (!isPlayable)
            isMoveable = false;

        isMoveable = true;

        //필드가 동적으로 이동불가 상태가 됐을 때 next 필드를 prev관리 필드로 지정한다. 
        if (!isMoveable)
        {
            //next.SetDiagnalField(this);
        }
    }

    BlockField[] arrPrev;
    List<BlockField> diagnalList;

    void SetDiagnalField(BlockField orderField)
    {
        //다른 진입 필드가 있다면 대각 필드를 지정하지 않는다. 
        if (arrPrev != null)
        {
            for (int i = 0; i < arrPrev.Length; i++)
            {
                if (arrPrev[i].isMoveable)
                    return;
            }
        }

        BlockField diagnalField = fieldMng.GetRightByDir(orderField);
        if (diagnalField.CanDiagnal(this))
        {
            if (diagnalList == null)
                diagnalList = new List<BlockField>();

            diagnalList.Add(diagnalField);
        }

        diagnalField = fieldMng.GetLeftByDir(orderField);
        if (diagnalField.CanDiagnal(this))
        {
            if (diagnalList == null)
                diagnalList = new List<BlockField>();

            diagnalList.Add(diagnalField);
        }

        //prev 필드가 NonMoveable이라 옆 라인의 상태를 확인하고 옆 라인 필드의 진행방향을 조정해야 하는 경우
        //prev 필드가 2개 이상일 경우

        //prev는 4개를 넘지않는다. 

        //대각필드의 변화에 따라

        //맵 설정에 따른 진입 필드에 따라 

        //고정상태 다음 필드와 대각 필드의 블럭상태 변화에 대한 이벤트를 등록/해제한다. 

        //다음 필드에 블럭이 없고 대각필드에 블럭이 들어왔을때 대각필드 이전 필드의 진행 방향을 다음필드로 지정한다. 
        //다음 필드에 블럭이 들어오면 대각필드 이전 필드의 진행방향을 원래대로 변경한다. 

        //대각필드의 라인에 빈 필드가 생겼다면 대각필드의 다음 필드를 원래대로 복구한다. 
        //대각필드의 다음 필드에 블럭이 들어왔다면 대각필드의 다음 필드를 이 필드로 변경한다. 

        //대각필드의 next는 최우선 순위는 original next이다. 
        //대각필드 다음 필드에 블럭이 차있고 현재 필드에 블럭이 없을 때만 next를 현재 필드로 변경한다?

        //매치 블럭이 모두 삭제된다. 
        //빈 필드에서 다음 블럭을 요청한다. 

    }


    // 대각 필드의 조건에 맞는지 검사한다. 
    bool CanDiagnal(BlockField orderField)
    {
        if (!IsCreateLine())
            return false;

        //orderField의 진행방향과 상대관계에 따라
        //

        return true;
    }

    void OnListenDiagonalField(BlockField target)
    {
        Assert.IsFalse(isMoveable);

        if (target.isEmpty)
        {
            target.prev.next = target.next;
        }
        else
        {
            target.prev.next = next;
        }
    }


    //라인에 생성필드가 있는지 확인합니다.
    bool IsCreateLine()
    {
        if (isCreateField)
            return true;

        BlockField field = prev;
        while (!field.IsFirst)
        {
            if (field.isCreateField)
                return true;
            field = field.prev;
        }

        if (field.isCreateField || field.prev.isCreateField)
            return true;

        return false;
    }

    public void CreateBlock()
    {
        Block block = BlockPool.Pool.Pop();
        block.ResetRand(this, 5);
        SetBlock(block);
    }

    public void SetBlock(Block block)
    {
        if (isCreateField && block == null)
        {
            block = BlockPool.Pool.Pop();
            block.ResetRand(this, 5);
        }
        else if (block == null)
            isEmpty = true;
        else
            isEmpty = false;

        this.block = block;

        if (blockChange != null)
            blockChange(this);
    }

    public void Match()
    {
        if (block != null)
        {
            block.Match();
            BlockPool.Pool.Push(block);
        }
    }

    public Block FindBlockInMyLine()
    {
        if (!isPlayable || !isEmpty)
            throw new Exception();

        BlockField field = prev;

        //현재 필드가 시작 필드가 아니고 빈 필드일경우 이전 필드 탐색 반복
        while (field.IsEmpty && !field.IsFirst)
        {
            field = field.prev;
        }

        //현재 필드가 시작 필드인데도 비어있고 이전 필드가 생성필드가 아닐 경우 블럭을 찾을 수 없음
        if (field.IsEmpty)
        {
            if (field.prev.IsCreateField)
                return field.prev.block;

            return null;
        }

        return field.block;
    }

    #region override Equals
    public override int GetHashCode()
    {
        return row ^ col;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is BlockField))
            return false;

        return Equals((BlockField)obj);
    }

    public bool Equals(BlockField other)
    {
        if (other == null)
            return false;

        if (row != other.row)
            return false;

        return col == other.col;
    }
    public static bool operator ==(BlockField lValue, BlockField rValue)
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

    public static bool operator !=(BlockField lValue, BlockField rValue)
    {
        return !(lValue == rValue);
    }
    #endregion
}