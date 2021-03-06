﻿using UnityEngine;
using System.Collections;

public class UI_Switch_EditPropertyPanel : MonoBehaviour
{

    Bindable<int> editMode; //1:Field 2:Block  3:ClearCondiriton
    Bindable<bool> pointerMode;//true:Select, false:Paint

    [SerializeField]
    GameObject[] arrPanels;


    private void Awake()
    {
        editMode = BindRepo.Inst.GetBindedData(N_Bind_Idx.EDIT_MODE);
        pointerMode = BindRepo.Inst.GetBindedData(B_Bind_Idx.EDIT_POINTER_MODE);

        editMode.valueChanged += OnChangeEditMode;
        pointerMode.valueChanged += OnChangeEditPointerMode;

        pointerMode.Value = true;
    }

    private void OnChangeEditMode()
    {
        SwitchPanel();
    }

    private void OnChangeEditPointerMode()
    {
        SwitchPanel();
    }

    private void SwitchPanel()
    {
        for (int i = 0; i < arrPanels.Length; i++)
        {
            arrPanels[i].SetActive(false);
        }

        if (editMode.Value == 2 && pointerMode.Value)
        {
            //Block Select
            arrPanels[0].SetActive(true);
        }
        else if (editMode.Value == 1 && pointerMode.Value)
        {
            //Field Select
            arrPanels[1].SetActive(true);
        }
        else if (editMode.Value == 2 && !pointerMode.Value)
        {
            //Block Paint
            arrPanels[2].SetActive(true);
        }
        else if (editMode.Value == 1 && !pointerMode.Value)
        {
            //FIeld Paint
            arrPanels[3].SetActive(true);
        }
    }
}
