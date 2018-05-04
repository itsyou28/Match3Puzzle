using System.Collections.Generic;
using UnityEngine;

public interface iClearConditionMaker
{
    void AddCondition(int blockType);
    void RemoveCondition(int blockType);
    void SetCount(int blockType, int requierCnt);
    IEnumerator<ClearCondition> GetEnumerator();
}

public class ClearConditionMaker : iClearConditionMaker
{
    Dictionary<int, ClearCondition> dic = new Dictionary<int, ClearCondition>();

    string stageName;

    public void AddCondition(int blockType)
    {
        ClearCondition newCond = new ClearCondition();
        newCond.blockType = blockType;

        dic.Add(blockType, newCond);
    }

    public void RemoveCondition(int blockType)
    {
        if (dic.ContainsKey(blockType))
            dic.Remove(blockType);
    }

    public void SetCount(int blockType, int requierCnt)
    {
        ClearCondition condition = new ClearCondition();
        condition.blockType = blockType;
        condition.requireCnt = requierCnt;

        if (dic.ContainsKey(blockType))
            dic[blockType] = condition;
    }

    ClearCondition[] GetConditionArr()
    {
        ClearCondition[] result = new ClearCondition[dic.Count];

        int idx = 0;
        foreach (var item in dic.Values)
        {
            result[idx] = item;
        }

        return result;
    }

    public IEnumerator<ClearCondition> GetEnumerator()
    {
        return dic.Values.GetEnumerator();
    }

    public void Init(string stageName)
    {
        this.stageName = stageName;

        ClearCondition[] conditions = DataManager.Inst.stageData.LoadStageClearCondition(stageName);

        if(conditions == null)
        {
            Debug.LogWarning("Fail load condition data file : " + stageName);
            return;
        }

        for (int i = 0; i < conditions.Length; i++)
        {
            dic.Add(conditions[i].blockType, conditions[i]);
        }
    }

    public void SaveConditions()
    {
        DataManager.Inst.stageData.SaveStageClearConditions(stageName, GetConditionArr());
    }
}
