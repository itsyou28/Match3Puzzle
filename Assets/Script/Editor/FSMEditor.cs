using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using FiniteStateMachine;

/// <summary>
/// 상태추가가 필요할 경우 enum을 추가한다. 
/// 상태이름을 변경할 경우 enum명만 변경한다. 
/// 상태삭제가 필요할 경우 enum명 앞에 'del_' prefix를 지정한다. (지정되어 있는 상수가 바뀌지 않아야 한다. ) 
///     FSM에서는 해당 상태를 제거해도 된다. 
/// 변경된 state enum 상태를 기존 fsm에 반영하고 싶을 경우 enum update 기능을 실행한다. 
/// </summary>
public class FSMEditor : EditorWindow
{
    string fsmFilePath = "Resources/FSMData/";

    [MenuItem("Tools/FSM Editor")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<FSMEditor>();
    }

    private void OnEnable()
    {
        FSM_Utility.Initialize();
    }

    FSM_ID curFSMID = FSM_ID.NONE;
    FSM curFSM = null;
    StatePopupList curStatePopupList = null;

    TransCondWithParam[] buffer_arrTransParam;

    Vector2 vScrollState = Vector2.zero;
    
    TRANS_PARAM_ID intId;
    TRANS_PARAM_ID floatId;
    TRANS_PARAM_ID boolId;

    bool isShowIntParam = false;
    bool isShowFloatParam = false;
    bool isShowBoolParam = false;

    Dictionary<STATE_ID, bool> dic_isShowBuffer = new Dictionary<STATE_ID, bool>();

    private void OnGUI()
    {
        SelectFSM();

        Menu();

        Edit_Parameter();

        Edit_State();
    }

    private void Menu()
    {
        EditorGUILayout.BeginHorizontal();
    
        if (GUILayout.Button("Save"))
            SaveFSM();
        
        if (GUILayout.Button("State Enum Update"))
            EnumUpdate();

        EditorGUILayout.EndHorizontal();
    }

    void SelectFSM()
    {
        GUILayout.Space(30);

        curFSMID = (FSM_ID)EditorGUILayout.EnumPopup(curFSMID);
        curStatePopupList = FSM_Utility.GetPopupList(curFSMID);

        if (GUI.changed)
            LoadFSM();

        GUILayout.Space(30);
    }

    void CreateFSM()
    {
        curFSM = new FSM(curFSMID);

        foreach(int stateid in curStatePopupList.arr_nID)
        {
            if ((STATE_ID)stateid == STATE_ID.HistoryBack)
                continue;
            curFSM.MakeStateFactory((STATE_ID)stateid);
        }
    }

    void SaveFSM()
    {
        FileManager.Inst.FileSave(fsmFilePath, curFSMID.ToString()+".bytes", curFSM);
    }

    void LoadFSM()
    {
        if (FileManager.Inst.CheckFileExists(fsmFilePath, curFSMID.ToString()+".bytes"))
            curFSM = FileManager.Inst.FileLoad(fsmFilePath, curFSMID.ToString() + ".bytes") as FSM;
        else
            CreateFSM();

        dic_isShowBuffer.Clear();

        foreach (STATE_ID id in curFSM.dicStateList.Keys)
            dic_isShowBuffer.Add(id, true);
    }

    void EnumUpdate()
    {
        //현재 FSM과 해당 state enum 목록을 비교해서
        //없는 enum만 새로 추가한다. 

        foreach(STATE_ID id in curStatePopupList.arr_nID)
        {
            if (id == STATE_ID.HistoryBack)
                continue;
            if (!curFSM.dicStateList.ContainsKey(id))
                curFSM.MakeStateFactory(id);
        }
    }

    void Edit_Parameter()
    {
        if (curFSM == null)
            return;

        Edit_Int_Param(ref curFSM.dicIntParam, ref isShowIntParam, ref intId);
        Edit_Int_Param(ref curFSM.dicFloatParam, ref isShowFloatParam, ref floatId);
        Edit_Int_Param(ref curFSM.dicBoolParam, ref isShowBoolParam, ref boolId);

        //flaot 파라메터 키/초기화값
        //bool 파라메터 키/초기화값
    }

