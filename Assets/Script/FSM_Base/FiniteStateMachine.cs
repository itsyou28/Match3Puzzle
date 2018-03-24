using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Event호출 순서
/// State.EventEnd
/// State.EventStart
/// FSM.EventStateChange
/// </summary>

namespace FiniteStateMachine
{
    public delegate void deleStateTransEvent(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID);
    public delegate void deleStatePauseResume(STATE_ID stateID);
    public delegate void deleLayerPauseResume(FSM_LAYER_ID layerNum);


    public enum TransitionType
    {
        INT, FLOAT, BOOL, TRIGGER
    }
    public enum TransitionComparisonOperator
    {
        EQUALS, NOTEQUAL, GREATER, LESS, OVER, UNDER
    }

    [System.Serializable]
    public class FSM
    {
        public const int logOption = (int)LogOption.FSM;
        
        public const int logLv = 8;
        public const int warningLoglv = 7;
        public const int errorLoglv = 6;

        public event deleStatePauseResume EventPause;
        public event deleStatePauseResume EventResume;
        public event deleStateTransEvent EventStateChange;

        public FSM_ID fsmID { get; private set; }

        public void SetFSMID(FSM_ID id)
        {
            fsmID = id;
        }

        State anyState;

        public Dictionary<STATE_ID, State> dicStateList = new Dictionary<STATE_ID, State>();

        public Dictionary<TRANS_PARAM_ID, int> dicIntParam = new Dictionary<TRANS_PARAM_ID, int>();
        public Dictionary<TRANS_PARAM_ID, float> dicFloatParam = new Dictionary<TRANS_PARAM_ID, float>();
        public Dictionary<TRANS_PARAM_ID, bool> dicBoolParam = new Dictionary<TRANS_PARAM_ID, bool>();
        public TRANS_PARAM_ID triggerID;


        Stack<STATE_ID> history = new Stack<STATE_ID>();
        State curState;

        bool IsActive = false;
        int calldepth = 0;

         int nParamBuffer;
         float fParamBuffer;
         bool bParamBuffer;
         State tStateBuffer;

        public FSM(FSM_ID id)
        {
            fsmID = id;

            anyState = new State(STATE_ID.AnyState);

            curState = anyState;

            dicStateList.Add(STATE_ID.AnyState, anyState);
        }

        /// <summary>
        /// 파일로드 방식으로 FSM을 초기화 했을 경우 TransCondWithTime 클래스에 Owner 이벤트를 수동으로 동록시켜줘야 한다. 
        /// </summary>
        public void InitNonSerializedField()
        {
            history = new Stack<STATE_ID>();
            curState = anyState;

            foreach (State t in dicStateList.Values)
                t.InitNonSerializedField();
        }


        public State MakeStateFactory(STATE_ID stateID, params TransitionCondition[] trans)
        {
            State tState = new State(stateID);


            if (trans != null)
            {
                if (trans.Length > 0)
                {
                    for (int i = 0; i < trans.Length; i++)
                    {
                        tState.AddTransition(trans[i]);
                        trans[i].SetOwnerForTimeCond(tState);
                    }
                }
            }

            AddState(tState);

            return tState;
        }

        public void AddState(State newState)
        {
            if (newState.eID == STATE_ID.AnyState)
                UDL.LogWarning("AnyState(Code_AnyState) 는 이미 생성되어 있습니다. ", logOption, FSM.warningLoglv);

            if (dicStateList.ContainsKey(newState.eID))
            {
                UDL.LogError("ID가 겹침 " + newState.eID.ToString());
                return;
            }

            dicStateList.Add(newState.eID, newState);
        }

        public State GetState(STATE_ID stateID)
        {
            UDL.Log("GetState : " + stateID.ToString() + " / dicStateList.Count : " + dicStateList.Count.ToString(), logOption, FSM.logLv);

            if (dicStateList.TryGetValue(stateID, out tStateBuffer))
                return tStateBuffer;

            UDL.LogError("등록되지 않은 상태를 호출했음 " + stateID, logOption, FSM.errorLoglv);
            return null;
        }

        public STATE_ID GetCurStateID()
        {
            return curState.eID;
        }

        public State GetCurState()
        {
            return curState;
        }

        public State GetAnyState()
        {
            return anyState;
        }

        public void AddParamInt(TRANS_PARAM_ID param_id, int value = 0)
        {
            if (!dicIntParam.ContainsKey(param_id))
                dicIntParam.Add(param_id, value);
        }

