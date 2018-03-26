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
    public List<Block> matchBlock = new List<Block>();

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
        //블럭생성 및 배치
        //for (int i = 1; i < lastRow; i++)
        //{
        //    for (int j = 1; j < lastCol; j++)
        //    {
        //        if(fields[i,j].IsPlayable)
        //        {
        //            fields[i,j].block = new Block();
        //        }
        //    }
        //}
    }

    public void FindMatchAble()
    {
        ableField.Clear();

        ChkRowAble();
        ChkColAble();
    }

    public void FindMatch()
    {
        matchBlock.Clear();

        ChkMatchRow(1, lastRow, 1, lastCol);
        ChkMatchCol(1, lastRow, 1, lastCol);
    }

    public void ExcuteMatch()
    {
        FindMatch();

        for (int i = 0; i < matchBlock.Count; i++)
        {
            matchBlock[i].Match();
        }
    }

    void ChkRowAble()
    {
        for (int row = 1; row <= lastRow; row++)
        {
            for (int col = 1; col <= lastCol-2; col++)
            {
                if (fields[row,col].block == fields[row,col + 2].block)
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
                if (fields[row,col].block == fields[row + 2,col].block)
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
        if (fields[row,col].block == fields[row,col + 3].block ||
            fields[row,col].block == fields[row,col - 1].block ||
            fields[row,col].block == fields[row + 1,col + 1].block ||
            fields[row,col].block == fields[row - 1,col + 1].block)
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
        if (fields[row,col].block == fields[row + 3,col].block ||
            fields[row,col].block == fields[row - 1,col].block ||
            fields[row,col].block == fields[row + 1,col + 1].block ||
            fields[row,col].block == fields[row + 1,col - 1].block)
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
                if (fields[line,r].IsPlayable && fields[line,l].block == fields[line,r].block)
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
                            matchBlock.Add(fields[line,l + i].block);
                            //Debug.Log("Row Match (" + line + ", " + (l + i).ToString() + ") " + fields[line,l+i].block.type);
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
                    matchBlock.Add(fields[line, l + i].block);
                    //Debug.Log("Row Match (" + line + ", " + (l + i).ToString() + ") " + fields[line, l + i].block.type);
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
                    if (fields[r, line].IsPlayable && fields[l, line].block == fields[r, line].block)
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
                                matchBlock.Add(fields[l + i, line].block);
                                //Debug.Log("Col Match (" + (l + i).ToString() + ", " + line + ")" + fields[l + i, line].block.type);
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
                    matchBlock.Add(fields[l + i, line].block);
                    //Debug.Log("Col Match (" + (l + i).ToString() + ", " + line + ")" + fields[l + i, line].block.type);
                }
            }
        }
    }
}