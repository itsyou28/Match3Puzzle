using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

[System.Serializable]
public class ReactionData
{
    public FSM_LAYER_ID layer;
    public FSM_ID fsm;
    public STATE_ID state;
    public ReactionID onStartBefore;
    public ReactionID onStart;
    public ReactionID onStartAfter1;
    public ReactionID onStartAfter2;
    public ReactionID onEndBefore;
    public ReactionID onEnd;
    public ReactionID onEndAfter;
    public ReactionID onPause;
    public ReactionID onResume;

    public int onStartBeforeExcuteId;
    public int onStartExcuteId;
    public int onStartAfter1ExcuteId;
    public int onStartAfter2ExcuteId;
    public int onEndBeforeExcuteId;
    public int onEndExcuteId;
    public int onEndAfterExcuteId;
    public int onPauseExcuteId;
    public int onResumeExcuteId;
}

public enum ReactionID
{
    None = 0,
    Show,
    Hide,
    Excute,
    Change
}

public interface iStateReaction
{
    void Show();
    void Hide();
    void Excute(int excuteid);
    void Change();
}

/// <summary>
/// FSM 생성 및 설정 과정 및 UIBinder 등록 절차를 보장할 수 있도록 
/// ExcutionOrder를 Default 이 후 시점으로 설정한다. 
/// </summary>
public class ReactionByState : MonoBehaviour, iStateReaction
{
    protected const int nLogOption = (int)LogOption.FSM_Reaction;
    protected const int nLogLevel = 8;
    protected const int nLogWarningLevel = 7;
    protected const int nLogErrorLevel = 6;

    [SerializeField]
    ReactionData[] arrSwitch;

    [SerializeField]
    bool isDebug = false;
    [SerializeField]
    bool hideFirstFrame = false;

    public bool bIsDebug { get { return isDebug; } }

    iStateReaction iReaction = null;

    bool bIsInitialze = false;

    Dictionary<STATE_ID, ReactionData> dicIndex = new Dictionary<STATE_ID, ReactionData>();
    
    ///Awake가 호출되지 않으면 작동하지 않기 때문에 반드시 Scene상에서 해당 오브젝트가 Active 상태여야 한다. 
    ///Start 시점에서는 FSM이 실행되기 때문에 반드시 Awake시점에서 Initailize 과정이 완료되어야 한다. 
    private void Awake()
    {
        if (SceneChecker.IS_NOT_USING_FSM_SCENE)
            return;

        UDL.Log(gameObject.name + " Awake // self : " + gameObject.activeSelf + " hierarchy : " + gameObject.activeInHierarchy, nLogOption, isDebug, nLogLevel);

        //상위 노드에 의해 초기화 되지 않았다면 초기화를 시작한다. 
        if (!bIsInitialze)
            Initialize_withChild();
    }

    ///child node내의 동일 클래스 RegistEvent를 보장하기 위해 하위노드를 검색해서 모두 호출해준다. 
    private void Initialize_withChild()
    {
        UDL.Log(gameObject.name + " Initialize", nLogOption, isDebug, nLogLevel);

        Initialize();

        //초기화 후 스스로 비활성화되기 때문에 직접 하위 노드 초기화 함수를 호출시켜준다. 
        ReactionByState[] arrChild = GetComponentsInChildren<ReactionByState>();

        for (int idx = 0; idx < arrChild.Length; idx++)
            arrChild[idx].Initialize();
    }
    
    private void OnEnable()
    {
        if (hideFirstFrame)
            StartCoroutine(DelayOneFrame());

        UDL.Log(gameObject.name + " OnEnable", nLogOption, isDebug, nLogLevel);
    }

    private void OnDisable()
    {
        if (hideFirstFrame)
            transform.localScale = Vector3.zero;

        UDL.Log(gameObject.name + " Disable", nLogOption, isDebug, nLogLevel);
    }

    private void OnDestroy()
    {
        if (SceneChecker.IS_NOT_USING_FSM_SCENE)
            return;

        State pState;

        for (int idx = 0; idx < arrSwitch.Length; idx++)
        {
            pState = FSM_Layer.Inst.GetState(arrSwitch[idx].layer, arrSwitch[idx].fsm, arrSwitch[idx].state);

#if UNITY_EDITOR
            if (pState == null)
                UDL.LogError(gameObject.name + " // " + arrSwitch[idx].state, nLogOption);
#endif

            pState.EventStart_Before -= OnStartBefore;
            pState.EventStart -= OnStart;
            pState.EventStart_After1 -= OnStartAfter1;
            pState.EventStart_After2 -= OnStartAfter2;
            pState.EventEnd_Before -= OnEndBefore;
            pState.EventEnd -= OnEnd;
            pState.EventEnd_After -= OnEndAfter;
            pState.EventPause -= OnPause;
            pState.EventResume -= OnResume;            
        }
    }

