using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using FiniteStateMachine;


public class Pointer_Editor_SelectMode : iPointerInput
{
    RaycastHit[] hits;

    Bindable<Vector2> bindBeginDragPos;
    Bindable<Vector2> bindDragPos;
    Bindable<Vector2> bindEndDragPos;

    public Pointer_Editor_SelectMode()
    {
        bindBeginDragPos = BindRepo.Inst.GetBindedData(V2_Bind_Idx.POINTER_EDITOR_SELECTMODE_BEGIN_DRAG);
        bindDragPos = BindRepo.Inst.GetBindedData(V2_Bind_Idx.POINTER_EDITOR_SELECTMODE_DRAG);
        bindEndDragPos = BindRepo.Inst.GetBindedData(V2_Bind_Idx.POINTER_EDITOR_SELECTMODE_END_DRAG);
    }

    //드래그 할 때마다 사각형을 갱신
    public void OnPointerDown(PointerEventData data)
    {
    }

    Vector3 vStart, vEnd, vCenter, vHalf;
    public void OnPointerUp(PointerEventData data)
    {
        if(Input.GetMouseButton(1) || Input.GetMouseButtonUp(1))
        {
            STATE_ID curID = FSM_Layer.Inst.GetCurStateID(FSM_LAYER_ID.UserStory);

            if (curID == STATE_ID.Editor_Idle)
                FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_NEXT);
            else if (curID == STATE_ID.Editor_EditBlock || curID == STATE_ID.Editor_EditField)
                FSM_Layer.Inst.SetTrigger(FSM_LAYER_ID.UserStory, TRANS_PARAM_ID.TRIGGER_BACK);

            EMC_MAIN.Inst.NoticeEventOccurrence(EMC_CODE.EDITORMODE_POINTER_UP, data);
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        EditManager.i.OffSelect();
        bindBeginDragPos.Value = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
        bindDragPos.Value = data.position;
    }

    public void OnEndDrag(PointerEventData data)
    {
        bindEndDragPos.Value = data.position;

        vStart = bindBeginDragPos.Value;
        vStart.z = 11;
        vEnd = bindEndDragPos.Value;
        vEnd.z = 11;

        vStart = Camera.main.ScreenToWorldPoint(vStart);
        vEnd = Camera.main.ScreenToWorldPoint(vEnd);
        vCenter = vStart + ((vEnd - vStart) * 0.5f);
        vHalf.x = Mathf.Abs(vCenter.x - vStart.x);
        vHalf.y = Mathf.Abs(vCenter.y - vStart.y);
        
        hits = Physics.BoxCastAll(vCenter, vHalf, Vector3.forward);

        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                ActiveMarker(hits[i]);
            }
        }
    }

    private void ActiveMarker(RaycastHit hit)
    {
        GameObject marker = FieldSelectMarkerPool.pool.Pop();
        marker.transform.localPosition = hit.transform.localPosition;
        marker.SetActive(true);

        EditManager.i.AddMarker(marker.GetComponent<Collider>());
    }

}