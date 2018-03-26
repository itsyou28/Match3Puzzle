using UnityEngine;
using System.Collections;

public class BlockPool
{
    private static BlockPool instance = null;
    public static BlockPool Inst
    {
        get
        {
            if (instance == null)
                instance = new BlockPool();
            return instance;
        }
    }

    ObjectPool<Block> pool;

    private BlockPool()
    {
        pool = new ObjectPool<Block>(81, 10, CreateBlock);
    } 

    Block CreateBlock()
    {
        return null;
    }

    public Block Pop()
    {
        return pool.Pop();
    }

    public void Push(Block block)
    {
        pool.Push(block);
    }
}
