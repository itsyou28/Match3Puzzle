using UnityEngine;
using System.Collections;

public interface iBlockGOPool
{
    Sprite GetBlockSprite(int blockType);
}

public class BlockGOPool : MonoBehaviour, iPool<iBlockGO>
{
    private static BlockGOPool instance = null;
    private static BLockGODummyPool dummypool = null;
    public static iPool<iBlockGO> pool
    {
        get
        {
            if (instance == null)
            {
                if (dummypool == null)
                    dummypool = new BLockGODummyPool();

                return dummypool;
            }

            return instance;
        }
    }
    public static BlockGOPool Inst
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    Sprite[] arrBlockSprite;

    public Sprite GetBlockSprite(int blockIdx)
    {
        if(blockIdx < GlobalVal.BLOCKTYPE_MIN || blockIdx > GlobalVal.BLOCKTYPE_MAX)
        {
            Debug.LogError("idx range over " + blockIdx);
            return null;
        }

        return arrBlockSprite[blockIdx - 1];
    }

    ObjectPool<iBlockGO> blockPool;
    
    iBlockGO CreateBlock()
    {
        GameObject obj = Instantiate(Resources.Load("Prefab/Block") as GameObject);
        obj.transform.SetParent(transform);

        return obj.GetComponent<BlockGO>();
    }

    private void Awake()
    {
        blockPool = new ObjectPool<iBlockGO>(100, 20, CreateBlock);
        instance = this;
    }
    
    public iBlockGO Pop()
    {
        return blockPool.Pop();
    }

    public void Push(iBlockGO target)
    {
        blockPool.Push(target);
    }
}

public class BLockGODummyPool : iPool<iBlockGO>
{
    public iBlockGO Pop()
    {
        return new BlockGODummy();
    }

    public void Push(iBlockGO target)
    {
    }
}
