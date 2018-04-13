using FiniteStateMachine;

namespace InputKeyProcessor
{
    public class FSM_Tester : KeyProcessor
    {
        public sealed override void EscapeDown()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_ESCAPE);
        }
        public sealed override void BackspaceDown()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_BACK);
        }

        public sealed override void Num1Down()
        {
            FSM_Layer.Inst.SetInt(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.INT_SELECT_MENU, 1);
        }
        public sealed override void Num2Down()
        {
            FSM_Layer.Inst.SetInt(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.INT_SELECT_MENU, 2);
        }
        public sealed override void Num3Down()
        {
            FSM_Layer.Inst.SetInt(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.INT_SELECT_MENU, 3);
        }
        public sealed override void Num4Down()
        {
            FSM_Layer.Inst.SetInt(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.INT_SELECT_MENU, 4);
        }


        public sealed override void Numpad1Down()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_1);
        }
        public sealed override void Numpad2Down()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_2);
        }
        public sealed override void Numpad3Down()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_3);
        }
        public sealed override void Numpad4Down()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_SELECT_4);
        }


        public sealed override void NDown()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
        }
        public sealed override void KDown()
        {
            FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_CHECK_CONDITION);
        }
        public sealed override void BDown()
        {
            FSM_Layer.Inst.SetBool(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.BOOL_FLAG1, 
                !FSM_Layer.Inst.GetBool(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.BOOL_FLAG1));
        }

    }
}