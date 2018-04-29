using UnityEngine;
using FiniteStateMachine;

public class UI_StageMenu : MonoBehaviour
{
    public void Resume()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_BACK);
    }

    public void ResetStage()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_1);
    }

    public void SelectStage()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_3);
    }

    public void BackToMain()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_2);
    }
}
