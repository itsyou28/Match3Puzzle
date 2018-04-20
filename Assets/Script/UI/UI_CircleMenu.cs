﻿using UnityEngine;
using System.Collections;

public class UI_CircleMenu : MonoBehaviour
{
    protected float r = 200;
    protected int maxNumber = 6;

    private void Start()
    {
        MakeBtn();
        SetBtnEvent();
    }

    void MakeBtn()
    {
        float x, y;
        float angle = 6.28319f / maxNumber;

        for (int i = 0; i < maxNumber; i++)
        {
            x = Mathf.Sin(angle * i) * r;
            y = Mathf.Cos(angle * i) * r;

            GameObject obj = Instantiate(Resources.Load("UIPrefab/CircleBtn") as GameObject);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(x, y);
        }
    }

    protected virtual void SetBtnEvent()
    {
    }
}
