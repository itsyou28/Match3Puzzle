using UnityEngine;
using System.Collections;
using FiniteStateMachine;

public interface iStage
{
    void SwapBlock(BlockField selectField, BlockField targetField);
    void ChangeStage(string stageName);
}

public class DummyStageManager : iStage
{
    public void SwapBlock(BlockField select, BlockField target)
    {
        Debug.LogWarning("Dummy Swap Block");
    }
    public void ChangeStage(string stageName)
    {
        Debug.LogWarning("Dummy ChangeStage");
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

    void Awake()
    {
        instance = this;

        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.SELECT_STAGE, OnSelectStage);
    }

    void OnSelectStage(params object[] args)
    {
        if (fieldMng != null)
            fieldMng.CleanUp();

        if (FSM_Layer.Inst.GetCurFSM(FSM_LAYER_ID.UserStory).fsmID == FSM_ID.Stage)
        {
            string stageName = (string)args[0];

            ChangeStage(stageName);
        }
    }
    
    public void ChangeStage(string stageName)
    {
        fieldMng = new BlockFieldManager(stageName);
        fieldMng.BlockInitialize();
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

        if (isMatching && fieldMng.IsNotEmpty())
        {
            if (fieldMng.IsNotMoving())
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
}
