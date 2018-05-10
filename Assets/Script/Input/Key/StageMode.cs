using FiniteStateMachine;

namespace InputKeyProcessor
{
    class StageKey : KeyProcessor
    {
        public sealed override void EscapeDown()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_ESCAPE);
        }

        public sealed override void SpaceDown()
        {
            StageManager.i.Match();
        }

        public sealed override void JDown()
        {
            BlockMng.Inst.UpdateAllReady();
        }

        public sealed override void LDown()
        {
        }
    }
}
