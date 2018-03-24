using UnityEngine;
using System.Collections;

public enum N_Bind_Idx
{
    NONE = 0,
    UserGold,
    UserLevel,
    UserExp,
    UserMaxExp,

    DynamicLayoutBtnCount = 100,

    DynamicListScrollCount = 200,

    UTC_Now_HourHand = 300,
    UTC_Now_MinuteHand,
    UTC_Now_SecondHand,

    UTC_Local_Now_HourHand,
    UTC_Local_Now_MinuteHand,
    UTC_Local_Now_SecondHand,

    Set_Countdown,

    Cooltime_Percent = 400,
    Cooltime_RemainTime,
    Cooltime_Mode,
    Cooltime_ShotCost,
}

public enum F_Bind_Idx
{
    NONE = 0,
    User_ExpProgress,

    Set_Local_UTC = 300,

    Cooltime_CurTime = 400,
    Cooltime_MaxTime,
    Cooltime_Percent,
    Cooltime_RemainTime,

}

public enum S_Bind_Idx
{
    NONE = 0,
    Userstory_State,
    UserExp,

    ToggleMode = 100,

    UTC_NowDate = 300,
    UTC_NowTime,
    UTC_Local_NowDate,
    UTC_Local_NowTime,
    
    Stopwatch_ElapseTime = 350,

    CooltimeState = 400,

    ReflectionEditor_CurDataType = 500,
    ReflectionEditor_CurDataID,
    ReflectionEditor_SaveExportMessage,
}
