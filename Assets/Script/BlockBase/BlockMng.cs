using System;
using UnityEngine;
using System.Collections.Generic;


public class BlockMng : iPool<iBlock>
{
    private static BlockMng instance = null;
    public static iPool<iBlock> Pool
    {
        get
        {
            if (instance == null)
                instance = new BlockMng();
            return instance;
        }
    }
    public static BlockMng Inst
    {
        get
        {
            if (instance == null)
            {
                instance = new BlockMng();
            }

            return instance;
        }
    }

    ObjectPool<iBlock> pool;
    LinkedList<iBlock> activeBlock = new LinkedList<iBlock>();

    Bindable<bool> bindAllReady;

    const int maxTypeNum = 15;
    const int maxArrSize = maxTypeNum + 1;
    Bindable<int>[] arrBindMatchCount = new Bindable<int>[maxArrSize];

    private BlockMng()
    {
        pool = new ObjectPool<iBlock>(100, 20, CreateBlock);
        bindAllReady = BindRepo.Inst.GetBindedData(B_Bind_Idx.BLOCK_ALL_READY);

        for (int i = 0; i < maxArrSize; i++)
        {
            arrBindMatchCount[i] = BindRepo.Inst.GetBindedData(N_Bind_Idx.MATCHCOUNT_BLOCKTYPE_START_IDX + i);
        }
    }

    iBlock CreateBlock()
    {
        return new Block();
    }
    int blockSerialNum = 0;

    public void Push(iBlock target)
    {
        pool.Push(target);
        activeBlock.Remove(target);
    }

    public iBlock Pop()
    {
        iBlock result = pool.Pop();
        result.self.ID = blockSerialNum;
        blockSerialNum++;

        activeBlock.AddLast(result);
        return result;
    }


    public event Action AllReady;
    public bool IsIngame { get; set; }
    public bool IsAllReady { get; private set; }

    public void UpdateAllReady()
    {
        if (!IsIngame)
            return;

        bool isReady = true;

        foreach(var p in activeBlock)
        {
            if (p.eState != BlockState.Ready)
            {
                isReady = false;
                break;
            }
        }

        IsAllReady = isReady;
        bindAllReady.Value = isReady;

        if (isReady && AllReady != null)
            AllReady();
    }

    public void MatchCount(int blockType)
    {
        arrBindMatchCount[blockType].Value += 1;
    }

    public void ResetMatchCount()
    {
        for (int i = 0; i < maxArrSize; i++)
        {
            arrBindMatchCount[i].Value = 0;
        }
    }
}