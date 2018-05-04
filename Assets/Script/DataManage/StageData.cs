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

    public void AddStage(string stageName, BlockField[,] stageData, ClearCondition[] clearData=null)
    {
        stageFileList.AddFirst(stageName);
        FileManager.Inst.EditFileSave(GlobalVal.FieldDataPath, stageName, stageData);
        FileManager.Inst.EditFileSave(GlobalVal.ClearConditionDataPath, stageName, clearData);

    }

    public void RemoveStage(string stageName)
    {
        stageFileList.Remove(stageName);
        FileManager.Inst.EditFileDelete(GlobalVal.FieldDataPath, stageName);
        FileManager.Inst.EditFileDelete(GlobalVal.ClearConditionDataPath, stageName);
    }

    public void SaveStageFields(string stageName, BlockField[,] stageData)
    {
        FileManager.Inst.EditFileSave(GlobalVal.FieldDataPath, stageName, stageData);
    }

    public void SaveStageClearConditions(string stageName, ClearCondition[] clearData)
    {
        FileManager.Inst.EditFileSave(GlobalVal.ClearConditionDataPath, stageName, clearData);
    }

    public BlockField[,] LoadStageFields(string stageName)
    {
        return FileManager.Inst.EditFileLoad(GlobalVal.FieldDataPath, stageName) as BlockField[,];
    }

    public ClearCondition[] LoadStageClearCondition(string stageName)
    {
        return FileManager.Inst.EditFileLoad(GlobalVal.ClearConditionDataPath, stageName) as ClearCondition[];
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
