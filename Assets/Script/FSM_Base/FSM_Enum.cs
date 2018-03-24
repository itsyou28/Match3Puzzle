using System.Collections;
using System.Collections.Generic;


namespace FiniteStateMachine
{

    public enum FSM_LAYER_ID
    {
        //레이어는 일반 배열을 사용하므로 Enum 변수는 반드시 0부터 순열로 정의되어야 한다. 
        Main = 0,
        UserStory,
        MainUI,
        SubUI,
        PopupUI
    }

    public enum FSM_ID
    {
        NONE = 0,
        Main,
        USMain,
        UIMain,
        PopupUI,
        USTime,
        USBtn,
        USScroll,
    }

    //Inspector 화면 State 드롭다운메뉴는 선택한 FSM_ID에 따라 목록이 변경된다. 
    //STATE_ID 언더바 앞의 prefix가 선택한 FSM_ID와 일치해야 한다. 
    //[Prefix]_[STATE_ID] // Prefix = FSM_ID

    public enum STATE_ID
    {
        None = 0,
        AnyState,
        HistoryBack,

        USMain_Loading = 100,
        USMain_WaitTouch,
        USMain_Mainmenu,
        USMain_TimeSample,
        USMain_BtnSample,
        USMain_ScrollSample,
        USMain_AnimationSample,
        USMain_UserStoryDepth2,
        USMain_UserSotryDepth3,
        USMain_OutroToMainMenu,
        USMain_ReflectionEditor,
        USMain_ExitConfirm,


        USTime_TimeDiaplay = 200,
        USTime_Cooltime,
        USTime_End,
        USTime_Countdown,
        USTime_Stopwatch,


        USBtn_DynamicLayoutBtn = 300,
        USBtn_Toggle,
        USBtn_End,

        USScroll_DynamicList = 400,
        USScroll_HorizonScroll,
        USScroll_End,


        UIMain_Standby = 1000,
        UIMain_ProgressSampleIntro,
        UIMain_ProgressSampleOutro,
        UImain_BtnSampleIntro,
        UIMain_BtnSampleOutro,
        UIMain_ScrollSampleIntro,
        UIMain_ScrollSampleOutro,
        UIMain_AnimationSampleIntro,
        UIMain_AnimationSampleOutro,
        UIMain_UserStoryDepth2_Intro,
        UIMain_UserStoryDepth2_Outro,
        UIMain_UserStoryDepth3_Intro,
        UIMain_UserStoryDepth3_Outro,

        PopupUI_Sleep = 10000,
        PopupUI_Exit,
        PopupUI_Menu,

    }


    public enum TRANS_PARAM_ID
    {
        TRIGGER_NONE = 1000,
        TRIGGER_RESET,
        TRIGGER_ESCAPE,
        TRIGGER_BACKBTN,
        TRIGGER_NEXT,
        TRIGGER_SUCCESS,
        TRIGGER_FAIL,
        TRIGGER_OPTION,
        TRIGGER_CHECK_OK,
        TRIGGER_CHECK_FAIL,
        TRIGGER_YES,
        TRIGGER_NO,
        TRIGGER_SKIP,
        TRIGGER_COMPLETE,
        TRIGGER_CHECK_CONDITION,
        TRIGGER_CHECK_ANY_CONDITION,
        TRIGGER_UP,
        TRIGGER_DOWN,
        TRIGGER_LEFT,
        TRIGGER_RIGHT,


        INT_NONE = 2000,
        INT_USERSTORY_STATE,
        INT_SELECT_MENU,
        INT_USERSTORY_PRE_STATE,
        INT_FLAG,

        FLOAT_NONE = 3000,

        BOOL_NONE = 4000,
    }


    public enum TRANS_ID
    {
        NONE,
        TIME,
        HISTORY_BACK,
        RESET,
        ESCAPE_TO_MAIN,
        BACK_TO_MAIN,
        GAMEOVER,
        ESCAPE,
        SKIP,
    }


}