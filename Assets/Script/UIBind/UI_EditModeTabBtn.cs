using UnityEngine;
using UnityEngine.UI;
using FiniteStateMachine;

public class UI_EditModeTabBtn : MonoBehaviour
{
    [SerializeField]
    GameObject editPropertyPanel;
    [SerializeField]
    GameObject clearConditionPanel;

    Bindable<int> editMode;

    private void Awake()
    {
        editMode = BindRepo.Inst.GetBindedData(N_Bind_Idx.EDIT_MODE);
        editMode.valueChanged += OnChangeEditMode;

        editMode.Value = 1;
    }

    private void OnChangeEditMode()
    {
        FSM_Layer.Inst.SetInt(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.INT_FLAG, editMode.Value);

        if (editMode.Value == 3)
        {
            clearConditionPanel.SetActive(true);
            editPropertyPanel.SetActive(false);
        }
        else
        {
            clearConditionPanel.SetActive(false);
            editPropertyPanel.SetActive(true);
        }
    }
}
