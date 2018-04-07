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

    bool isMoving = false;

    public void MoveToNextField()
    {
        //이동중에 호출 받았을 때 next필드가 변경되거나 하지 않도록 블럭해야 한다. 
        if (isMoving)
            return;

        if (curField.next.IsPlayable && curField.next.IsEmpty)
        {
            curField.SetBlock(null);
            curField = curField.next;
            curField.SetBlock(this);

            blockGO.Move(curField.X, curField.Y, Move);

            isMoving = true;
        }
    }

    void Move()
    {
        if (curField.next.IsPlayable && curField.next.IsEmpty)
        {
            curField.SetBlock(null);
            curField = curField.next;
            curField.SetBlock(this);

            Debug.Log(this.GetHashCode() + " Move1  " + curField.X + "  " + curField.Y);
            blockGO.Move(curField.X, curField.Y, Move);
        }
        else
        {
            Debug.Log(this.GetHashCode() + " Move2  " + curField.X + "  " + curField.Y);
            isMoving = false;
            blockGO.Stop();
        }
    }

    public void DeployScreen()
    {
        if(blockGO == null)
            blockGO = BlockGOPool.pool.Pop();
        blockGO.SetBlock(this, curField.X, curField.Y);
    }

    public void Match()
    {
        if (blockGO != null)
        {
            //화면에서 제거되고 pool로 돌아간다. 
            blockGO.Match();
            BlockGOPool.pool.Push(blockGO);
            blockGO = null; 
        }
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
