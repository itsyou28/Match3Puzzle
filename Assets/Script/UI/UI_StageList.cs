using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI_StageList : MonoBehaviour
{
    [SerializeField]
    GameObject rowOrigin;

    private void Start()
    {
        InitList();
    }

    void InitList()
    {
        IEnumerator<string> itor = DataFileManager.Inst.stageDataFile.fileListItor;

        itor.Reset();

        while(itor.MoveNext())
        {
            Debug.Log(itor.Current);
        }
    }

}
