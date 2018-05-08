using System.Collections.Generic;
using UnityEngine;

public interface iClearConditionMaker
{
    ClearCondition AddCondition(int clearIdx);
    void RemoveCondition(int clearIdx);
    IEnumerator<ClearCondition> GetEnumerator();
}

public class ClearConditionMaker : iClearConditionMaker
{
    Dictionary<int, ClearCondition> dic = new Dictionary<int, ClearCondition>();

    string stageName;

    public ClearCondition AddCondition(int clearIdx)
    {
        ClearCondition newCond = new ClearCondition();
        newCond.clearIdx = clearIdx;

        if (dic.ContainsKey(clearIdx))
        {
            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.DISP_MSG, "이미 추가된 조건입니다");
            return null;
        }
                    
        dic.Add(clearIdx, newCond);

        return newCond;
    }

    public void RemoveCondition(int clearIdx)
    {
        if (dic.ContainsKey(clearIdx))
            dic.Remove(clearIdx);
    }


    ClearCondition[] GetConditionArr()
    {
        ClearCondition[] result = new ClearCondition[dic.Count];

        int idx = 0;
        foreach (var item in dic.Values)
        {
            result[idx] = item;
            idx++;
        }
        
        return result;
    }

    public IEnumerator<ClearCondition> GetEnumerator()
    {
        return dic.Values.GetEnumerator();
    }

    public void Init(string stageName)
    {
        Debug.Log("Init Condition");
        this.stageName = stageName;

        ClearCondition[] conditions = DataManager.Inst.stageData.LoadStageClearCondition(stageName);

        if(conditions == null)
        {
            Debug.LogWarning("Fail load condition data file : " + stageName);
            return;
        }

        for (int i = 0; i < conditions.Length; i++)
        {
            dic.Add(conditions[i].clearIdx, conditions[i]);
        }
    }

    public void SaveConditions()
    {
        Debug.Log("SaveCondtion");
        DataManager.Inst.stageData.SaveStageClearConditions(stageName, GetConditionArr());
    }
}
