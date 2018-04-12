﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_EditFieldProperty : UI_CircleMenu
{

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

    }
}