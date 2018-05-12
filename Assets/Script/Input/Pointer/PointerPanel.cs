using UnityEngine;
using UnityEngine.EventSystems;
using FiniteStateMachine;

public class PointerPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    iPointerInput editorSelectInput = new Pointer_Editor_SelectMode();
    iPointerInput editorPaintInput = new Pointer_Editor_PaintMode();
    iPointerInput stageInput = new Pointer_StageMode();
    iPointerInput current;

    Bindable<bool> pointerMode;//true:Select, false:Paint;

    void Awake()
    {
        current = editorSelectInput;

        pointerMode = BindRepo.Inst.GetBindedData(B_Bind_Idx.EDIT_POINTER_MODE);
        pointerMode.valueChanged += OnChangeEditPointerMode;

        FSM_Layer.Inst.RegisterEventChangeLayerState(FSM_LAYER_ID.UserStory, OnChangeUS);
    }

    private void OnChangeEditPointerMode()
    {
        SwitchEditorInputMode();
    }

    private void SwitchEditorInputMode()
    {
        if (pointerMode.Value)
            current = editorSelectInput;
        else
            current = editorPaintInput;
    }

    void OnChangeUS(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        switch(stateID)
        {
            case STATE_ID.Main_Editor:
                SwitchEditorInputMode();
                break;
            case STATE_ID.Main_Stage:
                current = stageInput;
                break;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        current.OnPointerDown(data);
    }

    public void OnPointerUp(PointerEventData data)
    {
        current.OnPointerUp(data);
    }

    public void OnBeginDrag(PointerEventData data)
    {
        current.OnBeginDrag(data);
    }

    public void OnDrag(PointerEventData data)
    {
        current.OnDrag(data);
    }

    public void OnEndDrag(PointerEventData data)
    {
        current.OnEndDrag(data);
    }
}

public interface iPointerInput
{
    void OnPointerDown(PointerEventData data);
    void OnPointerUp(PointerEventData data);
    void OnBeginDrag(PointerEventData data);
    void OnDrag(PointerEventData data);
    void OnEndDrag(PointerEventData data);
}

