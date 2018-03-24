using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using FiniteStateMachine;
using System;

public class PopupWindow : MonoBehaviour
{
    [SerializeField]
    Text title;
    [SerializeField]
    Text msg;

    [SerializeField]
    GameObject[] arrBtn;//0:OK, 1:Yes, 2:No

    bool popupResult = false;
    Action<bool> callback;

    private void Awake()
    {
        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.POPUP, OnPopup);
        gameObject.SetActive(false);
    }
    
    /// <param name="args">0:titleText, 1:contentText, 2:btnType, 3:callback</param>
    void OnPopup(params object[] args)
    {
        if (args == null || args.Length < 2)
        {
            UDL.LogError("No Title Msg. No content Msg");
            return;
        }

        if (args[0].GetType() != typeof(string))
        {
            UDL.LogError("0 argument must string");
            return;
        }
        if (args[1].GetType() != typeof(string))
        {
            UDL.LogError("1 argument must string");
            return;
        }

        title.text = (string)args[0];
        msg.text = (string)args[1];

        int btnType = 0;
        if (args.Length >= 3)
        {
            if (args[2].GetType() != typeof(int))
            {
                UDL.LogError("2 argument must int");
                return;
            }

            btnType = (int)args[2];

            if (btnType < 0 || btnType > 1)
            {
                UDL.LogError("2 argument range 0 < btnType < 2");
                return;
            }

            SetBtnType(btnType);

            if (args.Length > 3)
            {
                if (args[3].GetType() != typeof(Action<bool>))
                {
                    UDL.LogError("3 argument must Action<bool>");
                    return;
                }

                callback = (Action<bool>)args[3];
            }
        }

        gameObject.SetActive(true);
    }

    private void SetBtnType(int btnType)
    {
        for (int i = 0; i < arrBtn.Length; i++)
        {
            arrBtn[i].gameObject.SetActive(false);
        }
        switch (btnType)
        {
            case 0:
                arrBtn[0].gameObject.SetActive(true);
                break;
            case 1:
                arrBtn[1].gameObject.SetActive(true);
                arrBtn[2].gameObject.SetActive(true);
                break;
        }
    }

    private void OnEnable()
    {
        FSM_Layer.Inst.Pause(FSM_LAYER_ID.UserStory);
    }

    private void OnDisable()
    {
        FSM_Layer.Inst.Resume(FSM_LAYER_ID.UserStory);

        if (callback != null)
            callback(popupResult);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    public void ClickOK()
    {
        popupResult = true;
        gameObject.SetActive(false);
    }

    public void ClickYes()
    {
        popupResult = true;
        gameObject.SetActive(false);
    }

    public void ClickNo()
    {
        popupResult = false;
        gameObject.SetActive(false);
    }

    public void ClickBackground()
    {
        popupResult = false;
        gameObject.SetActive(false);
    }
}
