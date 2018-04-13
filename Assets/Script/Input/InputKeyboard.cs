using UnityEngine;
using System.Collections;
using InputKeyProcessor;

public class InputKeyboard : MonoBehaviour
{
    KeyProcessor curProcess;

    private void Awake()
    {
        curProcess = new FSM_Tester();
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

    }
}
