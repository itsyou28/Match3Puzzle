using UnityEngine;

public struct ClearCondition
{
    public int blockType;
    public int requireCnt;
}

public class ClearChecker
{
    //드디어 클리어 조건 컨트롤
    //클리어 조건은 2개이상일 수 있다
    //카운트를 만족한다
    //블럭 카운트를 만족한다. 
    //fsm 조건 만족기에서 구현했던거랑 방식은 비슷할 것이다. 
    //스페이지 이름과 동일하게 조건 만족 구조체를 생성한다. 
    //클리어 조건은 무엇이 있고 어떤 형식으로 저장해야 하나

    //typeSelect : Add
    //type : intputfield requeireCnt

    ClearCondition[] conditions;
    Bindable<int>[] arrBindCnt;

    public ClearChecker(string stageName)
    {
        conditions = DataManager.Inst.stageData.LoadStageClearCondition(stageName);

        if(conditions == null)
        {
            Debug.LogWarning("fail load condition data file : " + stageName);
            return;
        }

        arrBindCnt = new Bindable<int>[conditions.Length];

        for (int i = 0; i < conditions.Length; i++)
        {
            arrBindCnt[i] = BindRepo.Inst.GetBindedData(
                 (N_Bind_Idx.MATCHCOUNT_BLOCKTYPE_START_IDX + conditions[i].blockType));

            arrBindCnt[i].valueChanged += OnChangeBlockCount;
        }
    }

    public void CleanUp()
    {
        for (int i = 0; i < arrBindCnt.Length; i++)
        {
            arrBindCnt[i].valueChanged -= OnChangeBlockCount;
        }
    }

    private void OnChangeBlockCount()
    {
        bool isClear = true;
        for (int i = 0; i < conditions.Length; i++)
        {
            if (arrBindCnt[i].Value >= conditions[i].requireCnt)
                isClear &= true;
            else
                isClear &= false;
        }

        if (isClear)
            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.STAGE_CLEAR);
    }

}
