using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public interface iBlockField
{
    BlockField self { get; }
    BlockField prev { get; }
    BlockField next { get; }
    float X { get; }
    float Y { get; }
    bool IsMoveable { get; }
    void SetBlock(iBlock block);
    BlockField GetDiagnalField();
    void OnPushbackBlock();
}

/// <summary>
/// 블럭의 이동 경로 관리
/// 블럭의 생성
/// </summary>
[Serializable]
public class BlockField : iBlockField
{
    public static BlockFieldManager fieldMng;

    public iBlock block;
    public int BlockType { get { return block == null ? -1 : block.BlockType; } }

    public BlockField self { get { return this; } }
    public BlockField prev { get; private set; }
    public BlockField next { get; private set; }

    public int Row { get; private set; }
    public int Col { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public bool IsPlayable { get { return isPlayable; } }
    public bool IsEmpty { get { return isEmpty; } }
    public bool IsCreateField { get { return isCreateField; } }
    public bool IsMoveable { get { return isMoveable; } }
    public bool IsLast { get { return !next.isMoveable; } }
    public bool IsFirst { get { return !prev.isMoveable; } }
    public bool IsDeadline { get { return isDeadline; } set { isDeadline = value; } }
    /// <summary>
    /// 0:down 1:left 2:up 3:right
    /// </summary>
    public int Direction { get { return direction; } }

    [Obsolete] public bool InProgress { get; set; }
    [NonSerialized] [Obsolete] bool inProgress;

    bool isBorderLine = false;
    bool isPlayable = true;
    bool isEmpty = true;
    bool isCreateField = false;
    bool isMoveable = true;
    bool isDeadline = false;
    int direction = 0; //0:down 1:left 2:up 3:right

    iBlockField[] arrPrev;

    [NonSerialized] int prevIdx = 0;
    [NonSerialized] iBlockFieldGO blockFieldGO;
    [NonSerialized] static List<iBlockField> bufferForSetPrev;

    #region Call By Editor
    public BlockField(int row, int col)
    {
        this.Row = row;
        this.Col = col;
    }

    public void Initialize()
    {
        SetDirection(0);

        X = Col;
        Y = fieldMng.RowLength - Row + 1;

        string log = "Initialize // " + X + " " + Y;
        if (next != null)
            log += " next : " + next.X + " " + next.Y;
        if (prev != null)
            log += " prev : " + prev.X + " " + prev.Y;
        Debug.Log(log);
    }

    public bool ValidateField()
    {
        if (isPlayable && prev == null)
        {
            Debug.LogError(X + " " + Y + " " + isPlayable + " // playable field 는 반드시 prev field가 존재해야 합니다. ");
            return false;
        }
        if (isPlayable && next == null)
        {
            Debug.LogError(X + " " + Y + " // playable field 는 반드시 prev field가 존재해야 합니다. ");
            return false;
        }



        if (prev == next && next.isPlayable)
        {
            Debug.LogError(X + " " + Y + " // 방향이 충돌합니다. ");
            return false;
        }

        if (next != fieldMng.GetNextByDir(this))
        {
            Debug.LogError(X + " " + Y + " // 방향과 지정된 next 필드가 일치하지 않습니다. ");
            return false;
        }

        if (this == next.next)
        {
            Debug.LogError("next가 서로를 가르키고 있음 무한루프 상태임 " + X + " " + Y);
            return false;
        }

        if (this == prev.prev)
        {
            Debug.LogError("prev가 서로를 가르키고 있음 상태임 " + X + " " + Y);
            return false;
        }

        if (this == next)
        {
            Debug.LogError("next가 자신임 " + X + " " + Y);
            return false;
        }

        if (this == prev)
        {
            Debug.LogError("prev가 자신임 " + X + " " + Y);
            return false;
        }

        if (isPlayable && next != null && next.prev != null && next.prev != this)
        {
            Debug.LogWarning(X + " " + Y + " // next Prev가 this가 아님 // next : " +
                next.X + " " + next.Y + " // next.prev : " + next.prev.X + " " + next.prev.Y);
        }

        return true;
    }

    public void SetBorderLine()
    {
        isBorderLine = true;
        SetNonPlayable();
    }

    public void SetPlayable()
    {
        isPlayable = true;

        SetMoveable();
        UpdateGO();
    }

    public void SetNonPlayable()
    {
        isPlayable = false;

        if (block != null)
        {
            block.CleanUp();
            block = null;
        }

        SetMoveable();
        UpdateGO();
    }

    public void SetCreateField(bool bValue)
    {
        isCreateField = bValue;
        SetDeadline(!isCreateField);

        UpdateGO();
    }

    public void SetDeadline(bool isDeadline)
    {
        BlockField field = this;

        while (field.isMoveable)
        {
            field.isDeadline = isDeadline;
            field = field.next;

            if (field == this)
                throw new Exception("Check next");
        }
    }

    public void SetDirection(int dir)
    {
        direction = dir;

        next = fieldMng.GetNextByDir(this);

        if (next != null)
        {
            if (Mathf.Abs(next.X - X) > 1 || Mathf.Abs(next.Y - Y) > 1)
                Debug.LogError("next 필드가 인접필드가 아님");

            next.SetPrevArray();
            next.prev = this;
        }

        UpdateGO();
    }

    public void SetPrevArray()
    {
        if (bufferForSetPrev == null)
            bufferForSetPrev = new List<iBlockField>();
        else
            bufferForSetPrev.Clear();

        foreach (BlockField field in fieldMng.GetAroundsFour(this))
        {
            if (field != null && field.IsPlayable && field.next == this)
                bufferForSetPrev.Add(field);
        }

        arrPrev = bufferForSetPrev.ToArray() as iBlockField[];

        if (arrPrev.Length > 1)
            Debug.LogWarning("Multi Prev " + X + " " + Y + " " + arrPrev.Length);

    }

    void UpdateGO()
    {
        if (blockFieldGO != null)
            blockFieldGO.ChangeFieldProperty();
    }

    public void SetBlockRandom()
    {
        CleanUpBlock();
    }

    public void SetBlockType(int blockType)
    {
        if (block == null)
            CreateBlock(blockType);
        else
            block.Reset(this, blockType);
    }
    #endregion

    public void DeployScreen()
    {
        if (!isBorderLine)
        {
            blockFieldGO = BlockFieldGOPool.pool.Pop();
            blockFieldGO.SetBlockField(this);
        }
    }
    // call by blockFieldGO.FixedUpdate()
    public void Update()
    {
        if (isEmpty)
        {
            if (!prev.isEmpty)
            {
                prev.block.MoveToNextField();
                //Debug.Log(X + " " + Y + " // prev : " + prev.X + " " + prev.Y);
            }
            //else if (!next.isEmpty || !next.IsMoveable)
            //{
            //    DiagnalProcess();
            //}
        }
    }


    //nonPlayable 이거나 field가 특수 상태이거나 block이 특수 상태일경우 
    void SetMoveable()
    {
        if (!isPlayable)
            isMoveable = false;
        else
            isMoveable = true;

        SetDeadline(!isMoveable);
    }

    public void CreateBlock()
    {
        iBlock block = BlockMng.Pool.Pop();
        block.ResetRand(this, 5);
        SetBlock(block);
    }

    public void CreateBlock(int blockType)
    {
        iBlock block = BlockMng.Pool.Pop();
        block.Reset(this, blockType);
        SetBlock(block);
    }


    public void CleanUp()
    {
        CleanUpBlock();

        if (blockFieldGO != null)
        {
            blockFieldGO.PushBack();
            BlockFieldGOPool.pool.Push(blockFieldGO);
        }
    }

    private void CleanUpBlock()
    {
        if (block != null)
        {
            block.CleanUp();
            BlockMng.Pool.Push(block);
            block = null;
        }
    }

    //진입방향이 2개이상일 경우 블록이 지날 때마다 진입방향을 변경한다. 
    void UpdatePrev()
    {
        if (arrPrev != null && arrPrev.Length > 1)
        {
            if (prevIdx == arrPrev.Length)
                prevIdx = 0;

            prev = arrPrev[prevIdx].self;
            prevIdx++;

            //Debug.LogWarning(X + " " + Y + "   UpdatePrev " + prev.X + " " + prev.Y + "   " + arrPrev[0].self.X + " " + arrPrev[0].self.Y);
        }
        else if (arrPrev != null && arrPrev.Length > 0)
        {

            prev = arrPrev[0].self;

            //Debug.LogWarning(X + " " + Y + "   UpdatePrev " + prev.X + " " + prev.Y + "   " + arrPrev[0].self.X + " " + arrPrev[0].self.Y);
        }
    }

    public void SetBlock(iBlock block)
    {
        if (isCreateField && block == null)
        {
            block = BlockMng.Pool.Pop();
            if (block == null)
                Debug.LogError("block is null");
            block.ResetRand(this, 5);
        }
        else if (block == null)
        {
            isEmpty = true;
        }
        else
        {
            UpdatePrev();
            isEmpty = false;
        }

        this.block = block;
    }

    public void Match()
    {
        if (block != null)
        {
            block.Match();
        }
    }

    bool CheckBlockInLinePrev()
    {
        BlockField field = prev;

        while (field.isMoveable)
        {
            if (!field.IsEmpty)
                return true;

            field = field.prev;

            if (field == this)
                throw new Exception("infinity loop. check prev");
        }

        return false;
    }

    public BlockField GetDiagnalField()
    {
        BlockField target = null;

        target = fieldMng.GetLeftDownDiagnalByDir(this);
        if (CheckDiagnalMove(target))
            return target;

        target = fieldMng.GetRightDownDiagnalByDir(this);
        if (CheckDiagnalMove(target))
            return target;

        return null;
    }

    //좌우필드의 대각이동 필요 여부를 검사한다
    bool CheckDiagnalMove(BlockField target)
    {
        if (!target.isEmpty)
            return false;
        if (!target.isDeadline)
            return false;
        if (target.CheckBlockInLinePrev())
            return false;

        return true;
    }

    //블럭이 매칭 이펙트등을 마치고 cleanup을 완료하면 호출한다. 
    public void OnPushbackBlock()
    {
        BlockMng.Pool.Push(block);
        SetBlock(null);
    }

    //#region 대각선 블럭이동

    //[NonSerialized] BlockField left, right, leftDiagnal, rightDiagnal;
    //[NonSerialized] BlockField lDiagnalReq, rDiagnalReq;
    //[NonSerialized] bool isMakeDiagnalFieldRef = false;
    //[NonSerialized] bool canUseLeftForDiagnal = false;
    //[NonSerialized] bool canUseRightForDiagnal = false;
    //[NonSerialized] bool isDiagnalMode = false;
    //[NonSerialized] bool diagnalLRFrag = false;

    //void SetLDiagnalNext(BlockField requester)
    //{
    //    lDiagnalReq = requester;
    //    SetDiagnalNext();
    //}

    //void SetRDiagnalNext(BlockField requester)
    //{
    //    rDiagnalReq = requester;
    //    SetDiagnalNext();
    //}

    //void SetDiagnalNext()
    //{
    //    Debug.Log("SetDiagnalNext " + X + " " + Y + " // next : " + next.X + " " + next.Y +
    //        " // next.prev : " + next.prev.X + " " + next.prev.Y);
    //    if (lDiagnalReq != null && rDiagnalReq != null)
    //    {
    //        if (diagnalLRFrag)
    //            next = lDiagnalReq;
    //        else
    //            next = rDiagnalReq;

    //        diagnalLRFrag = !diagnalLRFrag;
    //    }
    //    else if (lDiagnalReq != null)
    //        next = lDiagnalReq;
    //    else if (rDiagnalReq != null)
    //        next = rDiagnalReq;
    //    else
    //        next = fieldMng.GetNextByDir(this);
    //}

    //int CountPrevBlockInLine()
    //{
    //    int result = 0;

    //    //while(!prev.isEmpty)
    //    //{
    //    //    result++;
    //    //    prev = prev.prev;
    //    //}

    //    return result;
    //}

    ////현재 필드가 비어 있고 다음필드가 비어있지 않은 경우 호출된다. 
    //void DiagnalProcess()
    //{
    //    if (!isDiagnalMode && !CheckBlockOrCreateInLine())
    //    {
    //        if (!isMakeDiagnalFieldRef)
    //            MakeDiagnalFieldRef();

    //        //측면의 블럭 여부 등을 참조해 좌우의 대각필드 가능여부를 확인한다. 
    //        bool canL, canR;
    //        canL = canR = false;

    //        if (canUseLeftForDiagnal && left != null && !left.IsEmpty)
    //            canL = true;

    //        if (canUseRightForDiagnal && right != null && !right.isEmpty)
    //            canR = true;


    //        if (canL && canR)
    //        {
    //            //양쪽이 다 가능할 경우 블럭이 더 많은 쪽을 지정한다. 
    //            if (leftDiagnal.CountPrevBlockInLine() > rightDiagnal.CountPrevBlockInLine())
    //                canR = false;
    //            else
    //                canL = false;
    //        }

    //        if (canL)
    //        {
    //            //대각필드 사용!!
    //            Debug.Log("Use Left");
    //            prev = leftDiagnal;
    //            leftDiagnal.SetRDiagnalNext(this);
    //            isDiagnalMode = true;
    //        }
    //        else if (canR)
    //        {
    //            Debug.Log("Use Right");
    //            prev = rightDiagnal;
    //            rightDiagnal.SetLDiagnalNext(this);
    //            isDiagnalMode = true;
    //        }
    //    }
    //}

    ////연결 라인에 블럭이나 생성필드가 있는지 검사한다. 
    //bool CheckBlockOrCreateInLine()
    //{
    //    BlockField field = this;
    //    string log = "";
    //    int count = 0;

    //    //시작 필드에 도달할 때까지 탐색한다. 
    //    while (field.isMoveable)
    //    {
    //        if (!field.IsEmpty || field.IsCreateField)
    //            return true;

    //        field = field.prev;

    //        log += "(" + field.X + "," + field.Y + ")";

    //        count++;
    //        if (count > 300)
    //            throw new Exception("infinity loop?? " + log);
    //    }

    //    //블럭이나 생성필드가 라인에 존재하지 않는다. 
    //    return false;
    //}

    ////대각 필드의 참조를 캐싱하고 대각필드의 조건을 만족하는지 확인한다. 
    //void MakeDiagnalFieldRef()
    //{
    //    left = fieldMng.GetLeftByDir(this);
    //    leftDiagnal = fieldMng.GetLeftDiagnalByDir(this);
    //    right = fieldMng.GetRightByDir(this);
    //    rightDiagnal = fieldMng.GetRightDiagnalByDir(this);

    //    //대각 필드와 진행방향이 동일한 경우만 대각이동을 허용한다. 

    //    if (leftDiagnal != null && leftDiagnal.Direction == Direction)
    //        canUseLeftForDiagnal = true;

    //    if (rightDiagnal != null && rightDiagnal.Direction == Direction)
    //        canUseRightForDiagnal = true;

    //    isMakeDiagnalFieldRef = true;
    //    Debug.Log("MakeDiagnalFieldRef");
    //}

    //void RecoverDiagnalField()
    //{
    //    if (!isDiagnalMode)
    //        return;

    //    Debug.Log("RecoverDiagnal");

    //    if (leftDiagnal != null)
    //        leftDiagnal.SetRDiagnalNext(null);

    //    if (rightDiagnal != null)
    //        rightDiagnal.SetLDiagnalNext(null);

    //    UpdatePrev();
    //    isDiagnalMode = false;
    //}


    //#endregion

    #region override Equals
    public override int GetHashCode()
    {
        return Row ^ Col;
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

        if (Row != other.Row)
            return false;

        return Col == other.Col;
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