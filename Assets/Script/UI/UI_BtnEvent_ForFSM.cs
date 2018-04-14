using UnityEngine;
using System.Collections;
using FiniteStateMachine;

public class UI_BtnEvent_ForFSM : MonoBehaviour
{
    public void TriggerNext()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
    }
    public void TriggerBack()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_BACK);
    }
    public void TriggerSelect1()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_1);
    }
    public void TriggerSelect2()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_2);
    }
    public void TriggerSelect3()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_3);
    }
    public void TriggerSelect4()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_4);
    }

}
