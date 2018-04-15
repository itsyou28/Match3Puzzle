using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    }

    public void ClickRemove()
    {
        //파일 삭제 컨펌
        Destroy(gameObject);
    }

    public void SetRowData(StageRowData data)
    {
        stageName.text = data.stageName;
        this.data = data;
    }

    public void OnDestroy()
    {
        data.callbackDestroy(data);
    }


}
