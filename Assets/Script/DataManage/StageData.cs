using System;
using System.Collections.Generic;

class StageData : iDataFile
{
    LinkedList<string> stageFileList;

    public IEnumerator<string> fileListItor
    {
        get
        {
            return stageFileList.GetEnumerator();
        }
    }

    public bool CheckExist(string stageName)
    {
        foreach (var name in stageFileList)
        {
            if (stageName == name)
                return true;
        }

        return false;
    }

    public void AddStage(string stageName, BlockField[,] stageData)
    {
        stageFileList.AddFirst(stageName);
        FileManager.Inst.EditFileSave(GlobalVal.FieldDataPath, stageName, stageData);

    }

    public void RemoveStage(string stageName)
    {
        stageFileList.Remove(stageName);
        FileManager.Inst.EditFileDelete(GlobalVal.FieldDataPath, stageName);

    }

    public void SaveStage(string stageName, BlockField[,] stageData)
    {
        FileManager.Inst.EditFileSave(GlobalVal.FieldDataPath, stageName, stageData);
    }

    public BlockField[,] LoadStage(string stageName)
    {
        //BlockField[,] result;

        //if(!dicCachFields.TryGetValue(stageName, out result))
        //{
        //    result = FileManager.Inst.EditFileLoad(GlobalVal.FieldDataPath, stageName) as BlockField[,];
        //    dicCachFields.Add(stageName, result);
        //}

        //return result;
        return FileManager.Inst.EditFileLoad(GlobalVal.FieldDataPath, stageName) as BlockField[,];
    }

    public void Load()
    {
        stageFileList = FileManager.Inst.EditFileLoad("data", "stageFileList") as LinkedList<string>;

        if (stageFileList == null)
            stageFileList = new LinkedList<string>();
    }
    public void Save()
    {
        FileManager.Inst.EditFileSave("data", "stageFileList", stageFileList);
    }
}
