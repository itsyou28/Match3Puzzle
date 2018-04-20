using UnityEngine;
using System.Collections;
using FiniteStateMachine;

public interface iStage
{
    void SwapBlock(BlockField selectField, BlockField targetField);
    void CheckMatch();
}

public class DummyStageManager : iStage
{
    public void SwapBlock(BlockField select, BlockField target)
    {
        Debug.LogWarning("Dummy Swap Block");
    }

    public void CheckMatch()
    {
        Debug.LogWarning("Dummy CheckMatch");
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
    }

    private void OnStart_ToEditor(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        StopAllCoroutines();
        this.enabled = false;
        fieldMng.CleanUp();
        fieldMng = null;
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
        fieldMng = new BlockFieldManager(stageName);
        fieldMng.DeployBlock();
        BroadcastMessage("ActiveField", false, SendMessageOptions.DontRequireReceiver);
        this.enabled = true;
    }

    bool isMatching = false;
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (fieldMng.FindMatch())
        //    {
        //        isMatching = true;
        //        fieldMng.ExcuteMatch();
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Return))
        //    fieldMng.Shuffle();

        if (isMatching)
        {
            if (BlockMng.Inst.IsStop)
            {
                isMatching = false;
                StartCoroutine(DelayMatch());
            }
        }
    }

    IEnumerator DelayMatch()
    {
        if (fieldMng.FindMatch())
        {
            //매칭 이펙트, 매치 조건 확인을 위한 시야 제공 등의 이유로 시간차 매칭처리
            yield return new WaitForSeconds(0.1f);
            isMatching = true;
            fieldMng.ExcuteMatch();
        }
        else if(!fieldMng.FindMatchAble())
        {
            yield return new WaitForSeconds(0.1f);
            //셔플 -> 매칭 -> 체크able -> 반복
        }
    }

    public void SwapBlock(BlockField select, BlockField target)
    {
        fieldMng.SwapBlock(select, target,
            () =>
            {
                if (fieldMng.FindMatch())
                {
                    isMatching = true;
                    fieldMng.ExcuteMatch();
                }
            });
    }

    public void CheckMatch()
    {
        if (fieldMng.FindMatch())
            fieldMng.ExcuteMatch();
    }
}
