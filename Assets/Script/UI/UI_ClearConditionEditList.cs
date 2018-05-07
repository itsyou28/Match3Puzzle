using UnityEngine;
using System.Collections.Generic;
using FiniteStateMachine;
using UnityEngine.UI;

public class UI_ClearConditionEditList : MonoBehaviour
{
    [SerializeField]
    GameObject rowOrigin;
    [SerializeField]
    Transform contentPanel;
    [SerializeField]
    Dropdown dropdown;

    int curSelectedClearIdx;

    private void Awake()
    {
        InitDropdown();

        State tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Editor, STATE_ID.Editor_SelectStage);
        tstate.EventEnd += OnEndSelectStage;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Main, STATE_ID.Main_Editor);
        tstate.EventEnd += OnEnd_MainEditor;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Editor, STATE_ID.Editor_FromStage);
        tstate.EventStart += OnStart_Editor_FromStage;
    }

    private void OnStart_Editor_FromStage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        InitList();
    }

    void InitDropdown()
    {
        List<string> options = new List<string>();

        for (int i = 0; i < GlobalVal.BLOCKTYPE_NAME.Length; i++)
        {
            options.Add(GlobalVal.BLOCKTYPE_NAME[i]);
        }

        dropdown.AddOptions(options);
    }

    private void OnEnd_MainEditor(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        ClearRows();
    }

    private void OnEndSelectStage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        InitList();
    }

    public void OnSelectClearIdx(int idx)
    {
        curSelectedClearIdx = idx;
        Debug.Log(curSelectedClearIdx);
    }

    public void ClickAddCondition()
    {
        ClearCondition condition = EditManager.i.iClearCondition.AddCondition(curSelectedClearIdx);

        if(condition != null)
            AddRow(condition);
    }

    void InitList()
    {
        IEnumerator<ClearCondition> itor = EditManager.i.iClearCondition.GetEnumerator();

        itor.Reset();

        while (itor.MoveNext())
        {
            AddRow(itor.Current);
        }
    }

    private void ClearRows()
    {
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            Destroy(contentPanel.GetChild(i).gameObject);
        }
    }

    private GameObject AddRow(ClearCondition condition)
    {
        ClearConditionEditRowData data = new ClearConditionEditRowData();
        data.condition = condition;
        data.callbackDestroy = OnRemoveRow;

        GameObject obj = Instantiate(rowOrigin);
        obj.transform.SetParent(contentPanel, false);
        
        obj.GetComponent<UI_ClearConditionEditRow>().SetRowData(data);

        return obj;
    }

    void OnRemoveRow(ClearConditionEditRowData data)
    {
    }
}
