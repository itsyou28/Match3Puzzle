using System;
using UnityEngine;

[Serializable]
public class Block
{
    BlockField preField;
    BlockField curField;

    public int BlockType { get { return blockType; } }
    int blockType;

    public Block(BlockField field, int blockType)
    {
        curField = field;
        this.blockType = blockType;
    }

    public Block(BlockField field)
    {
        curField = field;
        blockType = UnityEngine.Random.Range(1, 8);
    }
    
    public void Match()
    {
        //화면에서 제거되고 pool로 돌아간다. 
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
