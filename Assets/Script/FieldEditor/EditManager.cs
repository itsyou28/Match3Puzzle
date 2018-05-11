using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FiniteStateMachine;

public interface iEditManager
{
    void PaintBlock(Collider col);
    void PaintField(Collider col);
    void AddMarker(Collider col);
    void RemoveMarker(Collider col);
    void SetPlayable();
    void SetNonplayable();
    void SetDirection(int dir);
    void SetCreate(bool bValue);
    void OffSelect();
    void SetBlockRandom();
    void SetBlockType(int blockType);
    void Validate();

    iClearConditionMaker iClearCondition { get; }
}

public class DummyEditManager : iEditManager
{
    public DummyEditManager()
    {
        Debug.LogWarning("Create DummyEdit Manager");
    }
    public void PaintField(Collider col) { }
    public void PaintBlock(Collider col) { }
    public void AddMarker(Collider col) { }
    public void RemoveMarker(Collider col) { }
    public void SetPlayable() { }
    public void SetNonplayable() { }
    public void SetDirection(int dir) { }
    public void SetCreate(bool bValue) { }
    public void OffSelect() { }
    public void SetBlockRandom() { }
    public void SetBlockType(int blockType) { }
    public void Validate() { }
    public iClearConditionMaker iClearCondition { get { return new ClearConditionMaker(); } }
}

public class EditManager : MonoBehaviour, iEditManager
{
    private static EditManager instance = null;
    private static DummyEditManager dummy = null;
    public static iEditManager i
    {
        get
        {
            if (instance == null)
            {
                if (dummy == null)
                    dummy = new DummyEditManager();

                return dummy;
            }
            return instance;
        }
    }

    public iClearConditionMaker iClearCondition { get { return conditionMng; } }

    BlockFieldManager fieldMng;
    ClearConditionMaker conditionMng;

    LinkedList<BlockField> selectedList = new LinkedList<BlockField>();
    LinkedList<Collider> markerList = new LinkedList<Collider>();

    Bindable<int> selectedBlockProperty;
    Bindable<int> selectedFieldProperty;

    string curStageName;

    void Awake()
    {
        instance = this;

        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.SELECT_STAGE, OnSelectStage);

        selectedBlockProperty = BindRepo.Inst.GetBindedData(N_Bind_Idx.EDIT_SELECTED_BLOCK_PROPERTY);
        selectedFieldProperty = BindRepo.Inst.GetBindedData(N_Bind_Idx.EDIT_SELECTED_FIELD_PROPERTY);

        State tstate;
        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Editor, STATE_ID.Editor_ToStage);
        tstate.EventStart += OnStart_ToStage;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Editor, STATE_ID.Editor_FromStage);
        tstate.EventStart += OnStart_FromStage;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Editor, STATE_ID.Editor_BackToMain);
        tstate.EventStart += OnStart_BackToMain;
    }

    private void OnStart_BackToMain(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        StageSave();
        CleanUp();
    }

    private void OnStart_FromStage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        InitStage(curStageName);

        //fromStage -> Idle
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
    }

    private void OnStart_ToStage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        StageSave();
        CleanUp();
    }

    private void CleanUp()
    {
        OffSelect();
        if (fieldMng != null)
        {
            fieldMng.CleanUp();
            fieldMng = null;
        }
    }

    private void StageSave()
    {
        if (fieldMng != null)
        {
            fieldMng.SetFieldRelationInfo();
            fieldMng.ValidateField();
            fieldMng.SaveFields();
        }

        if (conditionMng != null)
            conditionMng.SaveConditions();
    }

    void OnSelectStage(params object[] args)
    {
        if (args == null || args.Length == 0)
        {
            UDL.LogError("need stagename in args[0]");
            return;
        }

        curStageName = (string)args[0];

        if (FSM_Layer.Inst.GetCurFSM(FSM_LAYER_ID.UserStory).fsmID == FSM_ID.Editor)
        {
            InitStage(curStageName);
        }
    }
    
    public void InitStage(string stageName)
    {
        conditionMng = new ClearConditionMaker();
        conditionMng.Init(stageName);

        fieldMng = new BlockFieldManager(stageName);
        fieldMng.EditorInitialize();
        BroadcastMessage("ActiveField", true, SendMessageOptions.DontRequireReceiver);
    }

    public void PaintField(Collider col)
    {
        BlockField field = fieldMng.GetBlockField(
            fieldMng.RowLength - (int)col.transform.localPosition.y, (int)col.transform.localPosition.x);

        switch (selectedFieldProperty.Value)
        {
            case 0:
                field.SetPlayable();
                break;
            case 1:
                field.SetNonPlayable();
                break;
            case 2:
                field.SetDirection(0);
                break;
            case 3:
                field.SetDirection(1);
                break;
            case 4:
                field.SetDirection(2);
                break;
            case 5:
                field.SetDirection(3);
                break;
            case 6:
                field.SetCreateField(true);
                break;
            case 7:
                field.SetCreateField(false);
                break;
        }
    }

    public void PaintBlock(Collider col)
    {
        BlockField field = fieldMng.GetBlockField(
            fieldMng.RowLength - (int)col.transform.localPosition.y, (int)col.transform.localPosition.x);

        if (selectedBlockProperty.Value == 0)
            field.SetBlockRandom();
        else
            field.SetBlockType(selectedBlockProperty.Value);

    }

    public void AddMarker(Collider col)
    {
        markerList.AddLast(col);
        selectedList.AddLast(fieldMng.GetBlockField(
            fieldMng.RowLength - (int)col.transform.localPosition.y, (int)col.transform.localPosition.x));

    }
    public void RemoveMarker(Collider col)
    {
        markerList.Remove(col);
        selectedList.Remove(fieldMng.GetBlockField(
            fieldMng.RowLength - (int)col.transform.localPosition.y, (int)col.transform.localPosition.x));
    }
    public void OffSelect()
    {
        foreach (var collider in markerList)
        {
            collider.gameObject.SetActive(false);
        }

        markerList.Clear();
        selectedList.Clear();

        STATE_ID curID = FSM_Layer.Inst.GetCurStateID(FSM_LAYER_ID.UserStory);

        if (curID == STATE_ID.Editor_EditBlock || curID == STATE_ID.Editor_EditField)
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_BACK);
    }

    public void SetPlayable()
    {
        foreach (var field in selectedList)
        {
            field.SetPlayable();
        }
    }
    public void SetNonplayable()
    {
        foreach (var field in selectedList)
        {
            field.SetNonPlayable();
        }
    }

    public void SetDirection(int dir)
    {
        foreach (var field in selectedList)
        {
            field.SetDirection(dir);
        }
    }

    public void SetCreate(bool bValue)
    {
        foreach (var field in selectedList)
        {
            field.SetCreateField(bValue);
        }
    }

    public void SetBlockRandom()
    {
        foreach (var field in selectedList)
        {
            field.SetBlockRandom();
        }
    }

    public void SetBlockType(int blockType)
    {
        foreach (var field in selectedList)
        {
            field.SetBlockType(blockType);
        }
    }

    public void Validate()
    {
        fieldMng.ValidateField();
    }
}
