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

        public sealed override void TabDown()
        {
            FSM_Layer.Inst.SetBool(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.BOOL_FLAG1,
                !FSM_Layer.Inst.GetBool(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.BOOL_FLAG1));
        }
    }
}
