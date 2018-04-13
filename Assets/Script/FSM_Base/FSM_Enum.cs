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
        Editor,
        Stage,
    }

    //Inspector 화면 State 드롭다운메뉴는 선택한 FSM_ID에 따라 목록이 변경된다. 
    //STATE_ID 언더바 앞의 prefix가 선택한 FSM_ID와 일치해야 한다. 
    //[Prefix]_[STATE_ID] // Prefix = FSM_ID

    public enum STATE_ID
    {
        None = 0,
        AnyState,
        HistoryBack,

        Main_Start = 100,
        Main_Loading,
        Main_Entrance,
        Main_Editor,
        Main_Stage,
        Main_ExitConfirm,

        Stage_Start = 1000,
        Stage_Select,
        Stage_Intro,
        Stage_Play,
        Stage_ToEditor,
        Stage_Clear,
        Stage_GameOver,
        Stage_SelectOrReplay,
        Stage_BackToMain,
        Stage_FromEditor,

        Editor_Start = 2000,
        Editor_SelectStage,
        Editor_CreateStage,
        Editor_Idle,
        Editor_EditField,
        Editor_EditBlock,
        Editor_ToStage,
        Editor_SaveStage___noUse,
        Editor_BackToMain,
        Editor_FromStage,
    }


    public enum TRANS_PARAM_ID
    {
        TRIGGER_NONE = 1000,
        TRIGGER_RESET,
        TRIGGER_ESCAPE,
        TRIGGER_BACK,
        TRIGGER_OPTION,
        TRIGGER_SKIP,
        TRIGGER_COMPLETE,
        TRIGGER_CHECK_CONDITION,
        TRIGGER_CHECK_ANY_CONDITION,

        TRIGGER_PREV = 1100,
        TRIGGER_NEXT,
        TRIGGER_SUCCESS,
        TRIGGER_FAIL,
        TRIGGER_OK,
        TRIGGER_CANCEL,
        TRIGGER_YES,
        TRIGGER_NO,

        TRIGGER_UP = 1200,
        TRIGGER_DOWN,
        TRIGGER_LEFT,
        TRIGGER_RIGHT,

        TRIGGER_SELECT_1 = 1300,
        TRIGGER_SELECT_2,
        TRIGGER_SELECT_3,
        TRIGGER_SELECT_4,

        TRIGGER_TOSTAGE = 1400,
        TRIGGER_TOEDITOR,

        INT_NONE = 2000,
        INT_USERSTORY_STATE,
        INT_SELECT_MENU,
        INT_USERSTORY_PRE_STATE,
        INT_FLAG,

        FLOAT_NONE = 3000,

        BOOL_NONE = 4000,
        BOOL_FLAG1,
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