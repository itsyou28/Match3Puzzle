using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using FiniteStateMachine;

public class EditorMode : iInput
{
    Ray ray;
    RaycastHit hit;

    Collider buffer;

    public void OnPointerDown(PointerEventData data)
    {
        ray = Camera.main.ScreenPointToRay(data.position);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("SelectMarker"))
            {
                DeactiveMarker();
            }
            else
            {
                ActiveMarker();
            }
        }
        EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.EDITORMODE_POINTER_DOWN, data);
    }

    public void OnPointerUp(PointerEventData data)
    {
        EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.EDITORMODE_POINTER_UP, data);
    }

    public void OnBeginDrag(PointerEventData data)
    {
    }
    
    public void OnDrag(PointerEventData data)
    {
        ray = Camera.main.ScreenPointToRay(data.position);
        if (Physics.Raycast(ray, out hit))
        {
            if (!ReferenceEquals(hit.collider, buffer))
            {
                if (hit.collider.CompareTag("SelectMarker"))
                {
                    DeactiveMarker();
                }
                else
                {
                    ActiveMarker();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
    }

    private void DeactiveMarker()
    {
        hit.collider.gameObject.SetActive(false);
        FieldSelectMarkerPool.pool.Push(hit.collider.gameObject);

        EditManager.i.RemoveMarker(hit.collider);

        if (Physics.Raycast(ray, out hit))
            buffer = hit.collider;
    }

    private void ActiveMarker()
    {
        GameObject marker = FieldSelectMarkerPool.pool.Pop();
        marker.transform.localPosition = hit.transform.localPosition;
        marker.SetActive(true);
        buffer = marker.GetComponent<Collider>();

        EditManager.i.AddMarker(buffer);
    }

}