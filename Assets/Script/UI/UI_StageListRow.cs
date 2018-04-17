using UnityEngine;
using UnityEngine.UI;
using FiniteStateMachine;

public class StageRowData
{
    public string stageName;
    public System.Action<StageRowData> callbackDestroy;
}

public class UI_StageListRow : MonoBehaviour
{
    [SerializeField]
    Text stageName;

    StageRowData data;

    public void ClickRow()
    {
        //스테이지 선택 이벤트 발생
        EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.SELECT_STAGE, data.stageName);
        FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
    }

    public void ClickRemove()
    {
        //파일 삭제 컨펌
        EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.POPUP,
            "Delete Confirm", data.stageName + " 삭제??", 1,
            (System.Action<bool>)CallbackPopup);
    }

    private void CallbackPopup(bool result)
    {
        if (result)
        {
            DataFileManager.Inst.stageDataFile.RemoveStage(data.stageName);
            Destroy(gameObject);
        }
    }

    public void SetRowData(StageRowData data)
    {
        stageName.text = data.stageName;
        this.data = data;
    }

    public void OnDestroy()
    {
        if (data.callbackDestroy != null)
            data.callbackDestroy(data);
    }


}
