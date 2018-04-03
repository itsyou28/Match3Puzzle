using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    BlockFieldDraw fieldDraw;
    [SerializeField]
    BlockDraw blockDraw;

    BlockFieldManager fieldMng;

    private void Start()
    {
        InitFieldManager();
    }

    void InitFieldManager()
    {
        fieldMng = new BlockFieldManager("TestField");
        fieldMng.BlockInitialize();

        fieldDraw.DrawField(fieldMng);
        blockDraw.DeployBlock(fieldMng);
    }
}
