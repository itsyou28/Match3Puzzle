using UnityEngine;
using UnityEngine.UI;
using FiniteStateMachine;

public class UI_EditModeTabBtn : MonoBehaviour
{
    Bindable<bool> editMode;

    private void Awake()
    {
        editMode = BindRepo.Inst.GetBindedData(B_Bind_Idx.EDIT_MODE);
        editMode.valueChanged += OnChangeEditMode;
    }

    private void OnChangeEditMode()
    {
        FSM_Layer.Inst.SetBool(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.BOOL_FLAG1, editMode.Value);
    }
}
