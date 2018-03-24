using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlovalVar
{
    public const string TableDataNamespace = "TableData";
    public const string TableEditDataPath = "TableEditData";
    public const string TableDataPath = "TableData";
}

public enum LogOption
{
    DEFAULT = 0,
    FSM = 1,
    FSM_Layer = 2,
    FSM_Reaction = 3,
    UI_Binder = 4,
}

public enum CooltimeState
{
    Ready,
    Cooling,
    MultiActive,
}

public enum CooltimeMode
{
    OneShot = 1,
    MultiShot = 2
}


public interface iUserData
{
    bool Load();
    bool Save();
}