        public void AddParamFloat(TRANS_PARAM_ID param_id, float value = 0.0f)
        {
            if (!dicFloatParam.ContainsKey(param_id))
                dicFloatParam.Add(param_id, value);
        }

        public void AddParamBool(TRANS_PARAM_ID param_id, bool value = false)
        {
            if (!dicBoolParam.ContainsKey(param_id))
                dicBoolParam.Add(param_id, value);
        }

        public int GetParamInt(TRANS_PARAM_ID param_id)
        {
            if (!dicIntParam.TryGetValue(param_id, out nParamBuffer))
            {
                UDL.LogError(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.errorLoglv);
                return 0;
            }

            return nParamBuffer;
        }
        public float GetParamFloat(TRANS_PARAM_ID param_id)
        {
            if (!dicFloatParam.TryGetValue(param_id, out fParamBuffer))
            {
                UDL.LogError(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.errorLoglv);
                return 0;
            }

            return fParamBuffer;
        }
        public bool GetParamBool(TRANS_PARAM_ID param_id)
        {
            if (!dicBoolParam.TryGetValue(param_id, out bParamBuffer))
            {
                UDL.LogError(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.errorLoglv);
                return false;
            }

            return bParamBuffer;
        }


        public void SetInt(TRANS_PARAM_ID param_id, int value)
        {
            if (!dicIntParam.TryGetValue(param_id, out nParamBuffer))
            {
                UDL.LogWarning(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.warningLoglv);
                return;
            }

            dicIntParam[param_id] = value;

            RequestTransitionChk();
        }

        public void SetFloat(TRANS_PARAM_ID param_id, float value)
        {
            if (!dicFloatParam.TryGetValue(param_id, out fParamBuffer))
            {
                UDL.LogWarning(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.warningLoglv);
                return;
            }

            dicFloatParam[param_id] = value;

            RequestTransitionChk();
        }

        public void SetBool(TRANS_PARAM_ID param_id, bool value)
        {
            if (!dicBoolParam.TryGetValue(param_id, out bParamBuffer))
            {
                UDL.LogWarning(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.warningLoglv);
                return;
            }

            dicBoolParam[param_id] = value;

            RequestTransitionChk();
        }

        public void SetInt_NoCondChk(TRANS_PARAM_ID param_id, int value)
        {
            if (!dicIntParam.TryGetValue(param_id, out nParamBuffer))
                UDL.LogWarning(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.warningLoglv);
            else
                dicIntParam[param_id] = value;
        }

        public void SetFloat_NoCondChk(TRANS_PARAM_ID param_id, float value)
        {
            if (!dicFloatParam.TryGetValue(param_id, out fParamBuffer))
                UDL.LogWarning(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.warningLoglv);
            else
                dicFloatParam[param_id] = value;
        }

        public void SetBool_NoCondChk(TRANS_PARAM_ID param_id, bool value)
        {
            if (!dicBoolParam.TryGetValue(param_id, out bParamBuffer))
                UDL.LogWarning(((FSM_ID)fsmID).ToString() + " not have given Transition parameter id. ", logOption, FSM.warningLoglv);
            else
                dicBoolParam[param_id] = value;
        }

        public void SetTrigger(TRANS_PARAM_ID param_id)
        {
            triggerID = param_id;

            RequestTransitionChk();

            triggerID = TRANS_PARAM_ID.TRIGGER_NONE;
        }

        void RequestTransitionChk()
        {
            if (!CurStateTransitionChk())
                AnyStateTransitionChk();
        }

        bool CurStateTransitionChk()
        {
            UDL.Log(fsmID + " CurStateTransitionChk", logOption, FSM.logLv);

            if (curState.arrTransitionList == null || curState.arrTransitionList.Length == 0)
                UDL.LogWarning(fsmID + " No Transition List in curState", logOption, FSM.warningLoglv);

            if (curState.arrTransitionList != null)
            {
                for (int idx = 0; idx < curState.arrTransitionList.Length; idx++)
                {
                    if (curState.arrTransitionList[idx].ConditionCheck(this))
                    {
                        UDL.Log(fsmID + " ConditionCheck Pass : " + curState.arrTransitionList.Length.ToString() + "/ " + curState.eID, logOption, FSM.logLv);

                        TransitionStart(curState.arrTransitionList[idx].eTransID, curState.arrTransitionList[idx].nextStateID);
                        return true;
                    }
                }
            }

            return false;
        }