    private void Edit_Int_Param<T>(ref Dictionary<TRANS_PARAM_ID, T> dic, ref bool isShow, ref TRANS_PARAM_ID param_id)
    {
        System.TypeCode typecode = System.Type.GetTypeCode(typeof(T));

        EditorGUILayout.BeginHorizontal("box");

        if (GUILayout.Button(isShow ? "-":"+", GUILayout.Width(50)))
            isShow = !isShow;
        
        if(dic != null)
            GUILayout.Label(typecode.ToString() + " Param (" + dic.Count + ")");
        
        EditorGUILayout.EndHorizontal();

        if (!isShow)
            return;

        EditorGUILayout.BeginHorizontal();

        param_id = (TRANS_PARAM_ID)EditorGUILayout.EnumPopup(param_id, GUILayout.Width(250));
        if (GUILayout.Button("추가", GUILayout.Width(50)))
        {
            switch (typecode)
            {
                case System.TypeCode.Int32:
                    curFSM.AddParamInt(param_id);
                    break;
                case System.TypeCode.Single:
                    curFSM.AddParamFloat(param_id);
                    break;
                case System.TypeCode.Boolean:
                    curFSM.AddParamBool(param_id);
                    break;
            }
        }

        EditorGUILayout.EndHorizontal();

        //int 파라메터 키/초기화값
        if (dic != null)
        {
            TRANS_PARAM_ID[] keys = new TRANS_PARAM_ID[dic.Count];
            dic.Keys.CopyTo(keys, 0);

            foreach (TRANS_PARAM_ID key in keys)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(key.ToString(), GUILayout.Width(250));
                switch (typecode)
                {
                    case System.TypeCode.Int32:
                        dic[key] = (T)(object)EditorGUILayout.IntField((int)(object)dic[key], GUILayout.Width(100));
                        break;
                    case System.TypeCode.Single:
                        dic[key] = (T)(object)EditorGUILayout.FloatField((float)(object)dic[key], GUILayout.Width(100));
                        break;
                    case System.TypeCode.Boolean:
                        dic[key] = (T)(object)EditorGUILayout.Toggle((bool)(object)dic[key], GUILayout.Width(100));
                        break;
                }

                if (GUILayout.Button("삭제", GUILayout.Width(50)))
                    dic.Remove(key);

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void Edit_State()
    {
        if (curFSM == null || curFSM.dicStateList == null)
            return;

        GUILayout.Space(30);
        
        vScrollState = GUILayout.BeginScrollView(vScrollState);

        int rowcount = 0;
        STATE_ID removeId = STATE_ID.None;

        foreach (KeyValuePair<STATE_ID, State> pair in curFSM.dicStateList)
        {
            //전이조건 출력 및 수정
            StateTransitionConditionEdit(pair.Value, rowcount, ref removeId);

            GUILayout.Space(10);

            rowcount++;
        }

        if (removeId != STATE_ID.None)
            curFSM.dicStateList.Remove(removeId);

        GUILayout.EndScrollView();

    }

    void StateTransitionConditionEdit(State state, int rowcount, ref STATE_ID removeId )
    {
        //int transMax = EditorGUILayout.IntSlider("전이 개수 : ", 
        //    state.arrTransitionList==null?0:state.arrTransitionList.Length, 
        //    0, 
        //    state.arrTransitionList==null?10:state.arrTransitionList.Length<10?10:state.arrTransitionList.Length);

        GUILayout.BeginHorizontal("box");

        if(dic_isShowBuffer.ContainsKey(state.eID))
        {
            if (GUILayout.Button(dic_isShowBuffer[state.eID] ? "-" : "+", GUILayout.Width(50)))
                dic_isShowBuffer[state.eID] = !dic_isShowBuffer[state.eID];
        }

        GUILayout.Label(state.eID.ToString() + "("+((int)state.eID).ToString()+")", GUILayout.Width(250));

        if (state.eID != STATE_ID.AnyState)
        {
            if (GUILayout.Button("삭제", GUILayout.Width(40)))
                removeId = state.eID;
        }

        GUILayout.Space(30);

        GUILayout.Label("전이 개수 : ", GUILayout.Width(80));


        int transMax = EditorGUILayout.IntField(
            state.arrTransitionList == null ? 0 : state.arrTransitionList.Length, GUILayout.Width(80));

        if (transMax > 0)
            BK_Function.ResizeArr<TransitionCondition>(transMax, ref state.arrTransitionList);
        else
            state.arrTransitionList = null;

        GUILayout.Space(10);

        if (GUILayout.Button("+", GUILayout.Width(50)))
            BK_Function.ResizeArr<TransitionCondition>(transMax + 1, ref state.arrTransitionList);

        if (GUILayout.Button("-", GUILayout.Width(50)))
        {
            if (transMax > 0)
                BK_Function.ResizeArr<TransitionCondition>(transMax - 1, ref state.arrTransitionList);
        }

        state.NoHistory = GUILayout.Toggle(state.NoHistory, "NoHistory", GUILayout.Width(100));

        GUILayout.Label("", GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        if (dic_isShowBuffer.ContainsKey(state.eID) && !dic_isShowBuffer[state.eID])
            return;

        if (state.arrTransitionList != null)
        {
            for (int i = 0; i < state.arrTransitionList.Length; i++)
            {
                if (state.arrTransitionList[i] == null)
                    state.arrTransitionList[i] = new TransitionCondition(0, 0, 0);

                int TransCondParam_Max = TransitionConditionEdit(state, i);

                if (TransCondParam_Max == -1)
                    continue;

                TransCondParamEdit(state, i, TransCondParam_Max);

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                GUILayout.Space(5);
            }
        }
    }

    private int TransitionConditionEdit(State state, int countInList)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("X", GUILayout.Width(30)) && state.arrTransitionList.Length > 0)
        {
            BK_Function.RemoveAtArr<TransitionCondition>(countInList, ref state.arrTransitionList);
            return -1;
        }

        TRANS_ID transid = state.arrTransitionList[countInList].eTransID;

        if (transid != 0)
            GUI.color = Color.red;

        GUILayout.Label("TransID : ", GUILayout.Width(50));
        EditorGUI.BeginChangeCheck();
        transid = (TRANS_ID)EditorGUILayout.IntField((int)transid, GUILayout.Width(30));
        if (EditorGUI.EndChangeCheck())
            state.arrTransitionList[countInList].SetTransID(transid);

        GUI.color = Color.white;

        GUILayout.Label("다음 상태 : ", GUILayout.Width(60));

        STATE_ID nextid = (STATE_ID)EditorGUILayout.IntPopup((int)state.arrTransitionList[countInList].nextStateID,
            curStatePopupList.arr_sName, curStatePopupList.arr_nID, GUILayout.Width(200));
        state.arrTransitionList[countInList].SetNextStateID(nextid);

        float transtime = EditorGUILayout.FloatField("전이 시간 설정 : ",
            (state.arrTransitionList[countInList].transitionWithTime == null) ? 0 : state.arrTransitionList[countInList].transitionWithTime.m_fConditionTimeValue);

        if (transtime != 0)
            state.arrTransitionList[countInList].SetTransTime(transtime, state);
        else
            state.arrTransitionList[countInList].RemoveTransTime();


        if (state.arrTransitionList[countInList].arrTransParam == null)
            state.arrTransitionList[countInList].arrTransParam = new ArrayList();

        int TransCondParam_Max = EditorGUILayout.IntSlider(
            "전이 조건 개수 : ", state.arrTransitionList[countInList].arrTransParam.Count, 0, 5);

        GUILayout.EndHorizontal();

        return TransCondParam_Max;
    }

    private void TransCondParamEdit(State state, int countInList, int TransCondParam_Max)
    {
        if (TransCondParam_Max != state.arrTransitionList[countInList].arrTransParam.Count)
        {
            buffer_arrTransParam = new TransCondWithParam[state.arrTransitionList[countInList].arrTransParam.Count];
            int k = 0;
            foreach (TransCondWithParam t in state.arrTransitionList[countInList].arrTransParam)
            {
                buffer_arrTransParam[k] = t;
                k++;
            }

            state.arrTransitionList[countInList].arrTransParam.Clear();

            for (int j = 0; j < TransCondParam_Max; j++)
            {
                if (buffer_arrTransParam.Length > j)
                {
                    if (buffer_arrTransParam[j] != null)
                        state.arrTransitionList[countInList].arrTransParam.Add(buffer_arrTransParam[j]);
                    else
                        state.arrTransitionList[countInList].arrTransParam.Add(
                            new TransCondWithParam(TransitionType.TRIGGER, 0, TRANS_PARAM_ID.TRIGGER_NONE));
                }
                else
                    state.arrTransitionList[countInList].arrTransParam.Add(
                        new TransCondWithParam(TransitionType.TRIGGER, 0, TRANS_PARAM_ID.TRIGGER_NONE));
            }
        }

        if (state.arrTransitionList[countInList].arrTransParam.Count > 0)
        {
            foreach (TransCondWithParam cond in state.arrTransitionList[countInList].arrTransParam)
            {
                GUILayout.BeginHorizontal();
                TransitionType e = (TransitionType)EditorGUILayout.EnumPopup(cond.m_eTransType);
                if (e != cond.m_eTransType)
                    cond.SetTransitionType(e);

                TRANS_PARAM_ID eParam = (TRANS_PARAM_ID)EditorGUILayout.EnumPopup((TRANS_PARAM_ID)cond.m_uiParamID);
                if (eParam != (TRANS_PARAM_ID)cond.m_uiParamID)
                    cond.SetParamID(eParam);

                switch (e)
                {
                    case TransitionType.BOOL:
                        bool tb = EditorGUILayout.Toggle(cond.M_bConditionValue);
                        if (tb != cond.M_bConditionValue)
                            cond.SetConditionValue(tb);
                        break;
                    case TransitionType.FLOAT:
                        float tf = EditorGUILayout.FloatField(cond.M_fConditionValue);
                        if (tf != cond.M_fConditionValue)
                            cond.SetConditionValue(tf);
                        break;
                    case TransitionType.INT:
                        int ti = EditorGUILayout.IntField(cond.M_iConditionValue);
                        if (ti != cond.M_iConditionValue)
                            cond.SetConditionValue(ti);
                        break;
                }

                TransitionComparisonOperator eOp = TransitionComparisonOperator.EQUALS;
                if (e != TransitionType.TRIGGER)
                    eOp = (TransitionComparisonOperator)EditorGUILayout.EnumPopup(cond.m_eCompOperator);

                if (eOp != cond.m_eCompOperator)
                    cond.SetCompOperator(eOp);

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
        }
    }

}