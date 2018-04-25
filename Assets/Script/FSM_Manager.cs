using UnityEngine;
using System.Collections;
using FiniteStateMachine;

/// <summary>
/// Event호출 순서
/// State.EventEnd
/// State.EventStart
/// FSM.EventStateChange
/// </summary>
public class FSM_Manager : MonoBehaviour
{

    FSM RegistFSM(FSM_LAYER_ID layer, FSM_ID id)
    {
        FSM tFSM = FileManager.Inst.ResourceLoad("FSMData/" + id.ToString()) as FSM;

        if (tFSM == null)
        {
            Debug.LogWarning("No FSM Data " + id.ToString());
            return null;
        }

        tFSM.InitNonSerializedField();

        FSM_Layer.Inst.AddFSM(layer, tFSM, id);

        return tFSM;
    }

    FSM editor;
    FSM stage;

    private void Awake()
    {

        editor = RegistFSM(FSM_LAYER_ID.UserStory, FSM_ID.Editor);
        editor.SetTrigger(TRANS_PARAM_ID.TRIGGER_RESET);
        stage = RegistFSM(FSM_LAYER_ID.UserStory, FSM_ID.Stage);
        stage.SetTrigger(TRANS_PARAM_ID.TRIGGER_RESET);

        editor.GetState(STATE_ID.Editor_BackToMain).EventResume += OnResume_US_BackToMain;
        stage.GetState(STATE_ID.Stage_BackToMain).EventResume += OnResume_US_BackToMain;

        RegistFSM(FSM_LAYER_ID.UserStory, FSM_ID.Main);

        FSM_Layer.Inst.RegisterEventChangeLayerState(FSM_LAYER_ID.UserStory, OnChangeUserStory);
    }

    private void OnResume_US_BackToMain(STATE_ID stateID)
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
    }

    private void Start()
    {
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_RESET);
    }

    

    void OnChangeUserStory(TRANS_ID transId, STATE_ID stateId, STATE_ID preStateId)
    {
        Debug.Log("UserStory current State : " + stateId);

        stage.SetInt_NoCondChk(TRANS_PARAM_ID.INT_USERSTORY_STATE, (int)stateId);
        editor.SetInt_NoCondChk(TRANS_PARAM_ID.INT_USERSTORY_STATE, (int)stateId);

        switch (stateId)
        {
            case STATE_ID.Main_Editor:
                FSM_Layer.Inst.ChangeFSM(FSM_LAYER_ID.UserStory, FSM_ID.Editor);
                break;
            case STATE_ID.Main_Stage:
                FSM_Layer.Inst.ChangeFSM(FSM_LAYER_ID.UserStory, FSM_ID.Stage);
                break;
            case STATE_ID.Editor_ToStage:
                FSM_Layer.Inst.ChangeFSM(FSM_LAYER_ID.UserStory, FSM_ID.Main);
                FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);//Main_Editor->Main_Stage
                FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_TOSTAGE);//->Stage_FromEditor
                break;
            case STATE_ID.Stage_ToEditor:
                FSM_Layer.Inst.ChangeFSM(FSM_LAYER_ID.UserStory, FSM_ID.Main);
                FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);//Main_Stage->Main_Editor
                FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_TOEDITOR);//->Editor_FromStage
                break;
            case STATE_ID.Editor_BackToMain:
            case STATE_ID.Stage_BackToMain:
                FSM_Layer.Inst.ChangeFSM(FSM_LAYER_ID.UserStory, FSM_ID.Main);
                FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_BACK);//->Main_Entrance
                break;
        }
    }

    private void Update()
    {
        FSM_Layer.Inst.Update();
    }
}