        void AnyStateTransitionChk()
        {
            UDL.Log(fsmID + " AnyStateTransitionChk", logOption, FSM.logLv);

            if (anyState.arrTransitionList == null || anyState.arrTransitionList.Length == 0)
                UDL.LogWarning(fsmID + " No Transition List in AnyState", logOption, FSM.errorLoglv);
            else
            {
                for (int idx = 0; idx < anyState.arrTransitionList.Length; idx++)
                {
                    if (anyState.arrTransitionList[idx].ConditionCheck(this))
                    {
                        TransitionStart(anyState.arrTransitionList[idx].eTransID, anyState.arrTransitionList[idx].nextStateID);
                        break;
                    }
                }
            }
        }

        void TransitionStart(TRANS_ID transParamID, STATE_ID nextStateID)
        {
            triggerID = TRANS_PARAM_ID.TRIGGER_NONE;

            if (!IsActive)
            {
                UDL.Log(fsmID + " Refuse TransitionStart", logOption, FSM.logLv);
                return;
            }

            if (nextStateID == STATE_ID.HistoryBack)
            {
                HistoryBack();
                return;
            }

            if (!dicStateList.TryGetValue(nextStateID, out tStateBuffer))
            {
                UDL.LogError(nextStateID.ToString() + " 등록된 씬이 아님!", logOption, FSM.errorLoglv);
                return;
            }

            calldepth++;

            if (calldepth > 1)
                UDL.LogWarning(fsmID + " FSM Call Depth is : " + calldepth
                    + " // 재귀호출구조가 되면서 EvnetStateChange callback이 현재 상태만을 매개변수로 역순으로 반복호출됩니다. ", logOption, FSM.warningLoglv);

            UDL.Log(fsmID + " Transition Start// " + curState.eID + " -> "
                + dicStateList[nextStateID].eID + " // " + transParamID, logOption, FSM.logLv);

            STATE_ID preStateID = curState.eID;

            curState.End(transParamID, nextStateID);

            if(!curState.NoHistory)
                history.Push(curState.eID);

            curState = dicStateList[nextStateID];

            curState.Start(transParamID, preStateID);

            if (EventStateChange != null)
                EventStateChange(transParamID, curState.eID, preStateID);

            calldepth--;
        }

        public void TimeCheck()
        {
            if (curState.arrTransitionList == null || curState.arrTransitionList.Length == 0)
                return;

            for (int idx = 0; idx < curState.arrTransitionList.Length; idx++)
            {
                if (curState.arrTransitionList[idx].TimeConditionCheck(this))
                {
                    TransitionStart(curState.arrTransitionList[idx].eTransID, curState.arrTransitionList[idx].nextStateID);
                    return;
                }
            }
        }

        public void HistoryBack()
        {
            STATE_ID preStateID = curState.eID;
            STATE_ID nextStateID = history.Pop();

            UDL.Log(fsmID + " Transition Start// " + curState.eID + " -> " + nextStateID, logOption, FSM.logLv);

            curState.End(TRANS_ID.HISTORY_BACK, nextStateID);

            curState = dicStateList[nextStateID];

            curState.Start(TRANS_ID.HISTORY_BACK, preStateID);

            if (EventStateChange != null)
                EventStateChange(TRANS_ID.HISTORY_BACK, curState.eID, preStateID);
        }

        public void Pause()
        {
            if (!IsActive)
                return;
            IsActive = false;
            curState.Pause();

            if (EventPause != null)
                EventPause(curState.eID);
        }

        public void Resume()
        {
            if (IsActive)
                return;

            IsActive = true;
            curState.Resume();

            if (EventResume != null)
                EventResume(curState.eID);
        }
    }


    [System.Serializable]
    public class State
    {
        public STATE_ID eID { get; private set; }

        public bool NoHistory = false;

        [System.NonSerialized]
        public string name = null;

        public TransitionCondition[] arrTransitionList = null;

        public event deleStateTransEvent EventStart;
        public event deleStateTransEvent EventStart_Before;
        public event deleStateTransEvent EventStart_After1;
        public event deleStateTransEvent EventStart_After2;
        public event deleStateTransEvent EventEnd;
        public event deleStateTransEvent EventEnd_Before;
        public event deleStateTransEvent EventEnd_After;
        public event deleStatePauseResume EventPause;
        public event deleStatePauseResume EventResume;

