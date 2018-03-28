using System;

[Serializable]
public class BlockField
{
    public int row, col;

    public Block block;
    public int BlockType { get { return block.BlockType; } }

    public BlockField prev { get; private set; }
    public BlockField next { get; private set; }

    public bool IsPlayable { get { return isPlayable; } }
    public bool IsEmpty { get { return isEmpty; } }
    public bool IsFirst { get { return isFirst; } }
    public bool IsLast { get { return isLast; } }


    bool isPlayable = true;
    bool isEmpty = true;
    bool isFirst = false;
    bool isLast = false;

    public void SetPlayable()
    {
        isPlayable = true;
    }

    public void SetNonPlayable()
    {
        isPlayable = false;
    }

    public void SetBlock(Block block)
    {
        if (block == null)
            isEmpty = true;
        else
            isEmpty = false;

        this.block = block;

    }

    public void Match()
    {
        block.Match();
    }

    public Block FindBlockInMyLine()
    {
        BlockField field = prev;

        while (field.IsEmpty && field.IsPlayable)
        {
            field = field.prev;
        }

        return field.block;
    }
    
    #region override Equals
    public override int GetHashCode()
    {
        return row ^ col;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is BlockField))
            return false;

        return Equals((BlockField)obj);
    }

    public bool Equals(BlockField other)
    {
        if (other == null)
            return false;

        if (row != other.row)
            return false;

        return col == other.col;
    }
    public static bool operator ==(BlockField lValue, BlockField rValue)
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

    public static bool operator !=(BlockField lValue, BlockField rValue)
    {
        return !(lValue == rValue);
    }
    #endregion
}