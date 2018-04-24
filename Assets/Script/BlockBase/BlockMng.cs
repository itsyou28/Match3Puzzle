﻿using System;
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

    private BlockMng()
    {
        pool = new ObjectPool<iBlock>(100, 20, CreateBlock);
    }

    iBlock CreateBlock()
    {
        return new Block();
    }

    public void Push(iBlock target)
    {
        pool.Push(target);
        activeBlock.Remove(target);
    }

    public iBlock Pop()
    {
        iBlock result = pool.Pop();
        activeBlock.AddLast(result);
        return result;
    }


    public event Action AllReady;
    public bool IsAllReady { get; private set; }

    public void UpdateAllReady()
    {
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

        if (isReady && AllReady != null)
            AllReady();
    }

}