        public State(STATE_ID id)
        {
            eID = id;
        }

        public void InitNonSerializedField()
        {
            //에디터에서 생성한 TimeCondtion 에서 등록한 이벤트가 시리얼라이즈 되서 파일로 저장되어있다. 
            //현재 저장되어 있는 fsm 파일 데이터로 인해 중복호출이 발생하기 때문에 이 프로젝트(CnH)에서는 초기화 한다. 
            EventStart = null;
            EventEnd = null;
            EventPause = null;
            EventResume = null;

            if (arrTransitionList != null)
            {
                foreach (TransitionCondition t in arrTransitionList)
                    t.SetOwnerForTimeCond(this);
            }
        }

        public void AddTransition(TransitionCondition value)
        {
            TransitionCondition[] tempArr;

            if (arrTransitionList == null)
            {
                tempArr = new TransitionCondition[1];
                tempArr[0] = value;
            }
            else
            {
                for (int idx = 0; idx < arrTransitionList.Length; idx++)
                {
                    if (value.eTransID != 0 && arrTransitionList[idx].eTransID == value.eTransID)
                        UDL.LogWarning("동일한 전이 ID를 추가합니다", FSM.logOption, FSM.warningLoglv);
                }

                tempArr = new TransitionCondition[arrTransitionList.Length + 1];

                for (int i = 0; i < arrTransitionList.Length; i++)
                    tempArr[i] = arrTransitionList[i];

                tempArr[arrTransitionList.Length] = value;
            }

            arrTransitionList = tempArr;

            value.SetOwnerForTimeCond(this);
        }

        public void ResetTime()
        {
            foreach (TransitionCondition t in arrTransitionList)
            {
                if (t.transitionWithTime != null)
                    t.transitionWithTime.ResetStartTime();
            }
        }

        public void Start(TRANS_ID transID, STATE_ID preStateID)
        {
            if (EventStart_Before != null)
                EventStart_Before(transID, eID, preStateID);
            if (EventStart != null)
                EventStart(transID, eID, preStateID);
            if (EventStart_After1 != null)
                EventStart_After1(transID, eID, preStateID);
            if (EventStart_After2 != null)
                EventStart_After2(transID, eID, preStateID);
        }

        public void End(TRANS_ID transID, STATE_ID nextStateID)
        {
            if (EventEnd_Before != null)
                EventEnd_Before(transID, nextStateID, eID);
            if (EventEnd != null)
                EventEnd(transID, nextStateID, eID);
            if (EventEnd_After != null)
                EventEnd_After(transID, nextStateID, eID);
        }

        public void Pause()
        {
            if (EventPause != null)
                EventPause(eID);
        }

        public void Resume()
        {
            if (EventResume != null)
                EventResume(eID);
        }
    }

    [System.Serializable]
    public class TransitionCondition
    {
        public TRANS_ID eTransID { get; private set; }
        public ArrayList arrTransParam = new ArrayList();
        public TransCondWithTime transitionWithTime = null;

        public STATE_ID nextStateID { get; private set; }

        bool bCheckCondResult = false;

        [System.Obsolete]
        [System.NonSerialized]
        State ownerState;
        /// <summary>
        /// 특정 상태로 넘어가기 위한 조건을 1개 이상 설정할 수 있다. 
        /// 가지고 있는 모든 조건을 만족했을 때만 다음 상태로 전이한다. 
        /// </summary>
        /// <param name="uiID">특정코드번호를 정의하고 입력해두면 상태가 전이 됐을 때 어떤 전이조건으로 전이됐는지 체크할 때 사용할 수 있다. 사용할 일이 없다면 0으로 입력</param>
        /// <param name="transTime">초단위의 시간을 입력. 상태가 시작되고 입력된 시간이 지나면 조건이 만족된다. 시간조건을 사용하지 않는다면 0으로 입력</param>
        public TransitionCondition(STATE_ID _nextStateID, TRANS_ID uiID, float transTime, params TransCondWithParam[] _arrTransParam)
        {
            eTransID = uiID;
            nextStateID = _nextStateID;

            if (transTime != 0)
            {
                transitionWithTime = new TransCondWithTime(transTime);
            }

            if (_arrTransParam != null)
            {
                if (_arrTransParam.Length > 0)
                {
                    for (int i = 0; i < _arrTransParam.Length; i++)
                    {
                        arrTransParam.Add(_arrTransParam[i]);
                    }
                }
            }
        }

