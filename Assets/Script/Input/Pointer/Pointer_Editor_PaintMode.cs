using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using FiniteStateMachine;

public class Pointer_Editor_PaintMode : iPointerInput
{
    Ray ray;
    RaycastHit hit;

    Collider buffer;

    delegate void fnPaint(Collider col);
    fnPaint dele_editMng_Paint;

    Bindable<int> editMode; //1:Field 2:Block  3:ClearCondiriton

    public Pointer_Editor_PaintMode()
    {
        editMode = BindRepo.Inst.GetBindedData(N_Bind_Idx.EDIT_MODE);
        editMode.valueChanged += OnChangeEditMode;

        OnChangeEditMode();
    }

    private void OnChangeEditMode()
    {
        if (editMode.Value == 2)
            dele_editMng_Paint = EditManager.i.PaintBlock;
        else if(editMode.Value == 1)
            dele_editMng_Paint = EditManager.i.PaintField;
    }

    private void Paint(PointerEventData data)
    {
        ray = Camera.main.ScreenPointToRay(data.position);
        if (Physics.Raycast(ray, out hit))
        {
            if (!ReferenceEquals(hit.collider, buffer))
            {
                dele_editMng_Paint(hit.collider);
                buffer = hit.collider;
            }
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        Paint(data);
    }

    public void OnPointerUp(PointerEventData data)
    {
    }

    public void OnBeginDrag(PointerEventData data)
    {
    }
    
    public void OnDrag(PointerEventData data)
    {
        Paint(data);
    }

    public void OnEndDrag(PointerEventData data)
    {
    }

    

}
