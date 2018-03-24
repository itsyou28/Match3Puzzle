using System.Collections;
using System.Collections.Generic;

namespace FiniteStateMachine
{
    public class FSM_Layer
    {
        private static FSM_Layer instance = null;
        public static FSM_Layer Inst
        {
            get
            {
                if (instance == null)
                    instance = new FSM_Layer();
                return instance;
            }
        }

        const int iMaxLayer = 5;

        Dictionary<FSM_ID, FSM>[] dicFSM_EachLayer = new Dictionary<FSM_ID, FSM>[iMaxLayer];

        FSM[] curFSM_EachLayer = new FSM[iMaxLayer];
        FSM_ID[] layerFSM_Buffer = new FSM_ID[iMaxLayer];

        event deleLayerPauseResume EventLayerPause;
        event deleLayerPauseResume EventLayerResume;

        Dictionary<FSM_LAYER_ID, List<deleStateTransEvent>> dicLayerChangeState = new Dictionary<FSM_LAYER_ID, List<deleStateTransEvent>>();

        const int nLogOption = (int)LogOption.FSM_Layer;

        const int logLv = 8;
        const int warningLoglv = 7;
        const int errorLoglv = 6;

        int layerNum;

        int nParamBuffer;
        float fParamBuffer;
        bool bParamBuffer;
        FSM tFSMBuffer;

        public void ReleaseAll()
        {
            curFSM_EachLayer = new FSM[iMaxLayer];

            for (int i = 0; i < iMaxLayer; i++)
            {
                if (dicFSM_EachLayer[i] != null)
                    dicFSM_EachLayer[i].Clear();
            }

            dicFSM_EachLayer = null;

            EventLayerPause = null;
            EventLayerResume = null;

            dicLayerChangeState = null;
        }

        public void AddFSM(FSM_LAYER_ID eLayer, FSM newFSM, FSM_ID id = FSM_ID.NONE)
        {
            layerNum = (int)eLayer;
            if (dicFSM_EachLayer.Length <= layerNum)
            {
                UDL.LogError("할당되지 않은 레이어를 호출하고 있습니다 Layer 및 iMaxLayer를 확인해주세요", nLogOption, errorLoglv);
                return;
            }

            if (dicFSM_EachLayer[layerNum] == null)
                dicFSM_EachLayer[layerNum] = new Dictionary<FSM_ID, FSM>();

            if (id != FSM_ID.NONE)
            {
                if (dicFSM_EachLayer[layerNum].ContainsKey(id))
                  UDL.LogWarning("동일 레이어에 중복된 FSM ID 를 등록하려함!", nLogOption, warningLoglv); 
                else
                    newFSM.SetFSMID(id);
            }

            dicFSM_EachLayer[layerNum].Add(newFSM.fsmID, newFSM);

            UDL.Log(eLayer + " // add : " + newFSM.fsmID, nLogOption, logLv);

            if (curFSM_EachLayer[layerNum] == null)
            {
                curFSM_EachLayer[layerNum] = newFSM;

                RegisterToFSM_ChangeLayerState(eLayer);

                curFSM_EachLayer[layerNum].Resume();
            }
            else
                ChangeFSM(eLayer, newFSM.fsmID);
        }

        public void RemoveFSM(FSM_LAYER_ID eLayer, FSM_ID id)
        {
            layerNum = (int)eLayer;
            if (dicFSM_EachLayer[layerNum].ContainsKey(id))
            {
                dicFSM_EachLayer[layerNum].Remove(id);
            }
        else
            UDL.LogWarning("가지고 있지 않은 FSM을 삭제하려 함", nLogOption, warningLoglv); 

            if (curFSM_EachLayer[layerNum].fsmID == id)
                curFSM_EachLayer[layerNum] = null;
        }

        public void SetInt_NoCondChk(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID, int value)
        {
            if (CurLayerCheck(eLayer))
                curFSM_EachLayer[(int)eLayer].SetInt_NoCondChk(paramID, value);
        }

        public void SetFloat_NoCondChk(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID, float value)
        {
            if (CurLayerCheck(eLayer))
                curFSM_EachLayer[(int)eLayer].SetFloat_NoCondChk(paramID, value);
        }

        public void SetBool_NoCondChk(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID, bool value)
        {
            if (CurLayerCheck(eLayer))
                curFSM_EachLayer[(int)eLayer].SetBool_NoCondChk(paramID, value);
        }



        public void SetInt(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID, int value)
        {
            if (CurLayerCheck(eLayer))
                curFSM_EachLayer[(int)eLayer].SetInt(paramID, value);
        }

        public void SetFloat(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID, float value)
        {
            if (CurLayerCheck(eLayer))
                curFSM_EachLayer[(int)eLayer].SetFloat(paramID, value);
        }

        public void SetBool(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID, bool value)
        {
            if (CurLayerCheck(eLayer))
                curFSM_EachLayer[(int)eLayer].SetBool(paramID, value);
        }

