

namespace InputKeyProcessor
{
    class StageKey : KeyProcessor
    {
        public sealed override void SpaceDown()
        {
            StageManager.i.Match();
        }

        public sealed override void JDown()
        {
            BlockMng.Inst.UpdateAllReady();
        }
    }
}
