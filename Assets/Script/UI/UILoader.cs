using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FiniteStateMachine;

public class UILoader : MonoBehaviour
{
    [SerializeField]
    Slider progressBar;
    [SerializeField]
    Text progressText;
    
    void Awake()
    {
        State tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Main, STATE_ID.Main_Loading);
        tstate.EventStart += OnStart_USLoading;
    }

    private void OnDestroy()
    {
        State tstate = FSM_Layer.Inst.GetState(FSM_LAYER_ID.UserStory, FSM_ID.Main, STATE_ID.Main_Loading);
        tstate.EventStart -= OnStart_USLoading;
    }

    private void OnStart_USLoading(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        StartCoroutine(LoadUI());
    }

    /// <summary>
    /// UIPrefab 개수와 무관하게 항상 비동기로 UI를 로딩하고 진행율을 표시한다. 
    /// - Resouces/UIPrefab/LoadAtStart 경로에 UIPrefab을 저장
    /// - 새로운 Prefab이 추가될 경우 Tools/MakeUIList 실행해서 Prefab 목록 갱신
    /// </summary>
    IEnumerator LoadUI()
    {
        GameObject uiRoot = new GameObject("UI Root");
        List<string> uilist = FileManager.Inst.ResourceLoad("UIPrefab/uilist") as List<string>;
        float sumProgress = 0;

        for (int i = 0; i < uilist.Count; i++)
        {
            ResourceRequest request = Resources.LoadAsync("UIPrefab/LoadAtStart/"+uilist[i]);

            sumProgress = (float)i / uilist.Count;

            while (!request.isDone)
            {
                progressBar.value = sumProgress + (request.progress / uilist.Count);
                progressText.text = Mathf.FloorToInt(progressBar.value * 100).ToString();

                yield return request;
            }
            
            GameObject obj = Instantiate(request.asset) as GameObject;
            obj.transform.SetParent(uiRoot.transform);
        }
        
        progressBar.value = 1;
        progressText.text = "100";

        //Main_Loading -> Main_Entrance
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
    }
}
