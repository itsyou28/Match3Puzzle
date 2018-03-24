using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MakeUILoadList : MonoBehaviour
{
    [MenuItem("Tools/MakeUIList")]
    static void MakeList()
    {
        Debug.LogWarning("MakeList");

        string[] arrFileList = FileManager.Inst.GetFileList("Resources/UIPrefab/LoadAtStart");
        List<string> uiPrefabList = new List<string>();
        string[] buffer;

        for (int i = 0; i < arrFileList.Length; i++)
        {
            buffer = arrFileList[i].Split('\\');
            buffer = buffer[buffer.Length-1].Split('.');
            
            if (buffer[buffer.Length-1] == "meta")
                continue;

            uiPrefabList.Add(buffer[0]);

            Debug.Log(buffer[0]);
        }

        FileManager.Inst.FileSave("Resources/UIPrefab/", "uilist.bytes", uiPrefabList);

        Debug.LogWarning("Complete Make UI Load List");
    }
}
