using System;

[Serializable]
public class Block
{
    BlockField curField;

    public int type;

    public Block()
    {

    }

    public void Match()
    {
        //화면에서 제거되고 pool로 돌아간다. 
    }

    public void Initialize(BlockField field, int type)
    {
        curField = field;
        this.type = type;
    }

    public override int GetHashCode()
    {
        return type.GetHashCode();
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

        if (type != other.type)
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
}
