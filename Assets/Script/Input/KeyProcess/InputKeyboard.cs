using UnityEngine;
using System.Collections;
using InputKeyProcessor;
using FiniteStateMachine;

public class InputKeyboard : MonoBehaviour
{
    KeyProcessor curProcess;

    EditorKey editorMode = new EditorKey();
    StageKey stageMode = new StageKey();

    private void Awake()
    {
        curProcess = new FSM_Tester();

        FSM_Layer.Inst.GetFSM(FSM_LAYER_ID.UserStory, FSM_ID.Main).EventStateChange += OnChangeMainUS;
    }

    private void OnChangeMainUS(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        switch(stateID)
        {
            case STATE_ID.Main_Editor:
                curProcess = editorMode;
                break;
            case STATE_ID.Main_Stage:
                curProcess = stageMode;
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            curProcess.EscapeDown();
        if (Input.GetKeyDown(KeyCode.Return))
            curProcess.ReturnDown();
        if (Input.GetKeyDown(KeyCode.Space))
            curProcess.SpaceDown();
        if (Input.GetKeyDown(KeyCode.Backspace))
            curProcess.BackspaceDown();
        if (Input.GetKeyDown(KeyCode.Tab))
            curProcess.TabDown();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            curProcess.Num1Down();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            curProcess.Num2Down();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            curProcess.Num3Down();
        if (Input.GetKeyDown(KeyCode.Alpha4))
            curProcess.Num4Down();
        if (Input.GetKeyDown(KeyCode.Alpha5))
            curProcess.Num5Down();


        if (Input.GetKeyDown(KeyCode.Keypad1))
            curProcess.Numpad1Down();
        if (Input.GetKeyDown(KeyCode.Keypad2))
            curProcess.Numpad2Down();
        if (Input.GetKeyDown(KeyCode.Keypad3))
            curProcess.Numpad3Down();
        if (Input.GetKeyDown(KeyCode.Keypad4))
            curProcess.Numpad4Down();
        if (Input.GetKeyDown(KeyCode.Keypad5))
            curProcess.Numpad5Down();

        if (Input.GetKeyDown(KeyCode.N))
            curProcess.NDown();
        if (Input.GetKeyDown(KeyCode.K))
            curProcess.KDown();
        if (Input.GetKeyDown(KeyCode.B))
            curProcess.BDown();
        if (Input.GetKeyDown(KeyCode.J))
            curProcess.JDown();
        if (Input.GetKeyDown(KeyCode.L))
            curProcess.LDown();
        if (Input.GetKeyDown(KeyCode.V))
            curProcess.VDown();
    }
}