        public void SetTransID(TRANS_ID id)
        {
            eTransID = id;

            UDL.LogWarning("Set trans ID : " + eTransID, FSM.logOption, FSM.warningLoglv);
        }

        public void SetOwnerForTimeCond(State _ownerState)
        {
            if (transitionWithTime != null)
                transitionWithTime.SetOwnerState(_ownerState);
        }

        public bool ConditionCheck(FSM pFSM)
        {
            if (transitionWithTime == null)
            {
                if (arrTransParam.Count == 0)
                {
                    UDL.LogWarning("전이 조건이 없습니다. // FSM ID : " + pFSM.fsmID.ToString() + " / " + "CurrentState : "
                        + pFSM.GetCurState().eID + " / OwnerState : "
                        + "/ TransID : " + eTransID.ToString(), FSM.logOption, FSM.warningLoglv);

                    return false;
                }
            }

            bCheckCondResult = true;

            // &연산 true&true=true / true&false=false / false&false = false

            if (transitionWithTime != null)
            {
                bCheckCondResult &= transitionWithTime.TimeConditionChk();
            }

            foreach (TransCondWithParam t in arrTransParam)
            {
                bCheckCondResult &= t.ConditionCheck(pFSM);
            }


            return bCheckCondResult;
        }

        public bool TimeConditionCheck(FSM pFSM)
        {
            if (transitionWithTime != null)
            {
                if (transitionWithTime.TimeConditionChk())
                    return ConditionCheck(pFSM);
            }

            return false;
        }

#if UNITY_EDITOR
        public void SetTransTime(float transTime, State ownerState)
        {
            if (transitionWithTime == null)
            {
                transitionWithTime = new TransCondWithTime(transTime);
            }
            else
            {
                transitionWithTime.m_fConditionTimeValue = transTime;
            }
        }
#endif

        public void RemoveTransTime()
        {
            transitionWithTime = null;
        }

        public void SetNextStateID(STATE_ID nextID)
        {
            nextStateID = nextID;
        }
    }

    [System.Serializable]
    public class TransCondWithTime
    {
        public float m_fConditionTimeValue = 0;
        float fStartTime = 0, fPauseTIme = 0, fPauseInterval = 0;

        public TransCondWithTime(float condTime)
        {
            UDL.Log("                               TransConWithTime " + this.GetHashCode(), FSM.logOption, FSM.logLv);
            m_fConditionTimeValue = condTime;
        }

        public void SetOwnerState(State OwnerState)
        {
            UDL.Log("SetOwnerState : " + OwnerState.eID + " / " + this.GetHashCode(), FSM.logOption, FSM.logLv);

            OwnerState.EventStart += OwnerStart;
            OwnerState.EventEnd += OwnerEnd;
            OwnerState.EventPause += OwnerPause;
            OwnerState.EventResume += OwnerResume;
        }

        void OwnerStart(TRANS_ID transID, STATE_ID stateid, STATE_ID preStateID)
        {
            fStartTime = UnityEngine.Time.realtimeSinceStartup;
            fPauseTIme = 0;
            fPauseInterval = 0;

            UDL.LogWarning("TransCondWithTime Owner Start // current :" + stateid.ToString() + " / " + fStartTime.ToString() + " / pre : " + preStateID + " / transID : " + transID + " / hashcode : " + this.GetHashCode(), FSM.logOption, FSM.warningLoglv);

        }

        void OwnerEnd(TRANS_ID transID, STATE_ID stateid, STATE_ID preStateID)
        {
        }

        void OwnerPause(STATE_ID stateID)
        {
            fPauseTIme = UnityEngine.Time.realtimeSinceStartup;
        }

        void OwnerResume(STATE_ID stateID)
        {
            if (fPauseTIme != 0)
                fPauseInterval = UnityEngine.Time.realtimeSinceStartup - fPauseTIme;
        }

        public void ResetStartTime()
        {
            fStartTime = UnityEngine.Time.realtimeSinceStartup;
            fPauseTIme = 0;
            fPauseInterval = 0;
        }

