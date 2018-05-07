using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClearConditionEditRowData
{
    public ClearCondition condition;
    public System.Action<ClearConditionEditRowData> callbackDestroy;
}

public class UI_ClearConditionEditRow : MonoBehaviour
{
    [SerializeField]
    Text conditionName;
    [SerializeField]
    InputField reqCntField;

    ClearConditionEditRowData rowData;
    
    public void SetRowData(ClearConditionEditRowData data)
    {
        this.rowData = data;
        conditionName.text = GlobalVal.BLOCKTYPE_NAME[data.condition.clearIdx];
        reqCntField.text = data.condition.requireCnt.ToString();
    }

    public void OnSelectCondition(int idx)
    {
        rowData.condition.clearIdx = idx;
    }

    public void OnChangeReqCnt(string cnt)
    {
        rowData.condition.requireCnt = int.Parse(cnt);
    }

    public void ClickRemove()
    {
        EditManager.i.iClearCondition.RemoveCondition(rowData.condition.clearIdx);
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if (rowData.callbackDestroy != null)
            rowData.callbackDestroy(rowData);
    }
}
