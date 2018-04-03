using UnityEngine;
using System.Collections;

public class BlockFieldDraw : MonoBehaviour
{
    ObjectPool<GameObject> fieldPool;


    GameObject CreateField()
    {
        GameObject obj = Instantiate(Resources.Load("Prefab/Field") as GameObject);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Awake()
    {
        fieldPool = new ObjectPool<GameObject>(100, 20, CreateField);
    }

    public void DrawField(BlockFieldManager fieldMng)
    {
        //현재 스테이지의 필드 배열을 읽어와서 해당 정보를 토대로 화면에 필드를 배치한다. 
        foreach(BlockField field in fieldMng.GetField())
        {
            GameObject go = fieldPool.Pop();
            BlockFieldGO control = go.GetComponent<BlockFieldGO>();

            control.SetBlockField(field);
            go.transform.localPosition = new Vector3(field.X, field.Y);
            go.SetActive(true);
        }
    }
}
