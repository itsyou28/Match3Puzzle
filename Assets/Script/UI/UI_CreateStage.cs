using UnityEngine;
using UnityEngine.UI;

public class UI_CreateStage : MonoBehaviour
{
    [SerializeField]
    InputField newStageName;

    public void OnChangeStageName(string text)
    {
        if (text.Length >= 21)
        {
            newStageName.text = text.Remove(20);

            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.POPUP, "Warning", "스테이지 명은 20글자 이하로 해주세요", 0);
        }
    }

    public void Click()
    {
        if (newStageName.text.Length <= 0)
            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.POPUP, "스테이지 생성 실패", "스테이지 명을 입력해주세요", 0);
        if (DataManager.Inst.stageData.CheckExist(newStageName.text))
            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.POPUP, "스테이지 생성 실패", "동일한 스테이지 이름이 존재합니다", 0);
        else
        {
            DataManager.Inst.stageData.AddStage(newStageName.text, 
                BlockFieldMaker.CreateField(12, 15));
            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.CREATE_STAGE, newStageName.text);
        }
    }

    private void OnDestroy()
    {
        DataManager.Inst.stageData.Save();
    }
}
