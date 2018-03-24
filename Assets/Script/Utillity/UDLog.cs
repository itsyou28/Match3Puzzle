using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UDLogOption
{
    public bool twoCondition;
    public bool logToConsole;
    public bool logToDB;
    public bool logToDBLog;

    public bool[] arrOption;
}

public class UDLog : MonoBehaviour
{
    private static UDLog instance = null;
    public static UDLog Inst { get { return instance; } }


    public string logdbURL = "http://localhost";

    public bool twoCondition = false;
    public bool logToConsole = false;
    public bool logToDB = false;
    public bool logToDBLog = false;

    public int level = 0;

    public bool bDefault = false;
    public bool bFSM = false;
    public bool bFSMLayer = false;
    public bool bUIReaction = false;
    public bool bUIBind = false;
    
    UDLogOption editorOption;

    private void Awake()
    {
        instance = this;

        UDL.logLevel = level;

        UDL.tagname = "START";

        UDL.buildVersion = 1;
        UDL.uid = SystemInfo.deviceUniqueIdentifier;

        UDL.uri = new System.Uri(logdbURL);

        UDL.deviceModel = SystemInfo.deviceModel;
        UDL.deviceInfo = SystemInfo.operatingSystem;
        
        editorOption.twoCondition = twoCondition;
        editorOption.logToConsole = logToConsole;
        editorOption.logToDB = logToDB;
        editorOption.logToDBLog = logToDBLog;
        
        editorOption.arrOption = new bool[5];

        editorOption.arrOption[(int)LogOption.DEFAULT]      = bDefault;
        editorOption.arrOption[(int)LogOption.FSM]          = bFSM;
        editorOption.arrOption[(int)LogOption.FSM_Layer]    = bFSMLayer;
        editorOption.arrOption[(int)LogOption.FSM_Reaction] = bUIReaction;
        editorOption.arrOption[(int)LogOption.UI_Binder]    = bUIBind;     
                
        SetOption(editorOption);
    }
    

    private void SetOption(UDLogOption _option)
    {
        UDL.twoCondition    = _option.twoCondition;
        UDL.logToConsole    = _option.logToConsole;
        UDL.logToDb         = _option.logToDB;
        UDL.logToDbLog      = _option.logToDBLog;

        UDL.arr_Option = _option.arrOption;
    }
}
