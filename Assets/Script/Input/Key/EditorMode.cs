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

        Bindable<bool> pointerMode;
        public sealed override void TabDown()
        {
            if (pointerMode == null)
                pointerMode = BindRepo.Inst.GetBindedData(B_Bind_Idx.EDIT_POINTER_MODE);

            pointerMode.Value = !pointerMode.Value;

            if (!pointerMode.Value)
                EditManager.i.OffSelect();
        }

        public sealed override void VDown()
        {
            EditManager.i.Validate();
        }

        Bindable<int> editMode; //1:Field 2:Block  3:ClearCondiriton
        public sealed override void BackQuoteDown()
        {
            if (editMode == null)
                editMode = BindRepo.Inst.GetBindedData(N_Bind_Idx.EDIT_MODE);

            if (editMode.Value >= 3)
                editMode.Value = 1;
            else
                editMode.Value += 1;
        }
    }
}
