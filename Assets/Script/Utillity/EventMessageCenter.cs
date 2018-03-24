using System;
using System.Collections;
using System.Collections.Generic;

public delegate void EventCallBackFunction(params object[] args);

//Publish-Subscribe Pattern. Observer pattern
//Event Message Center
public class EMC<T>
{
    Dictionary<T, EventCallBackFunction>
        callBackFunctionList = new Dictionary<T, EventCallBackFunction>();

    Dictionary<T, EventCallBackFunction>
        callBackFunctionListBefore = new Dictionary<T, EventCallBackFunction>();

    Dictionary<T, EventCallBackFunction>
        callBackFunctionListAfter = new Dictionary<T, EventCallBackFunction>();

    /// <param name="atPoint">-1, 0, 1</param>
    public void AddEventCallBackFunction(T key, EventCallBackFunction cbFunction, int atPoint = 0)
    {
        if (atPoint < -1 || atPoint > 1)
            throw new IndexOutOfRangeException("-1 <= atPoint <=1");

        switch (atPoint)
        {
            case -1:
                AddCallBack(ref callBackFunctionListBefore, key, cbFunction);
                break;
            case 0:
                AddCallBack(ref callBackFunctionList, key, cbFunction);
                break;
            case 1:
                AddCallBack(ref callBackFunctionListAfter, key, cbFunction);
                break;
        }
    }

    public void ClearAll()
    {
        callBackFunctionList.Clear();
        callBackFunctionListAfter.Clear();
        callBackFunctionListBefore.Clear();
    }

    private void AddCallBack(ref Dictionary<T, EventCallBackFunction> target, T key, EventCallBackFunction cbFunction)
    {
        if (!target.ContainsKey(key))
            target.Add(key, cbFunction);
        else
            target[key] += cbFunction;
    }

    /// <param name="atPoint">-1, 0, 1</param>
    public void RemoveEventCallBackFunction(T key, EventCallBackFunction cbFunction, int atPoint = 0)
    {
        if (atPoint < -1 || atPoint > 1)
            throw new IndexOutOfRangeException("-1 <= atPoint <=1");

        switch (atPoint)
        {
            case -1:
                RemoveCallBack(ref callBackFunctionListBefore, key, cbFunction);
                break;
            case 0:
                RemoveCallBack(ref callBackFunctionList, key, cbFunction);
                break;
            case 1:
                RemoveCallBack(ref callBackFunctionListAfter, key, cbFunction);
                break;
        }
    }

    private void RemoveCallBack(ref Dictionary<T, EventCallBackFunction> target, T key, EventCallBackFunction cbFunction)
    {
        if (!target.ContainsKey(key))
            return;
        else
            target[key] -= cbFunction;
    }

    public void NoticeEventOccurrence(T key, params object[] args)
    {
        if (callBackFunctionListBefore.ContainsKey(key))
        {
            if (callBackFunctionListBefore[key] != null)
                callBackFunctionListBefore[key](args);
        }

        if (callBackFunctionList.ContainsKey(key))
        {
            if (callBackFunctionList[key] != null)
                callBackFunctionList[key](args);
        }

        if (callBackFunctionListAfter.ContainsKey(key))
        {
            if (callBackFunctionListAfter[key] != null)
                callBackFunctionListAfter[key](args);
        }
    }
}

public enum EMC_CODE
{
    POPUP=100, //string msg, BTN Type,
    DISP_MSG,
}

public class EMC_MAIN : EMC<EMC_CODE>
{
    private static EMC_MAIN instance;

    public static EMC_MAIN Inst
    {
        get
        {
            if (instance == null)
            {
                instance = new EMC_MAIN();
            }
            return instance;
        }
    }
}