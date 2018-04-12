using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InputPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    EditorMode editorInput = new EditorMode();
    StageMode stageInput = new StageMode();
    iInput current;

    void Awake()
    {
        current = editorInput;
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

