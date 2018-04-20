using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class UI_EditFieldProperty : UI_CircleMenu
{
    [SerializeField]
    RectTransform panel;
    
    private void Awake()
    {
        r = 200;
        maxNumber = 8;

        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.EDITORMODE_POINTER_DOWN, OnPointerDown, 1);
        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.EDITORMODE_POINTER_UP, OnPointerUp, 1);
    }

    void OnPointerUp(params object[] args)
    {
        PointerEventData data = (PointerEventData)args[0];
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel, data.position, data.pressEventCamera, out result);
        //화면 경계 부분 위치 보정 
        transform.localPosition = result;
    }

    void OnPointerDown(params object[] args)
    {
    }

    protected override void SetBtnEvent()
    {
        Transform t;

        t = transform.GetChild(0);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetPlayable(); });
        t.GetComponentInChildren<Text>().text = "Playable";

        t = transform.GetChild(1);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetNonplayable(); });
        t.GetComponentInChildren<Text>().text = "Nonplayable";

        t = transform.GetChild(2);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetDirection(0); });
        t.GetComponentInChildren<Text>().text = "Down";
        t = transform.GetChild(3);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetDirection(1); });
        t.GetComponentInChildren<Text>().text = "Left";
        t = transform.GetChild(4);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetDirection(2); });
        t.GetComponentInChildren<Text>().text = "Up";
        t = transform.GetChild(5);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetDirection(3); });
        t.GetComponentInChildren<Text>().text = "Right";

        t = transform.GetChild(6);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetCreate(true); });
        t.GetComponentInChildren<Text>().text = "Create able";
        t = transform.GetChild(7);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetCreate(false); });
        t.GetComponentInChildren<Text>().text = "Create disable";
    }
}
