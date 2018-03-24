using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool<T>
{
    //특정오브젝트를 Stack에 담을 수 있다. 
    //Stack에 원하는 개수만큼 추가할 수 있다. 
    //Pop요청시 오브젝트가 Pool에 없다면 추가로 생성한다. 
    //사용이 끝난 후 Push할 수 있다. 

    short initialCount;
    short allocateUnit;

    public delegate T Func();
    Func m_deleCreate_fn;

    Stack<T> m_stackObjectPool;

    public ObjectPool(short initialCount, short allocateUnit, Func createFn)
    {
        this.initialCount = initialCount;
        m_deleCreate_fn = createFn;
        this.allocateUnit = allocateUnit;

        m_stackObjectPool = new Stack<T>(this.initialCount);

        Allocate(initialCount);
    }

    void Allocate(short max)
    {
        for (int i = 0; i < max; ++i)
        {
            m_stackObjectPool.Push(m_deleCreate_fn());
        }
    }

    public T Pop()
    {
        if (m_stackObjectPool.Count <= 0)
        {
            Allocate(allocateUnit);
        }

        return m_stackObjectPool.Pop();
    }

    public void Push(T obj)
    {
        m_stackObjectPool.Push(obj);
    }

    public virtual void Destroy()
    {
        m_deleCreate_fn = null;

        m_stackObjectPool.Clear();
    }

    public int Count { get { return m_stackObjectPool.Count; } }
}