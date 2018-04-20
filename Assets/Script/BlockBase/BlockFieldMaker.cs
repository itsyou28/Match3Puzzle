using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class BlockFieldMaker
{
    public static BlockField[,] CreateField(int maxRow, int maxCol)
    {
        maxRow += 2; maxCol += 2;

        BlockField[,] fields = new BlockField[maxRow, maxCol];

        for (int row = 0; row < maxRow; row++)
        {
            for (int col = 0; col < maxCol; col++)
            {
                fields[row, col] = new BlockField(row, col);

                if (row == 0 || row == maxRow - 1 || col == 0 || col == maxCol - 1)
                    fields[row, col].SetBorderLine();

                if (row == 1 && col >= 1 && col <maxCol-1)
                {
                    fields[row, col].SetCreateField(true);
                    fields[row, col].SetNonPlayable();
                }
            }
        }

        BlockFieldManager fieldMng = new BlockFieldManager(fields);

        for (int row = 0; row < maxRow; row++)
        {
            for (int col = 0; col < maxCol; col++)
            {
                fields[row, col].Initialize();
            }
        }

        return fields;
    }

    public static bool ValidateField(BlockField[,] target)
    {
        int rowMax = target.GetLength(0);
        int colMax = target.GetLength(1);
        bool result = true;
        for (int row = 0; row < rowMax; row++)
        {
            for (int col = 0; col < colMax; col++)
            {
                result &= target[row, col].ValidateField();
            }
        }

        return result;
    }
}
