using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVal
{
    public const string FieldDataPath = "FieldData";
    public const string ClearConditionDataPath = FieldDataPath+"/ClearCondition";
    public const int BLOCKTYPE_MIN = 1;
    public const int BLOCKTYPE_MAX = 15;
    public const int BLOCKTYPE_NORMAL_MIN = 1;
    public const int BLOCKTYPE_NORMAL_MAX = 8;
    public const int BLOCKTYPE_SKILL_GARO = 9;
    public const int BLOCKTYPE_SKILL_SERO = 10;
    public const int BLOCKTYPE_SKILL_SMALLBOMB = 11;
    public const int BLOCKTYPE_SKILL_MIDDLEBOMB = 12;
    public const int BLOCKTYPE_SKILL_BIGBOMB = 13;
    public const int BLOCKTYPE_BOX = 14;
    public const int BLOCKTYPE_MOVEONLY = 15;

    public static string[] BLOCKTYPE_NAME =
    {
        "NONE",
        "1_NORMAL",
        "2_NORMAL",
        "3_NORMAL",
        "4_NORMAL",
        "5_NORMAL",
        "6_NORMAL",
        "7_NORMAL",
        "8_NORMAL",
        "9_SKILL_GARO",
        "10_SKILL_SERO",
        "11_SMALLBOMB",
        "12_MIDDLEBOMB",
        "13_BIGBOMB",
        "14_BOX",
        "15_MOVEONLY",
    };   
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

public enum BlockType
{
    RED,
    BLUE,
    GREEN,
    PURPLE,
    YELLOW,
    BLACK,
    SPECIAL,
}