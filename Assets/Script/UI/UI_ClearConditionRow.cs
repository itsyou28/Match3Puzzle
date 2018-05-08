using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_ClearConditionRow : MonoBehaviour
{
    [SerializeField]
    Image img;
    [SerializeField]
    Text text;

    ClearCondition condition;

    Bindable<int> bindCnt;

    public void SetCondition(ClearCondition condition)
    {
        this.condition = condition;

        img.sprite = BlockGOPool.Inst.GetBlockSprite(condition.clearIdx);
        text.text = condition.requireCnt.ToString();

        bindCnt = BindRepo.Inst.GetBindedData((N_Bind_Idx.MATCHCOUNT_BLOCKTYPE_START_IDX + condition.clearIdx));
        bindCnt.valueChanged += OnChangeBlockCount;
    }

    public void OnDestroy()
    {
        bindCnt.valueChanged -= OnChangeBlockCount;
    }

    void OnChangeBlockCount()
    {
        int dispCnt = condition.requireCnt - bindCnt.Value;
        dispCnt = dispCnt < 0 ? 0 : dispCnt;
        text.text = dispCnt.ToString();
    }
}
