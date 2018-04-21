using FiniteStateMachine;

namespace InputKeyProcessor 
{
    class EditorKey : KeyProcessor
    {
        public sealed override void EscapeDown()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_ESCAPE);
        }

        public sealed override void SpaceDown()
        {
            EditManager.i.OffSelect();
        }

        Bindable<bool> editMode;
        public sealed override void TabDown()
        {
            if (editMode == null)
                editMode = BindRepo.Inst.GetBindedData(B_Bind_Idx.EDIT_MODE);

            editMode.Value = !editMode.Value;
        }
    }
}