        public void SetTrigger(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID)
        {
            if (CurLayerCheck(eLayer))
                curFSM_EachLayer[(int)eLayer].SetTrigger(paramID);
        }



        public int GetInt(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID)
        {
            if (!CurLayerCheck(eLayer))
                return 0;
            return curFSM_EachLayer[(int)eLayer].GetParamInt(paramID);
        }

        public float GetFloat(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID)
        {
            if (!CurLayerCheck(eLayer))
                return 0;
            return curFSM_EachLayer[(int)eLayer].GetParamFloat(paramID);
        }

        public bool GetBool(FSM_LAYER_ID eLayer, TRANS_PARAM_ID paramID)
        {
            if (!CurLayerCheck(eLayer))
                return false;
            return curFSM_EachLayer[(int)eLayer].GetParamBool(paramID);
        }

        bool CurLayerCheck(FSM_LAYER_ID eLayer)
        {
            if (curFSM_EachLayer[(int)eLayer] == null)
            {
                UDL.LogWarning(eLayer + " 지정한 레이어에 FSM이 지정되 있지 않음", nLogOption, warningLoglv);
                return false;
            }
            else
                return true;
        }


        public FSM GetCurFSM(FSM_LAYER_ID eLayer)
        {
            if (!CurLayerCheck(eLayer))
                return null;

            return curFSM_EachLayer[(int)eLayer];
        }

        public FSM GetFSM(FSM_LAYER_ID eLayer, FSM_ID fsmID)
        {
            layerNum = (int)eLayer;
            if (dicFSM_EachLayer == null)
                return null;

            if (dicFSM_EachLayer[layerNum] == null)
            {
                UDL.LogError("지정한 레이어에 FSM이 지정되 있지 않음 ", nLogOption, errorLoglv);
                return null;
            }

            if (!dicFSM_EachLayer[layerNum].TryGetValue(fsmID, out tFSMBuffer))
            {
                UDL.LogError(fsmID.ToString() + " FSM 이 등록되어 있지 않음", nLogOption, errorLoglv);
                return null;
            }

            return tFSMBuffer;
        }

        public State GetState(FSM_LAYER_ID eLayer, FSM_ID fsmID, STATE_ID stateID)
        {
            layerNum = (int)eLayer;
            if (dicFSM_EachLayer == null)
                return null;

            if (dicFSM_EachLayer[layerNum] == null)
            {
                UDL.LogError("지정한 레이어에 FSM이 지정되 있지 않음 ", nLogOption, errorLoglv);
                return null;
            }

            if (!dicFSM_EachLayer[layerNum].TryGetValue(fsmID, out tFSMBuffer))
            {
                UDL.LogError(fsmID.ToString() + " FSM 이 등록되어 있지 않음", nLogOption, errorLoglv);
                return null;
            }

            return tFSMBuffer.GetState(stateID);
        }

        public State GetCurState(FSM_LAYER_ID eLayer)
        {
            layerNum = (int)eLayer;
            if (curFSM_EachLayer[layerNum] == null)
                return null;

            return curFSM_EachLayer[layerNum].GetCurState();
        }

        public STATE_ID GetCurStateID(FSM_LAYER_ID eLayer)
        {
            layerNum = (int)eLayer;
            if (curFSM_EachLayer[layerNum] == null)
                return STATE_ID.None;

            return curFSM_EachLayer[layerNum].GetCurStateID();
        }

        public void HistoryBack(FSM_LAYER_ID eLayer)
        {
            if (CurLayerCheck(eLayer))
                curFSM_EachLayer[(int)eLayer].HistoryBack();
        }

        public void ChangeFSM(FSM_LAYER_ID eLayer, FSM_ID fsmID)
        {
            layerNum = (int)eLayer;

            if (!CurLayerCheck(eLayer))
            {
                UDL.LogWarning("Fail Change Layer : " + eLayer, nLogOption, warningLoglv);
                return;
            }

            if (!dicFSM_EachLayer[layerNum].TryGetValue(fsmID, out tFSMBuffer))
            {
                UDL.LogWarning(eLayer + " 에 " + fsmID + " 가 등록되어 있지 않습니다", nLogOption, warningLoglv);
                return;
            }

            if (CurLayerCheck(eLayer))
            {
                layerFSM_Buffer[layerNum] = curFSM_EachLayer[layerNum].fsmID;

                curFSM_EachLayer[layerNum].Pause();

                UnRegisterToFSM_ChangeLayerState(eLayer);

                curFSM_EachLayer[layerNum] = dicFSM_EachLayer[layerNum][fsmID];

                RegisterToFSM_ChangeLayerState(eLayer);

                curFSM_EachLayer[layerNum].Resume();
            }

            UDL.Log("(ChangeFSM) curFSM : " + curFSM_EachLayer[layerNum].fsmID.ToString(), nLogOption, logLv); 
        }