    private void Initialize()
    {
        if (bIsInitialze)
            return;

        bIsInitialze = true;

        if (hideFirstFrame)
            transform.localScale = Vector3.zero;

        UDL.Log(gameObject.name + " RegistEvent", nLogOption, isDebug, nLogLevel);

        if (iReaction == null)
            iReaction = this;

        State pState;
        
        for(int idx=0; idx<arrSwitch.Length; idx++)
        {
            pState = FSM_Layer.Inst.GetState(arrSwitch[idx].layer, arrSwitch[idx].fsm, arrSwitch[idx].state);
            
            if (pState == null)
            {
                UDL.LogError(gameObject.name + " // " + arrSwitch[idx].state, nLogOption);
                return;
            }

            pState.EventStart_Before += OnStartBefore;
            pState.EventStart += OnStart;
            pState.EventStart_After1 += OnStartAfter1;
            pState.EventStart_After2 += OnStartAfter2;
            pState.EventEnd_Before += OnEndBefore;
            pState.EventEnd += OnEnd;
            pState.EventEnd_After += OnEndAfter;
            pState.EventPause += OnPause;
            pState.EventResume += OnResume;

#if UNITY_EDITOR
            try
            {
                dicIndex.Add(arrSwitch[idx].state, arrSwitch[idx]); 
            }
            catch
            {
                UDL.LogError(gameObject.name + " overlap state " + arrSwitch[idx].state, nLogOption);
            }
#else
            dicIndex.Add(arrSwitch[idx].state, arrSwitch[idx]); 
#endif
        }

        Reaction_Expand p = GetComponent<Reaction_Expand>();
        if(p)
            p.Initialize();

        gameObject.SetActive(false);
    }

    private void OnEndAfter(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        ReactionProcess(dicIndex[preStateID].onEndAfter, dicIndex[preStateID].onEndAfterExcuteId);

        UDL.Log(gameObject.name + " OnEndAfter : " + preStateID, nLogOption, isDebug, nLogLevel);
    }

    private void OnEndBefore(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        ReactionProcess(dicIndex[preStateID].onEndBefore, dicIndex[preStateID].onEndBeforeExcuteId);

        UDL.Log(gameObject.name + " OnEndBefore : " + stateID, nLogOption, isDebug, nLogLevel);
    }

    private void OnStartAfter2(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        ReactionProcess(dicIndex[stateID].onStartAfter2, dicIndex[stateID].onStartAfter2ExcuteId);

        UDL.Log(gameObject.name + " OnStartAfter2 : " + stateID, nLogOption, isDebug, nLogLevel);
    }

    IEnumerator DelayOneFrame()
    {
        yield return true;

        transform.localScale = Vector3.one;
    }

    private void OnStartAfter1(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        ReactionProcess(dicIndex[stateID].onStartAfter1, dicIndex[stateID].onStartAfter1ExcuteId);

        UDL.Log(gameObject.name + " OnStartAfter1 : " + stateID, nLogOption, isDebug, nLogLevel);
    }

    private void OnStartBefore(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        ReactionProcess(dicIndex[stateID].onStartBefore, dicIndex[stateID].onStartBeforeExcuteId);

        UDL.Log(gameObject.name + " OnStartBefore : " + stateID, nLogOption, isDebug, nLogLevel);
    }

    private void OnResume(STATE_ID stateID)
    {
        ReactionProcess(dicIndex[stateID].onResume, dicIndex[stateID].onResumeExcuteId);

        UDL.Log(gameObject.name + " OnResume : " + stateID, nLogOption, isDebug, nLogLevel);
    }

    private void OnPause(STATE_ID stateID)
    {
        ReactionProcess(dicIndex[stateID].onPause, dicIndex[stateID].onPauseExcuteId);

        UDL.Log(gameObject.name + " OnPause : " + stateID, nLogOption, isDebug, nLogLevel);
    }

    private void OnEnd(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        ReactionProcess(dicIndex[preStateID].onEnd, dicIndex[preStateID].onEndExcuteId);

        UDL.Log(gameObject.name + " OnEnd : " + preStateID + " // after active : " + gameObject.activeSelf, nLogOption, isDebug, nLogLevel);
    }

    private void OnStart(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        ReactionProcess(dicIndex[stateID].onStart, dicIndex[stateID].onStartExcuteId);

        UDL.Log(gameObject.name + " OnStart : " + stateID + " // after active : " + gameObject.activeSelf, nLogOption, isDebug, nLogLevel);
    }

    void ReactionProcess(ReactionID ss, int excuteId)
    {
        if(ss != ReactionID.None)
            UDL.Log(gameObject.name + " ReactionProcess : " + ss + " // " + excuteId, nLogOption, isDebug, nLogLevel);

        switch (ss)
        {
            case ReactionID.None:
                break;
            case ReactionID.Show:
                iReaction.Show();
                break;
            case ReactionID.Hide:
                iReaction.Hide();
                break;
            case ReactionID.Excute:
                iReaction.Excute(excuteId);
                break;
            case ReactionID.Change:
                iReaction.Change();
                break;
        }
    }

    public void Show()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public void Hide()
    {
        BroadcastMessage("OnBeforeDisable", SendMessageOptions.DontRequireReceiver);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
    
    public void Excute(int excuteId)
    {
    }

    public void Change()
    {
    }
    
    /// <summary>
    /// Awake 시점에서 호출해야 한다. 
    /// Start 에서 지정할 경우 gameobject가 다시 active 될 때까지 Start가 미뤄져서 최초 이벤트를 받지 못한다. 
    /// </summary>
    /// <param name="target"></param>
    public void SetReaction(iStateReaction target)
    {
        UDL.Log(gameObject.name + " SetReaction. // " + gameObject.activeSelf, nLogOption, isDebug, nLogLevel);

        iReaction = target;
    }
}
