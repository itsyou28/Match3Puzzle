using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FiniteStateMachine;

public interface iEditManager
{
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
}

public class DummyEditManager : iEditManager
{
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

    BlockFieldManager fieldMng;

    LinkedList<BlockField> selectedList = new LinkedList<BlockField>();
    LinkedList<Collider> markerList = new LinkedList<Collider>();

    string curStageName;

    void Awake()
    {
        instance = this;

        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.SELECT_STAGE, OnSelectStage);
        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.EDITORMODE_POINTER_DOWN, OnPointerDown);
        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.EDITORMODE_POINTER_UP, OnPointerUp);

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
        CleanUp();
    }

    private void OnStart_FromStage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        InitFields(curStageName);

        //fromStage -> Idle
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
    }

    private void OnStart_ToStage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        FieldSave();
        CleanUp();
    }

    private void CleanUp()
    {
        OffSelect();
        fieldMng.CleanUp();
        fieldMng = null;
    }

    private void FieldSave()
    {
        fieldMng.SetFieldRelationInfo();
        fieldMng.ValidateField();
        fieldMng.SaveFields();
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
            InitFields(curStageName);
        }
    }

    void OnPointerUp(params object[] args)
    {
        CircleMenuTurnOn();
    }

    private void CircleMenuTurnOn()
    {
        if (selectedList.Count > 0)
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
    }

    private static void CircleMenuTurnOff()
    {
        if (FSM_Layer.Inst.GetCurStateID(FSM_LAYER_ID.UserStory) != STATE_ID.Editor_Idle)
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_BACK);
    }

    void OnPointerDown(params object[] args)
    {
        CircleMenuTurnOff();
    }

    public void InitFields(string stageName)
    {
        fieldMng = new BlockFieldManager(stageName);
        fieldMng.EditorInitialize();
        BroadcastMessage("ActiveField", true, SendMessageOptions.DontRequireReceiver);
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

        CircleMenuTurnOff();
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
