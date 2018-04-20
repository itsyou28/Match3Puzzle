using UnityEngine;
using System.Collections;

public class EmptyFieldMarkerPool : GameObjectPool
{
    private static EmptyFieldMarkerPool instance = null;
    public static iPool<GameObject> pool
    {
        get
        {
            if (instance == null)
            {
                if (dummypool == null)
                    dummypool = new GameObjectDummyPool();
                return dummypool;
            }

            return instance;
        }
    }

    protected override void Awake()
    {
        instance = this;
        base.Awake();
    }
}