        public bool TimeConditionChk()
        {
            if (UnityEngine.Time.realtimeSinceStartup - fStartTime - fPauseInterval >= m_fConditionTimeValue)
            {
                UDL.LogWarning("CurrentTime : " + Time.realtimeSinceStartup + " // State start time : " + fStartTime
                         + " // pause interval : " + fPauseInterval + " // condition time : " + m_fConditionTimeValue, FSM.logOption, FSM.warningLoglv);

                return true;
            }

            return false;
        }
    }

    [System.Serializable]
    public class TransCondWithParam
    {
        public TRANS_PARAM_ID m_uiParamID { get; private set; }

        public TransitionType m_eTransType { get; private set; }
        public TransitionComparisonOperator m_eCompOperator { get; private set; }

        const string warningmsg = "사용하지 않는 전이조건 변수를 설정했습니다.";

        int m_iConditionValue = 0;

         int nParamBuffer = 0;
         float fParamBuffer;
         bool bParamBuffer;
         State tStateBuffer;

        public int M_iConditionValue
        {
            get
            {
                return m_iConditionValue;
            }
            set
            {
                if (m_eTransType != TransitionType.INT)
                    UDL.LogWarning(warningmsg, FSM.logOption, FSM.warningLoglv);

                m_iConditionValue = value;
            }
        }

        float m_fConditionValue = 0;
        public float M_fConditionValue
        {
            get { return m_fConditionValue; }
            set
            {
                if (m_eTransType != TransitionType.FLOAT)
                    UDL.LogWarning(warningmsg, FSM.logOption, FSM.warningLoglv);

                m_fConditionValue = value;
            }
        }

        bool m_bConditionValue = false;
        public bool M_bConditionValue
        {
            get { return m_bConditionValue; }
            set
            {
                if (m_eTransType != TransitionType.BOOL)
                    UDL.LogWarning(warningmsg, FSM.logOption, FSM.warningLoglv);

                m_bConditionValue = value;
            }
        }

        TRANS_PARAM_ID m_TriggerID = 0;
        public TRANS_PARAM_ID M_TriggerID
        {
            get { return m_TriggerID; }
            set
            {
                if (m_eTransType != TransitionType.TRIGGER)
                    UDL.LogWarning(warningmsg, FSM.logOption, FSM.warningLoglv);

                m_TriggerID = value;
            }
        }

        /// <summary>
        /// 해당 FSM이 가지고 있는 파라메터의 값들과 설정된 값을 비교해서 조건을 만족하는지 검사할 수 있다. 
        /// </summary>
        /// <param name="_paramID">FSM이 가지고 있는 파라메터 ID를 입력한다. </param>
        public TransCondWithParam(TransitionType eT, TRANS_PARAM_ID _paramID = 0, object conditionValue = null, TransitionComparisonOperator eCompOp = TransitionComparisonOperator.EQUALS)
        {
            m_eTransType = eT;

            if (eT != TransitionType.TRIGGER && _paramID == 0)
                UDL.LogError("파라메터 아이디를 설정해야 합니다!", FSM.logOption);

            m_uiParamID = _paramID;

            if (eT != TransitionType.TRIGGER && conditionValue == null)
                UDL.LogError("조건 변수를 설정해야 합니다. ", FSM.logOption);

            switch (eT)
            {
                case TransitionType.BOOL:
                    M_bConditionValue = (bool)conditionValue;
                    break;
                case TransitionType.FLOAT:
                    M_fConditionValue = (float)conditionValue;
                    break;
                case TransitionType.INT:
                    M_iConditionValue = (int)conditionValue;
                    break;
                case TransitionType.TRIGGER:
                    M_TriggerID = _paramID;
                    break;
            }

            m_eCompOperator = eCompOp;
        }

        public void SetTransitionType(TransitionType e)
        {
            m_eTransType = e;
        }

        public void SetParamID(TRANS_PARAM_ID _paramID)
        {
            m_uiParamID = _paramID;

            if (m_eTransType == TransitionType.TRIGGER)
                M_TriggerID = _paramID;
        }

        public void SetConditionValue(object conditionValue)
        {
            switch (m_eTransType)
            {
                case TransitionType.BOOL:
                    M_bConditionValue = (bool)conditionValue;
                    break;
                case TransitionType.FLOAT:
                    M_fConditionValue = (float)conditionValue;
                    break;
                case TransitionType.INT:
                    M_iConditionValue = (int)conditionValue;
                    break;
            }
        }

        public void SetCompOperator(TransitionComparisonOperator eOp)
        {
            m_eCompOperator = eOp;
        }


