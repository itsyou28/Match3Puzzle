using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using FiniteStateMachine;

public class InputPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    EditorMode editorInput = new EditorMode();
    StageMode stageInput = new StageMode();
    iInput current;

    void Awake()
    {
        current = editorInput;

        FSM_Layer.Inst.RegisterEventChangeLayerState(FSM_LAYER_ID.UserStory, OnChangeUS);
    }

    void OnChangeUS(TRANS_ID transID, STATE_ID stateID, STATE_ID preStateID)
    {
        switch(stateID)
        {
            case STATE_ID.Main_Editor:
                current = editorInput;
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

public interface iInput
{
    void OnPointerDown(PointerEventData data);
    void OnPointerUp(PointerEventData data);
    void OnBeginDrag(PointerEventData data);
    void OnDrag(PointerEventData data);
    void OnEndDrag(PointerEventData data);
}

