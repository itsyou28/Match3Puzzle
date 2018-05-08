using UnityEngine;
using System.Collections.Generic;

public class UI_StageList : MonoBehaviour
{
    [SerializeField]
    GameObject rowOrigin;
    [SerializeField]
    Transform contentPanel;

    private void Awake()
    {
        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.CREATE_STAGE, OnCreateStage);
    }
    private void Start()
    {
        InitList();
    }

    void OnCreateStage(params object[] args)
    {
        if (args != null || args.Length > 0 || args[0].GetType() == typeof(string))
        {
            AddRow((string)args[0]).transform.SetAsFirstSibling();
        }
    }

    void InitList()
    {
        IEnumerator<string> itor = DataManager.Inst.stageData.fileListItor;

        itor.Reset();

        while(itor.MoveNext())
        {
            AddRow(itor.Current);
        }
    }

    private GameObject AddRow(string stageName)
    {
        GameObject obj = Instantiate(rowOrigin);
        obj.transform.SetParent(contentPanel, false);

        StageRowData data = new StageRowData();
        data.stageName = stageName;

        obj.GetComponent<UI_StageListRow>().SetRowData(data);

        return obj;
    }
}
