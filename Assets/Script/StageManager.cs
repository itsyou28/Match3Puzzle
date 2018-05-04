using UnityEngine;
using System.Collections;
using FiniteStateMachine;

public interface iStage
{
    void SwapBlock(BlockField selectField, BlockField targetField);
    void Match();
    void Skill_Line(BlockField targetField, bool isRow);
    void Skill_SmallBomb(BlockField targetField);
    void Skill_MiddleBomb(BlockField targetField);
    void Skill_BigBomb(BlockField targetField);
}

public class DummyStageManager : iStage
{
    public void SwapBlock(BlockField select, BlockField target)
    {
        Debug.LogWarning("Dummy Swap Block");
    }

    public void Match()
    {
        Debug.LogWarning("Dummy CheckMatch");
    }

    public void Skill_Line(BlockField targetField, bool isRow)
    {
    }

    public void Skill_SmallBomb(BlockField targetField)
    {
    }

    public void Skill_MiddleBomb(BlockField targetField)
    {
    }

    public void Skill_BigBomb(BlockField targetField)
    {
    }
}

public class StageManager : MonoBehaviour, iStage
{
    private static StageManager instance = null;
    private static DummyStageManager dummy = null;
    public static iStage i
    {
        get
        {
            if (instance == null)
            {
                if (dummy == null)
                    dummy = new DummyStageManager();

                return dummy;
            }
            return instance;
        }
    }

    BlockFieldManager fieldMng;
    ClearChecker clearChecker;

    string curStageName;

    void Awake()
    {
        instance = this;

        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.SELECT_STAGE, OnSelectStage);

        State tstate;
        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Stage, STATE_ID.Stage_FromEditor);
        tstate.EventStart += OnStart_FromEditor;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Stage, STATE_ID.Stage_ToEditor);
        tstate.EventStart += OnStart_ToEditor;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Stage, STATE_ID.Stage_BackToMain);
        tstate.EventStart += OnStart_BackToMain;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Main, STATE_ID.Main_Stage);
        tstate.EventStart += OnStart_Main_Stage;
        tstate.EventEnd += OnEnd_Main_Stage;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Stage, STATE_ID.Stage_Play);
        tstate.EventStart += OnStart_Stage_Play;
        tstate.EventEnd += OnEnd_Stage_Play;
    }

    private void OnEnd_Stage_Play(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        BlockMng.Inst.IsIngame = false;
    }

    private void OnStart_Stage_Play(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        BlockMng.Inst.IsIngame = true;
    }

    private void OnEnd_Main_Stage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        BlockMng.Inst.AllReady -= OnAllReady;
    }

    private void OnStart_Main_Stage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        BlockMng.Inst.AllReady += OnAllReady;
    }

    private void OnStart_BackToMain(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        CleanUpStage();
    }

    private void OnAllReady()
    {
        Match();
    }

    private void OnStart_ToEditor(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        CleanUpStage();
    }

    private void OnStart_FromEditor(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        InitStage(curStageName);

        //fromEditor->Intro
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
    }

    void OnSelectStage(params object[] args)
    {
        if (args == null || args.Length == 0)
        {
            UDL.LogError("need stagename in args[0]");
            return;
        }

        curStageName = (string)args[0];

        if (FSM_Layer.Inst.GetCurFSM(FSM_LAYER_ID.UserStory).fsmID == FSM_ID.Stage)
        {
            InitStage(curStageName);
        }
    }

    void InitStage(string stageName)
    {
        CleanUpStage();

        clearChecker = new ClearChecker(stageName);

        fieldMng = new BlockFieldManager(stageName);
        fieldMng.DeployBlock();
        fieldMng.ActiveFields(false);
        this.enabled = true;
    }


    private void CleanUpStage()
    {
        StopAllCoroutines();
        this.enabled = false;

        if(clearChecker != null)
        {
            clearChecker.CleanUp();
        }

        if (fieldMng != null)
        {
            fieldMng.DeactiveFields();
            fieldMng.CleanUp();
            fieldMng = null;
        }
    }

    public void SwapBlock(BlockField select, BlockField target)
    {
        fieldMng.SwapBlock(select, target);
    }

    public void Match()
    {
        if (fieldMng.FindMatch())
            fieldMng.ExcuteMatch();
    }

    public void Skill_Line(BlockField field, bool isRow)
    {
        if (isRow)
            StartCoroutine(fieldMng.Skill_LineGaro(field));
        else
            StartCoroutine(fieldMng.Skill_LineSero(field));
    }

    public void Skill_SmallBomb(BlockField field)
    {
        StartCoroutine(fieldMng.Skill_SmallBomb(field));
    }

    public void Skill_MiddleBomb(BlockField field)
    {
        StartCoroutine(fieldMng.Skill_MiddleBomb(field));
    }

    public void Skill_BigBomb(BlockField field)
    {
        StartCoroutine(fieldMng.Skill_BigBomb(field));
    }
}
