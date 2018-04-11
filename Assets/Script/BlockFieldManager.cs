using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class BlockFieldManager
{
    BlockField[,] fields;

    //배열 길이 -2
    public int RowLength { get; private set; }
    public int ColLength { get; private set; }

    int rowArrLastIdx, colArrLastIdx; //배열 길이 -1
    int lastRow, lastCol; //가장자리 필드를 제외한 마지막 배열 인덱스

    public List<BlockField> ableField = new List<BlockField>();
    public List<BlockField> matchedField = new List<BlockField>();

    public void CleanUp()
    {
        for (int row = 0; row <= rowArrLastIdx; row++)
        {
            for (int col = 0; col < colArrLastIdx; col++)
            {
                fields[row, col].CleanUp();
            }
        }
    }

    // 에디터, 테스터 등에서 호출한다. 
    public BlockFieldManager(BlockField[,] fields)
    {
        this.fields = fields;

        Initialize();
    }

    /// <summary>
    /// 파일로 저장되어 있는 필드를 불러온다. 
    /// </summary>
    public BlockFieldManager(string fieldFileName)
    {
        //필드로드
        fields = FileManager.Inst.EditFileLoad(GlovalVar.FieldDataPath, fieldFileName) as BlockField[,];

        Initialize();

        if (fields == null)
            throw new Exception("check field data " + fieldFileName);

        if (fields.Length * fields.Length < 6)
            throw new Exception("small fields size : " + fieldFileName);

        Debug.Log("fields load : " + fieldFileName + " " + RowLength + " " + ColLength + " " + lastRow + " " + lastCol);
    }

    void Initialize()
    {
        rowArrLastIdx = fields.GetLength(0) - 1;
        colArrLastIdx = fields.GetLength(1) - 1;
        RowLength = rowArrLastIdx - 1;
        ColLength = colArrLastIdx - 1;
        lastRow = RowLength;
        lastCol = ColLength;

        BlockField.fieldMng = this;
    }

    public void BlockInitialize()
    {
        BlockField field;

        //Playable Field 위치에 랜덤 블럭 생성
        for (int i = 0; i <= rowArrLastIdx; i++)
        {
            for (int j = 0; j <= colArrLastIdx; j++)
            {
                field = fields[i, j];

                field.DeployScreen();

                if (field.IsPlayable)
                {
                    if (field.block == null)
                        field.CreateBlock();
                    else
                        field.block.DeployScreen();
                }
                else if (field.IsCreateField)
                    field.CreateBlock();
            }
        }

        //시작과 동시에 매칭 블럭이 없도록 조정
        while(FindMatch())
        {
            for (int i = 0; i < matchedField.Count; i+=2)
            {
                matchedField[i].block.ResetRand(matchedField[i], 5);
            }

            if(!FindMatchAble())
            {
                //임의의 able 패턴을 생성(그냥 셔플->매치가 반복되면 유저가 게임 시작도 안했는데 게임이 진행되는 셈)
            }
        }
    }

    public void Shuffle()
    {
        int row, col;

        //모든 블럭을 한 번씩 랜덤한 위치의 블럭과 교환한다. 
        foreach (var field in GetField())
        {
            row = UnityEngine.Random.Range(1, lastRow);
            col = UnityEngine.Random.Range(1, lastCol);

            SwapBlock(field, fields[row, col], null);
        }
    }

    //event Action BlockMove;

    List<BlockField> movedLineList = new List<BlockField>();

    public IEnumerable<BlockField> GetField()
    {
        for (int i = 1; i <= lastRow; i++)
        {
            for (int j = 1; j <= lastCol; j++)
            {
                yield return fields[i, j];
            }
        }
    }

    //해당 라인의 마지막 Field를 반환한다. 
    BlockField GetLineLast(BlockField field)
    {
        while (!field.IsLast)
            field = field.next;

        return field;
    }

    bool swaping1 = false;
    bool swaping2 = false;
    public void SwapBlock(BlockField selected, BlockField target, Action callback)
    {
        swaping1 = false;
        swaping2 = false;
        swapCallback = callback;
        selected.block.SwapMove(target, () => { swaping1 = true; SwapResult(); });
        target.block.SwapMove(selected, () => { swaping2 = true; SwapResult(); });

        iBlock buffer = selected.block;

        selected.SetBlock(target.block);
        target.SetBlock(buffer);

        selected.block.SetField(selected);
        target.block.SetField(target);
    }

    Action swapCallback;
    void SwapResult()
    {
        if (swaping1 && swaping2 && swapCallback != null)
            swapCallback();
    }

    //void MoveAllBlock()
    //{
    //    BlockMove = null;

    //    BlockField last, cur;

    //    for (int i = 0; i < matchedField.Count; i++)
    //    {
    //        //이미 필드에 블럭이 있을 경우 건너뛴다
    //        if (!matchedField[i].IsEmpty)
    //            continue;

    //        last = GetLineLast(matchedField[i]);

    //        cur = last;

    //        //해당 라인의 시작필드에 도달할 때까지 한 칸씩 역행하며 빈블럭을 채운다. 
    //        while (cur.IsPlayable)
    //        {
    //            Block block = cur.FindBlockInMyLine();

    //            if (block == null)//현재 라인에 이동 가능한 블럭이 없으므로 루프를 중단한다. 
    //                break;

    //            block.SetNextField();
    //            BlockMove += block.MoveToNextField;
    //            cur = cur.prev;
    //        }
    //    }

    //    //이동된 블럭의 화면상의 이동 애니메이션을 실행한다. 
    //    if (BlockMove != null)
    //        BlockMove();
    //}

    public bool FindMatchAble()
    {
        ableField.Clear();

        ChkRowAble();
        ChkColAble();

        return ableField.Count > 0 ? true : false;
    }

    public bool FindMatch()
    {
        matchedField.Clear();

        ChkMatchRow(1, lastRow, 1, lastCol);
        ChkMatchCol(1, lastRow, 1, lastCol);

        return matchedField.Count > 0 ? true : false;
    }

    public void ExcuteMatch()
    {
        for (int i = 0; i < matchedField.Count; i++)
        {
            matchedField[i].Match();
        }
    }

    //필드가 비어있는지 체크한다. 
    public bool IsNotEmpty()
    {
        bool result = true;

        foreach (var field in GetField())
        {
            result &= !field.IsEmpty;
        }

        return result;
    }

    /// <summary>
    ///블럭이 이동중인지 체크한다. 
    /// -블럭이 이동중일 때 매치가 발생할 경우 블럭 재사용에서 문제가 발생한다. 
    /// -유저에게 불완전한 피드백을 줄 수 있기 때문에 블럭이 이동중일 때 이동의 완료를 보장해야 한다.)
    /// </summary>
    public bool IsNotMoving()
    {
        bool result = true;

        foreach (var field in GetField())
        {
            result &= !field.block.IsMoving;
        }

        return result;
    }


    //row와 row+2가 동일한 블럭을 검색해서 일치할 경우 매치가 가능한 패턴인지 확인한다. 
    void ChkRowAble()
    {
        for (int row = 1; row <= lastRow; row++)
        {
            for (int col = 1; col <= lastCol - 2; col++)
            {
                if (fields[row, col].BlockType == fields[row, col + 2].BlockType)
                    ChkRowPattern(row, col);
            }
        }
    }

    void ChkColAble()
    {
        for (int col = 1; col <= lastCol; col++)
        {
            for (int row = 1; row <= lastRow - 2; row++)
            {
                if (fields[row, col].BlockType == fields[row + 2, col].BlockType)
                    ChkColPattern(row, col);
            }
        }
    }



    /// ◆ : 검색 기준
    /// △ : 비교 대상
    /// ■ : 기준 블럭과 동일 블럭
    /// ☆ : 매치가 가능한 위치

    /// 1       2
    /// ◆☆■△ △◆☆■
    /// 
    /// 3     4
    /// ◆☆■ □△□  
    /// □△□ ◆☆■
    void ChkRowPattern(int row, int col)
    {
        if (fields[row, col].BlockType == fields[row, col + 3].BlockType ||
            fields[row, col].BlockType == fields[row, col - 1].BlockType ||
            fields[row, col].BlockType == fields[row + 1, col + 1].BlockType ||
            fields[row, col].BlockType == fields[row - 1, col + 1].BlockType)
        {
            ableField.Add(fields[row, col + 1]);
        }
    }

    /// 1   2   3     4
    /// ◆  △   □◆  ◆□ 
    /// ☆  ◆   △☆  ☆△ 
    /// ■  ☆   □■  ■□ 
    /// △  ■             
    void ChkColPattern(int row, int col)
    {
        if (fields[row, col].BlockType == fields[row + 3, col].BlockType ||
            fields[row, col].BlockType == fields[row - 1, col].BlockType ||
            fields[row, col].BlockType == fields[row + 1, col + 1].BlockType ||
            fields[row, col].BlockType == fields[row + 1, col - 1].BlockType)
        {
            ableField.Add(fields[row + 1, col]);
        }
    }


    void ChkMatchRow(int minRow, int maxRow, int minCol, int maxCol)
    {
        int l, r, cnt;
        int rLimit = maxCol + 1;

        for (int line = minRow; line <= maxRow; line++)
        {
            l = minCol;
            r = minCol + 1;
            cnt = 0;
            while (r < rLimit)
            {
                if (fields[line, r].IsPlayable && fields[line, l].BlockType == fields[line, r].BlockType)
                {
                    cnt++;
                }
                else
                {
                    if (cnt > 1)
                    {
                        //cnt+1 match!
                        for (int i = 0; i <= cnt; i++)
                        {
                            matchedField.Add(fields[line, l + i]);
                            //Debug.Log("Row Match (" + line + ", " + (l + i).ToString() + ") " + fields[line,l+i].BlockType.type);
                        }
                    }

                    l = r;
                    cnt = 0;
                }
                r++;
            }
            if (cnt > 1)
            {
                //cnt+1 match!
                for (int i = 0; i <= cnt; i++)
                {
                    matchedField.Add(fields[line, l + i]);
                    //Debug.Log("Row Match (" + line + ", " + (l + i).ToString() + ") " + fields[line, l + i].BlockType.type);
                }
            }
        }
    }

    void ChkMatchCol(int minRow, int maxRow, int minCol, int maxCol)
    {
        int l, r, cnt;
        int rLimit = maxRow + 1;

        for (int line = minCol; line <= maxCol; line++)
        {
            l = minRow;
            r = minRow + 1;
            cnt = 0;
            while (r < rLimit)
            {
                if (fields[r, line].IsPlayable && fields[l, line].BlockType == fields[r, line].BlockType)
                {
                    cnt++;
                }
                else
                {
                    if (cnt > 1)
                    {
                        //cnt+1 match!
                        for (int i = 0; i <= cnt; i++)
                        {
                            matchedField.Add(fields[l + i, line]);
                            //Debug.Log("Col Match (" + (l + i).ToString() + ", " + line + ")" + fields[l + i, line].BlockType.type);
                        }
                    }

                    l = r;
                    cnt = 0;
                }
                r++;
            }
            if (cnt > 1)
            {
                //cnt+1 match!
                for (int i = 0; i <= cnt; i++)
                {
                    matchedField.Add(fields[l + i, line]);
                    //Debug.Log("Col Match (" + (l + i).ToString() + ", " + line + ")" + fields[l + i, line].BlockType.type);
                }
            }
        }
    }

    #region GetFields
    public IEnumerable<BlockField> Arounds(BlockField centerField)
    {
        for (int i = centerField.Row - 1; i < 3; i++)
        {
            for (int j = centerField.Col - 1; j < 3; j++)
            {
                yield return fields[i, j];
            }
        }
    }

    public BlockField GetNextByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 8);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 4);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 2);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 6);
        }
    }

    public BlockField GetPrevByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 2);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 6);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 8);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 4);
        }
    }

    public BlockField GetRightByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 6);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 8);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 4);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 2);
        }
    }

    public BlockField GetLeftByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 4);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 2);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 6);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 8);
        }
    }

    public BlockField GetRightDiagnalByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 3);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 9);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 7);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 1);
        }
    }

    public BlockField GetLeftDiagnalByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 1);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 3);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 9);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 7);
        }
    }

    /// <summary>
    /// 1 2 3
    /// 4   6
    /// 7 8 9
    /// 5 위치를 기준으로 숫자에 해당하는 상대위치의 필드 반환
    /// </summary>
    public BlockField GetBlockField(int row, int col, int dir=5)
    {
        switch (dir)
        {
            default:
            case 5:
                return fields[row, col];
            case 1:
                if (row <= 0 || col <= 0)
                    return null;
                if (row > rowArrLastIdx || col > colArrLastIdx)
                    return null;
                return fields[row - 1, col - 1];
            case 2:
                if (row <= 0)
                    return null;
                if (row > rowArrLastIdx || col < 0 || col > colArrLastIdx)
                    return null;
                return fields[row - 1, col];
            case 3:
                if (row <= 0 || col >= colArrLastIdx)
                    return null;
                if (row > rowArrLastIdx || col < 0)
                    return null;
                return fields[row - 1, col + 1];
            case 4:
                if (col <= 0)
                    return null;
                if (row < 0 || row > rowArrLastIdx || col > colArrLastIdx)
                    return null;
                return fields[row, col - 1];
            case 6:
                if (col >= colArrLastIdx)
                    return null;
                if (row < 0 || row > rowArrLastIdx || col < 0)
                    return null;
                return fields[row, col + 1];
            case 7:
                if (row >= rowArrLastIdx || col <= 0)
                    return null;
                if (row < 0 || col > colArrLastIdx)
                    return null;
                return fields[row + 1, col - 1];
            case 8:
                if (row >= rowArrLastIdx)
                    return null;
                if (row < 0 || col < 0 || col > colArrLastIdx)
                    return null;
                return fields[row + 1, col];
            case 9:
                if (row >= rowArrLastIdx || col >= colArrLastIdx)
                    return null;
                if (row < 0 || col < 0)
                    return null;
                return fields[row + 1, col + 1];
        }
    }
    #endregion
}