        public void ChangeFSM_Manual(FSM_LAYER_ID eLayer, FSM_ID fsmID)
        {
            layerNum = (int)eLayer;
            if (CurLayerCheck(eLayer))
            {
                layerFSM_Buffer[layerNum] = curFSM_EachLayer[layerNum].fsmID;

                UnRegisterToFSM_ChangeLayerState(eLayer);

                curFSM_EachLayer[layerNum] = dicFSM_EachLayer[layerNum][fsmID];

                RegisterToFSM_ChangeLayerState(eLayer);
            }
        }

        public void ChangeFSM_BufferBack(FSM_LAYER_ID eLayer)
        {
            ChangeFSM(eLayer, layerFSM_Buffer[layerNum]);
        }

        string[] errMsgAbout_Register_ChangeLayerState =
        {
            "Success",
            "해당 레이어에 FSM이 등록되어 있지 않습니다. ",
            "해당 레이어의 이벤트 버퍼 리스트가 존재하지 않습니다"
        };

        private int RegisterToFSM_ChangeLayerState(FSM_LAYER_ID eLayer)
        {
            layerNum = (int)eLayer;
            if (!CurLayerCheck(eLayer))
                return 1;

            UnRegisterToFSM_ChangeLayerState(eLayer);//중복등록을 방지하기 위해 호출한다. 

            if (!dicLayerChangeState.ContainsKey(eLayer))
                return 2;

            for (int idx = 0; idx < dicLayerChangeState[eLayer].Count; idx++)
                curFSM_EachLayer[layerNum].EventStateChange += dicLayerChangeState[eLayer][idx];

            return 0;
        }

        private int UnRegisterToFSM_ChangeLayerState(FSM_LAYER_ID eLayer)
        {
            if (!CurLayerCheck(eLayer))
                return 1;

            if (!dicLayerChangeState.ContainsKey(eLayer))
                return 2;

            for (int idx = 0; idx < dicLayerChangeState[eLayer].Count; idx++)
                curFSM_EachLayer[layerNum].EventStateChange -= dicLayerChangeState[eLayer][idx];

            return 0;
        }

        public void RegisterEventChangeLayerState(FSM_LAYER_ID eLayer, deleStateTransEvent _deleFunc)
        {
            layerNum = (int)eLayer;
            if (layerNum >= iMaxLayer)
            {
                UDL.LogError("할당 되지 않은 레이어를 호출했습니다", nLogOption, errorLoglv);
                return;
            }

            if (!dicLayerChangeState.ContainsKey(eLayer))
                dicLayerChangeState.Add(eLayer, new List<deleStateTransEvent>());

            dicLayerChangeState[eLayer].Add(_deleFunc);
            
            int result = RegisterToFSM_ChangeLayerState(eLayer);

            if (result == 1)
                UDL.LogWarning(eLayer + " " + errMsgAbout_Register_ChangeLayerState[result] + " // 이 후 해당 레이어에 ChangeFSM이 호출 될 때 반영될 수 있습니다. ", nLogOption, warningLoglv);

        }

        public void UnRegisterChangeLayerState(FSM_LAYER_ID eLayer, deleStateTransEvent _deleFunc)
        {
            if (!dicLayerChangeState.ContainsKey(eLayer))
                return;

            dicLayerChangeState[eLayer].Remove(_deleFunc);


            foreach (Dictionary<FSM_ID, FSM> dic in dicFSM_EachLayer)
            {
                foreach (FSM fsm in dic.Values)
                    fsm.EventStateChange -= _deleFunc;
            }
        }

        public void RegisterPauseEvent(deleLayerPauseResume _deleFunc)
        {
            EventLayerPause += _deleFunc;
        }

        public void UnRegisterPauseEvent(deleLayerPauseResume _deleFunc)
        {
            EventLayerPause -= _deleFunc;
        }

        public void RegisterResumeEvent(deleLayerPauseResume _deleFunc)
        {
            EventLayerResume += _deleFunc;
        }

        public void UnRegisterResumeEvent(deleLayerPauseResume _deleFunc)
        {
            EventLayerResume -= _deleFunc;
        }

        public void Pause(FSM_LAYER_ID eLayer)
        {
            if (!CurLayerCheck(eLayer))
                return;

            curFSM_EachLayer[layerNum].Pause();

            if (EventLayerPause != null)
                EventLayerPause(eLayer);
        }

        public void Resume(FSM_LAYER_ID eLayer)
        {
            if (!CurLayerCheck(eLayer))
                return;

            curFSM_EachLayer[layerNum].Resume();

            if (EventLayerResume != null)
                EventLayerResume(eLayer);
        }

        public void Update()
        {
            for (int i = 0; i < iMaxLayer; i++)
            {
                if (curFSM_EachLayer[i] != null)
                    curFSM_EachLayer[i].TimeCheck();
            }
        }

        public void ReleaseLayer(FSM_LAYER_ID eLayer)
        {
            dicFSM_EachLayer[layerNum].Clear();
            curFSM_EachLayer[layerNum] = null;
        }
    }
}