        public bool ConditionCheck(FSM pFSM)
        {
            switch (m_eTransType)
            {
                case TransitionType.INT:
                    return IntTypeCondtionChk(m_iConditionValue, pFSM);
                case TransitionType.FLOAT:
                    return FloatTypeCondtionChk(m_fConditionValue, pFSM);
                case TransitionType.BOOL:
                    return BoolTypeConditionChk(m_bConditionValue, pFSM);
                case TransitionType.TRIGGER:
                    return TriggerTypeConditionChk(m_TriggerID, pFSM);
            }

            return false;
        }

        bool IntTypeCondtionChk(int value, FSM pFSM)
        {
            if (!pFSM.dicIntParam.TryGetValue(m_uiParamID, out nParamBuffer))
                UDL.LogError(m_uiParamID.ToString() + ". 등록되지 않은 Trans_Param_ID 입니다. ", FSM.logOption, FSM.errorLoglv);

            switch (m_eCompOperator)
            {
                case TransitionComparisonOperator.EQUALS:
                    if (nParamBuffer == value)
                        return true;
                    break;
                case TransitionComparisonOperator.NOTEQUAL:
                    if (nParamBuffer != value)
                        return true;
                    break;
                case TransitionComparisonOperator.GREATER:
                    if (nParamBuffer >= value)
                        return true;
                    break;
                case TransitionComparisonOperator.LESS:
                    if (nParamBuffer <= value)
                        return true;
                    break;
                case TransitionComparisonOperator.OVER:
                    if (nParamBuffer > value)
                        return true;
                    break;
                case TransitionComparisonOperator.UNDER:
                    if (nParamBuffer < value)
                        return true;
                    break;
            }

            return false;
        }

        bool FloatTypeCondtionChk(float value, FSM pFSM)
        {
            if (!pFSM.dicFloatParam.TryGetValue(m_uiParamID, out fParamBuffer))
                UDL.LogError(m_uiParamID.ToString() + ". 등록되지 않은 Trans_Param_ID 입니다. ", FSM.logOption, FSM.errorLoglv);

            switch (m_eCompOperator)
            {
                case TransitionComparisonOperator.EQUALS:

                    UDL.LogWarning("float 변수는 Equal 조건을 만족시키지 못 할 위험이 있습니다. ", FSM.logOption, FSM.warningLoglv);

                    if (fParamBuffer == value)
                        return true;
                    break;
                case TransitionComparisonOperator.NOTEQUAL:
                    UDL.LogWarning("float 변수는 NotEqual 조건을 사용하면 정확하지 않을 위험이 있습니다. ", FSM.logOption, FSM.warningLoglv);

                    if (fParamBuffer != value)
                        return true;
                    break;
                case TransitionComparisonOperator.GREATER:
                    if (fParamBuffer >= value)
                        return true;
                    break;
                case TransitionComparisonOperator.LESS:
                    if (fParamBuffer <= value)
                        return true;
                    break;
                case TransitionComparisonOperator.OVER:
                    if (fParamBuffer > value)
                        return true;
                    break;
                case TransitionComparisonOperator.UNDER:
                    if (fParamBuffer < value)
                        return true;
                    break;
            }

            return false;
        }

        bool BoolTypeConditionChk(bool value, FSM pFSM)
        {
            if (!pFSM.dicBoolParam.TryGetValue(m_uiParamID, out bParamBuffer))
                UDL.LogError(m_uiParamID.ToString() + ". 등록되지 않은 Trans_Param_ID 입니다. ", FSM.logOption, FSM.errorLoglv);

            switch (m_eCompOperator)
            {
                case TransitionComparisonOperator.LESS:
                case TransitionComparisonOperator.GREATER:
                case TransitionComparisonOperator.OVER:
                case TransitionComparisonOperator.UNDER:
                    UDL.LogWarning("bool 변수에 잘못된 조건연산자를 지정했음", FSM.logOption, FSM.warningLoglv);
                    return false;
                case TransitionComparisonOperator.EQUALS:
                    if (bParamBuffer == value)
                        return true;
                    break;
                case TransitionComparisonOperator.NOTEQUAL:
                    if (bParamBuffer != value)
                        return true;
                    break;
            }

            return false;
        }

        bool TriggerTypeConditionChk(TRANS_PARAM_ID triggerID, FSM pFSM)
        {
            if (triggerID == pFSM.triggerID)
                return true;

            return false;
        }

    }
}