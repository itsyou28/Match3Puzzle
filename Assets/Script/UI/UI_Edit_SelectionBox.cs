using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Edit_SelectionBox : MonoBehaviour
{
    [SerializeField]
    RectTransform canvas;
    [SerializeField]
    RectTransform rect;
    
    Vector2 leftTop, rightBottom, vSize;

    Bindable<Vector2> bindBeginDragPos;
    Bindable<Vector2> bindDragPos;
    Bindable<Vector2> bindEndDragPos;

    private void Awake()
    {
        bindBeginDragPos = BindRepo.Inst.GetBindedData(V2_Bind_Idx.POINTER_EDITOR_SELECTMODE_BEGIN_DRAG);
        bindDragPos = BindRepo.Inst.GetBindedData(V2_Bind_Idx.POINTER_EDITOR_SELECTMODE_DRAG);
        bindEndDragPos = BindRepo.Inst.GetBindedData(V2_Bind_Idx.POINTER_EDITOR_SELECTMODE_END_DRAG);

        bindDragPos.valueChanged += OnDrag;
        bindEndDragPos.valueChanged += OnEndDrag;
    }

    private void OnEndDrag()
    {
        rect.sizeDelta = Vector2.zero;
    }

    private void OnDrag()
    {
        SetBoxShape(bindBeginDragPos.Value, bindDragPos.Value);
    }


    void SetBoxShape(Vector2 vDownPos, Vector2 vDragPos)
    {
        vDownPos.x /= canvas.localScale.x;
        vDownPos.y /= canvas.localScale.y;
        vDownPos.y -= canvas.sizeDelta.y;
        
        vDragPos.x /= canvas.localScale.x;
        vDragPos.y /= canvas.localScale.y;
        vDragPos.y -= canvas.sizeDelta.y;

        if (vDownPos.x < vDragPos.x)
        {
            leftTop.x = vDownPos.x;
            rightBottom.x = vDragPos.x;
        }
        else
        {
            leftTop.x = vDragPos.x;
            rightBottom.x = vDownPos.x;
        }

        if (vDownPos.y > vDragPos.y)
        {
            leftTop.y = vDownPos.y;
            rightBottom.y = vDragPos.y;
        }
        else
        {
            leftTop.y = vDragPos.y;
            rightBottom.y = vDownPos.y;
        }

        vSize = rightBottom - leftTop;
        vSize.y = Mathf.Abs(vSize.y);

        rect.anchoredPosition = leftTop;
        rect.sizeDelta = vSize;
    }

}
