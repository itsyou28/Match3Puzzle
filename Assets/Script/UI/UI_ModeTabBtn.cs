using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using FiniteStateMachine;

public class UI_ModeTabBtn : MonoBehaviour
{
    [SerializeField]
    Toggle toEditorBtn;
    [SerializeField]
    Toggle toStageBtn;

    private void Awake()
    {
        State tstate;
        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Main, STATE_ID.Main_Editor);
        tstate.EventStart += OnStartEditor;

        tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Main, STATE_ID.Main_Stage);
        tstate.EventStart += OnStartStage;
    }

    private void OnStartStage(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        if (!toStageBtn.isOn)
            toStageBtn.isOn = true;

        if (toEditorBtn.isOn)
            toEditorBtn.isOn = false;
    }

    private void OnStartEditor(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        if (!toEditorBtn.isOn)
            toEditorBtn.isOn = true;

        if (toStageBtn.isOn)
            toStageBtn.isOn = false;
    }

    public void OnChangeToStageToggle(bool bToggle)
    {
        if (bToggle)
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_TOSTAGE);
    }

    public void OnChangeToEditorToggle(bool bToggle)
    {
        if (bToggle)
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_TOEDITOR);            
    }
}
