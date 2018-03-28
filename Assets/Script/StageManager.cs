using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class StageManager
{
    BlockField[,] fields;

    int rowLength, lastRow;
    int colLength, lastCol;

    public List<BlockField> ableField = new List<BlockField>();
    public List<BlockField> matchedField = new List<BlockField>();

    public void LoadFields(string fieldFileName)
    {
        //필드로드
        fields = FileManager.Inst.EditFileLoad(GlovalVar.FieldDataPath, fieldFileName) as BlockField[,];

        if (fields == null)
            throw new Exception("check field data " + fieldFileName);

        if (fields.Length * fields.Length < 6)
            throw new Exception("small fields size : " + fieldFileName);

        rowLength = fields.GetLength(0)-2;
        colLength = fields.GetLength(1)-2;
        lastRow = rowLength;
        lastCol = colLength;

        Debug.Log("stage field load : " + fieldFileName + " " + rowLength + " " + colLength + " " + lastRow + " " + lastCol);

        BlockInitialize();
    }

    void BlockInitialize()
    {
        //Playable Field 위치에 랜덤 블럭 생성
    }

    event Action BlockMove;

    List<BlockField> movedLineList = new List<BlockField>();

    //이미 처리된 lastField인지 확인하고 아닐 경우 버퍼에 삽입한다. 
    bool ChkMovedLine(BlockField last)
    {
        for (int i = 0; i < movedLineList.Count; i++)
        {
            if (movedLineList[i] == last)
                return true;
        }

        movedLineList.Add(last);

        return false;
    }

    //해당 라인의 마지막 Field를 반환한다. 
    BlockField GetLineLast(BlockField field)
    {
        while (!field.IsLast)
            field = field.next;

        return field;
    }
    
    void MoveAllBlock()
    {
        movedLineList.Clear();
        BlockMove = null;

        BlockField last, cur;

        for (int i = 0; i < matchedField.Count; i++)
        {
            last = GetLineLast(matchedField[i]);

            //해당 라인이 이미 처리됐다면 Skip 한다
            if (ChkMovedLine(last))
                continue;

            cur = last;

            //해당 라인의 시작필드에 도달할 때까지 한 칸씩 역행하며 빈블럭을 채운다. 
            while (!cur.IsFirst)
            {
                Block block = cur.FindBlockInMyLine();
                block.SetNextField();
                BlockMove += block.MoveToNextField;
                cur = cur.prev;
            }
        }

        //이동된 블럭의 화면상의 이동 애니메이션을 실행한다. 
        if (BlockMove != null)
            BlockMove();
    }

    public void FindMatchAble()
    {
        ableField.Clear();

        ChkRowAble();
        ChkColAble();
    }

    public void FindMatch()
    {
        matchedField.Clear();

        ChkMatchRow(1, lastRow, 1, lastCol);
        ChkMatchCol(1, lastRow, 1, lastCol);
    }

    public void ExcuteMatch()
    {
        FindMatch();

        for (int i = 0; i < matchedField.Count; i++)
        {
            matchedField[i].Match();
        }
    }


    //row와 row+2가 동일한 블럭을 검색해서 일치할 경우 매치가 가능한 패턴인지 확인한다. 
    void ChkRowAble()
    {
        for (int row = 1; row <= lastRow; row++)
        {
            for (int col = 1; col <= lastCol-2; col++)
            {
                if (fields[row,col].BlockType == fields[row,col + 2].BlockType)
                    ChkRowPattern(row, col);
            }
        }
    }

    void ChkColAble()
    {
        for (int col = 1; col <= lastCol; col++)
        {
            for (int row = 1; row <= lastRow-2; row++)
            {
                if (fields[row,col].BlockType == fields[row + 2,col].BlockType)
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
        if (fields[row,col].BlockType == fields[row,col + 3].BlockType ||
            fields[row,col].BlockType == fields[row,col - 1].BlockType ||
            fields[row,col].BlockType == fields[row + 1,col + 1].BlockType ||
            fields[row,col].BlockType == fields[row - 1,col + 1].BlockType)
        {
            ableField.Add(fields[row,col + 1]);
        }
    }

    /// 1   2   3     4
    /// ◆  △   □◆  ◆□ 
    /// ☆  ◆   △☆  ☆△ 
    /// ■  ☆   □■  ■□ 
    /// △  ■             
    void ChkColPattern(int row, int col)
    {
        if (fields[row,col].BlockType == fields[row + 3,col].BlockType ||
            fields[row,col].BlockType == fields[row - 1,col].BlockType ||
            fields[row,col].BlockType == fields[row + 1,col + 1].BlockType ||
            fields[row,col].BlockType == fields[row + 1,col - 1].BlockType)
        {
            ableField.Add(fields[row + 1,col]);
        }
    }


    void ChkMatchRow(int minRow, int maxRow, int minCol, int maxCol)
    {
        int l, r, cnt;
        int rLimit = maxCol + 1;

        for (int line = minRow; line <= maxRow; line++)
        {
            l = minCol;
            r = minCol+1;
            cnt = 0;
            while (r < rLimit)
            {
                if (fields[line,r].IsPlayable && fields[line,l].BlockType == fields[line,r].BlockType)
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
                            matchedField.Add(fields[line,l + i]);
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
            r = minRow+1;
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
}