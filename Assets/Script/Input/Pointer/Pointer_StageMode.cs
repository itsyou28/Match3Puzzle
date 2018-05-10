using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


public class Pointer_StageMode : iPointerInput
{
    Ray ray;
    RaycastHit hit;

    BlockFieldGO selectField, targetField;

    public void OnPointerDown(PointerEventData data)
    {
        ray = Camera.main.ScreenPointToRay(data.position);
        if (Physics.Raycast(ray, out hit))
        {
            selectField = hit.collider.GetComponent<BlockFieldGO>();
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        ray = Camera.main.ScreenPointToRay(data.position);
        if (Physics.Raycast(ray, out hit))
        {
            targetField = hit.collider.GetComponent<BlockFieldGO>();

            StageManager.i.SwapBlock(selectField.Field, targetField.Field);
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
    }

    public void OnDrag(PointerEventData data)
    {
    }

    public void OnEndDrag(PointerEventData data)
    {
    }
}