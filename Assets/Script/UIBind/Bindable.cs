using System.Collections;
using System.Collections.Generic;
using System;

public class Bindable<T>
{
    private T value;

    public event Action valueChanged;

    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
            OnValueChange();
        }
    }

    public Bindable()
    {
    }

    public Bindable(T _value)
    {
        value = _value;
    }

    void OnValueChange()
    {
        if (valueChanged != null)
            valueChanged();
    }

    public void ValueChange()
    {
        if (valueChanged != null)
            valueChanged();
    }
}
