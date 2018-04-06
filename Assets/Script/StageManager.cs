using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour
{
    BlockFieldManager fieldMng;

    void Start()
    {
        InitFieldManager();
    }

    void InitFieldManager()
    {
        fieldMng = new BlockFieldManager("TestField2");
        fieldMng.BlockInitialize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fieldMng.ExcuteMatch();
        }
    }
}
