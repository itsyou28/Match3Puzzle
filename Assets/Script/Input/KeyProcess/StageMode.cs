

namespace InputKeyProcessor
{
    class StageKey : KeyProcessor
    {
        public sealed override void SpaceDown()
        {
            StageManager.i.CheckMatch();
        }
    }
}
