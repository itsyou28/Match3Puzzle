using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_CircleMenu : MonoBehaviour
{
    [SerializeField]
    RectTransform panel;
    [SerializeField]
    GameObject circleBtnOrigin;

    protected float radius = 150;
    protected float rInclBtnSize = 50;
    protected int maxNumber = 6;

    private void Awake()
    {
        ChildAwake();
        rInclBtnSize += radius;
        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.EDITORMODE_POINTER_DOWN, OnPointerDown, 1);
        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.EDITORMODE_POINTER_UP, OnPointerUp, 1);

        MakeCircleBtn();
        SetBtnEvent();
    }

    protected virtual void ChildAwake() { }


    void MakeCircleBtn()
    {
        float x, y;
        float angle = 6.28319f / maxNumber;

        for (int i = 0; i < maxNumber; i++)
        {
            x = Mathf.Sin(angle * i) * radius;
            y = Mathf.Cos(angle * i) * radius;

            GameObject obj = Instantiate(circleBtnOrigin);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(x, y);
        }

        GameObject offBtn = Instantiate(circleBtnOrigin);
        offBtn.transform.SetParent(transform);
        offBtn.transform.localPosition = Vector3.zero;
        offBtn.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
        offBtn.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.OffSelect(); });
        offBtn.GetComponentInChildren<Text>().text = "X";
    }

    protected virtual void SetBtnEvent()
    {
    }

    void OnPointerUp(params object[] args)
    {
        PointerEventData data = (PointerEventData)args[0];
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel, data.position, data.pressEventCamera, out pos);

        //화면 경계 부분 위치 보정 
        if (panel.rect.xMin + rInclBtnSize > pos.x)
            pos.x = panel.rect.xMin + rInclBtnSize;
        if (panel.rect.xMax - rInclBtnSize < pos.x)
            pos.x = panel.rect.xMax - rInclBtnSize;
        if (panel.rect.yMin + rInclBtnSize > pos.y)
            pos.y = panel.rect.yMin + rInclBtnSize;
        if (panel.rect.yMax - rInclBtnSize < pos.y)
            pos.y = panel.rect.yMax - rInclBtnSize;

        transform.localPosition = pos;
    }

    void OnPointerDown(params object[] args)
    {
    }
}
