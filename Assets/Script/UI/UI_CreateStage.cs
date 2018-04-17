using UnityEngine;
using UnityEngine.UI;
using FiniteStateMachine;

public class UI_CreateStage : MonoBehaviour
{
    [SerializeField]
    InputField newStageName;

    public void OnChangeStageName(string text)
    {
        if (text.Length >= 11)
        {
            newStageName.text = text.Remove(10);

            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.POPUP, "Warning", "스테이지 명은 10글자 이하로 해주세요", 0);
        }
    }

    public void Click()
    {
        if (newStageName.text.Length <= 0)
            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.POPUP, "스테이지 생성 실패", "스테이지 명을 입력해주세요", 0);
        if (DataFileManager.Inst.stageDataFile.CheckExist(newStageName.text))
            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.POPUP, "스테이지 생성 실패", "동일한 스테이지 이름이 존재합니다", 0);
        else
        {
            DataFileManager.Inst.stageDataFile.AddStage(newStageName.text, 
                BlockFieldMaker.CreateField(10, 10));
            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.CREATE_STAGE, newStageName.text);
        }
    }

    private void OnDestroy()
    {
        DataFileManager.Inst.stageDataFile.Save();
    }
}
