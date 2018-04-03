using UnityEngine;
using System.Collections;

public class BlockDraw : MonoBehaviour
{
    ObjectPool<GameObject> blockPool;


    GameObject CreateBlock()
    {
        GameObject obj = Instantiate(Resources.Load("Prefab/Block") as GameObject);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Awake()
    {
        blockPool = new ObjectPool<GameObject>(100, 20, CreateBlock);
    }

    public void DeployBlock(BlockFieldManager fieldMng)
    {
        foreach (BlockField field in fieldMng.GetField())
        {
            GameObject go = blockPool.Pop();
            BlockGO control = go.GetComponent<BlockGO>();

            control.SetBlock(field.block);
            go.transform.localPosition = new Vector3(field.X, field.Y);
            go.SetActive(true);
        }
    }

}
