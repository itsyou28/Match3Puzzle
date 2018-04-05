using UnityEngine;
using System.Collections;

public class BlockFieldGOPool : MonoBehaviour, iPool<iBlockFieldGO>
{
    private static BlockFieldGOPool instance = null;
    private static BlockFieldGODummyPool dummypool = null;
    public static iPool<iBlockFieldGO> pool
    {
        get
        {
            if(instance == null)
            {
                if (dummypool == null)
                    dummypool = new BlockFieldGODummyPool();

                return dummypool;
            }

            return instance;
        }
    }

    ObjectPool<iBlockFieldGO> fieldPool;
    
    iBlockFieldGO CreateField()
    {
        GameObject obj = Instantiate(Resources.Load("Prefab/Field") as GameObject);
        obj.transform.SetParent(transform);

        return obj.GetComponent<BlockFieldGO>();
    }

    private void Awake()
    {
        fieldPool = new ObjectPool<iBlockFieldGO>(100, 20, CreateField);
        instance = this;
    }

    public iBlockFieldGO Pop()
    {
        return fieldPool.Pop();
    }

    public void Push(iBlockFieldGO target)
    {
        fieldPool.Push(target);
    }
}

public class BlockFieldGODummyPool : iPool<iBlockFieldGO>
{
    public iBlockFieldGO Pop()
    {
        return new BlockFieldGODummy();
    }

    public void Push(iBlockFieldGO target)
    {
    }
}