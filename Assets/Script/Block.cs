using System;
using UnityEngine;

public class BlockPool
{
    private static BlockPool instance = null;
    private static ObjectPool<Block> pool;
    public static ObjectPool<Block> Pool
    {
        get
        {
            if (instance == null)
                instance = new BlockPool();
            return pool;
        }
    }

    private BlockPool()
    {
        pool = new ObjectPool<Block>(100, 20, CreateBlock);
    }

    Block CreateBlock()
    {
        return new Block();
    }
}


/// <summary>
/// 게임 시간동안 빈번하게 생성/해제가 발생하므로 pool로 관리한다. 
/// 블럭 타입 관리 및 블럭타입 비교를 구현한다. 
/// 블럭이 위치하고 있는 필드, 이동할 필드 등을 관리한다. 
/// </summary>
[Serializable]
public class Block
{
    BlockField preField;
    BlockField curField;

    [NonSerialized]
    iBlockGO blockGO;

    public int BlockType { get { return blockType; } }
    int blockType;

    public void InitByEditor(BlockField field, int blockType)
    {
        curField = field;
        this.blockType = blockType;
    }

    public void Reset(BlockField field, int blockType)
    {
        curField = field;
        this.blockType = blockType;

        DeployScreen();
    }

    public void ResetRand(BlockField field, int randMax)
    {
        curField = field;
        blockType = UnityEngine.Random.Range(1, randMax);

        DeployScreen();
    }

    public void SetBlockType(int blockType)
    {
        this.blockType = blockType;
    }

    public void SetNextField()
    {
        BlockField field = curField;

        while (field.next.IsEmpty && field.next.IsPlayable)
        {
            field = field.next;
        }

        preField = curField;
        curField.SetBlock(null);
        curField = field;
        curField.SetBlock(this);
    }

    public void MoveToNextField()
    {
        //preField에서 curField로의 이동애니메이션을 실행한다. 
        //병합구간에서상호 간섭 없이 애니를 하려면?
    }

    public void DeployScreen()
    {
        if(blockGO == null)
            blockGO = BlockGOPool.pool.Pop();
        blockGO.SetBlock(this, curField.X, curField.Y);
    }

    public void Match()
    {
        //화면에서 제거되고 pool로 돌아간다. 
        blockGO.Match();
        BlockGOPool.pool.Push(blockGO);
        blockGO = null;
    }

    public void CleanUp()
    {
        blockGO.PushBack();
        BlockGOPool.pool.Push(blockGO);
        blockGO = null;
    }

    #region override Equals
    public override int GetHashCode()
    {
        return BlockType.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is Block))
            return false;

        return Equals((Block)obj);
    }

    public bool Equals(Block other)
    {
        if (other == null)
            return false;

        if (BlockType != other.BlockType)
            return false;

        return true;
    }
    public static bool operator ==(Block lValue, Block rValue)
    {
        // Check for null on left side.
        if (ReferenceEquals(lValue, null))
        {
            if (ReferenceEquals(rValue, null))
            {
                // null == null = true.
                return true;
            }

            // Only the left side is null.
            return false;
        }

        return lValue.Equals(rValue);
    }

    public static bool operator !=(Block lValue, Block rValue)
    {
        return !(lValue == rValue);
    } 
    #endregion
}
