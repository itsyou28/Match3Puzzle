using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour
{
    BlockFieldManager fieldMng;

    private void Start()
    {
        InitFieldManager();
    }

    void InitFieldManager()
    {
        fieldMng = new BlockFieldManager("TestField2");
        fieldMng.BlockInitialize();
    }
}
