using UnityEngine;
using System.Collections;
using FiniteStateMachine;

public class UI_ClearCondition : MonoBehaviour
{
    [SerializeField]
    GameObject rowOrigin;

    private void Awake()
    {
        State tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Stage, STATE_ID.Stage_Intro);
        tstate.EventStart += OnStart_US_StageIntro;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Main, STATE_ID.Main_Stage);
        tstate.EventEnd += OnEnd_US_MainStage;
    }

    private void OnEnd_US_MainStage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        CleanUpRow();
    }

    private void OnStart_US_StageIntro(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        DispCondition();
    }

    void DispCondition()
    {
        if (!StageManager.i.ClearChecker.IsEnable)
        {
            Debug.LogError("Can't use Clearcheker");
            return;
        }

        CleanUpRow();

        foreach (var item in StageManager.i.ClearChecker.GetCondition())
        {
            AddRow(item);
        }
    }

    private void CleanUpRow()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void AddRow(ClearCondition condition)
    {
        GameObject obj = Instantiate(rowOrigin);
        obj.transform.SetParent(transform, false);

        obj.GetComponent<UI_ClearConditionRow>().SetCondition(condition);

    }
}
