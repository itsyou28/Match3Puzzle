using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class BlockFieldMaker
{
    private static BlockFieldMaker instance = null;
    public static BlockFieldMaker Inst
    {
        get
        {
            if (instance == null)
                instance = new BlockFieldMaker();

            return instance;
        }
    }

    Dictionary<string, BlockField[,]> dicEditFields = new Dictionary<string, BlockField[,]>();

    string curFieldsName;
    BlockField[,] curEditFields;
    BlockFieldManager fieldMng;

    public BlockField[,] CreateField(int maxRow, int maxCol)
    {
        maxRow += 2; maxCol += 2;

        BlockField[,] fields = new BlockField[maxRow, maxCol];

        for (int row = 0; row < maxRow; row++)
        {
            for (int col = 0; col < maxCol; col++)
            {
                fields[row, col] = new BlockField(row, col);

                if (row == 0)
                {
                    fields[row, col].SetCreateField(true);
                    fields[row, col].SetNonPlayable();
                }
                if (row == maxRow - 1 || col == 0 || col == maxCol - 1)
                    fields[row, col].SetNonPlayable();
            }
        }

        fieldMng = new BlockFieldManager(fields);

        for (int row = 0; row < maxRow; row++)
        {
            for (int col = 0; col < maxCol; col++)
            {
                fields[row, col].Initialize();
            }
        }

        return fields;
    }

    public bool ValidateField()
    {
        int rowMax = curEditFields.GetLength(0);
        int colMax = curEditFields.GetLength(1);
        bool result = true;
        for (int row = 0; row < rowMax; row++)
        {
            for (int col = 0; col < colMax; col++)
            {
                result &= curEditFields[row, col].ValidateField();
            }
        }

        return result;
    }

    public void CreateField(int maxRow, int maxCol, string fieldName)
    {
        curFieldsName = fieldName;
        curEditFields = CreateField(maxRow, maxCol);

        if (dicEditFields.ContainsKey(fieldName))
        {
            dicEditFields[fieldName] = curEditFields;
        }
        else
        {
            dicEditFields.Add(fieldName, curEditFields);
        }
    }

    public void SaveField()
    {
        FileManager.Inst.EditFileSave(GlovalVar.FieldDataPath, curFieldsName, curEditFields);
        Debug.Log("Save Field " + curFieldsName + " " + curEditFields.GetLength(0) + " " + curEditFields.GetLength(1));
    }

    public void SaveField(string fieldName)
    {
        if (dicEditFields.ContainsKey(fieldName))
        {
            FileManager.Inst.EditFileSave(GlovalVar.FieldDataPath, fieldName, dicEditFields[fieldName]);
            Debug.Log("Save Field " + curFieldsName + " " + dicEditFields[fieldName].GetLength(0) + " " + dicEditFields[fieldName].GetLength(1));
        }
    }

    public BlockField[,] LoadField(string fieldName)
    {
        curFieldsName = fieldName;

        if (dicEditFields.ContainsKey(fieldName))
            curEditFields = dicEditFields[fieldName];
        else
        {
            curEditFields = FileManager.Inst.EditFileLoad(GlovalVar.FieldDataPath, fieldName) as BlockField[,];
            dicEditFields.Add(fieldName, curEditFields);
        }

        return curEditFields;
    }

    public void MakeTestField(int[,] arr, string fieldName)
    {
        CreateField(arr.GetLength(0), arr.GetLength(1), fieldName);

        Debug.Log(curEditFields.GetLength(0) + " " + curEditFields.GetLength(1) + " // " + arr.GetLength(0) + " " + arr.GetLength(1));
        for (int row = 0; row < arr.GetLength(0); row++)
        {
            for (int col = 0; col < arr.GetLength(1); col++)
            {
                Block block = new Block();
                block.InitByEditor(curEditFields[row + 1, col + 1], arr[row, col]);
                curEditFields[row + 1, col + 1].SetBlock(block);
            }
        }

        SaveField();
    }
}
