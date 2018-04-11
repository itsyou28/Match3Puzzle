using UnityEngine;
using System.Collections;

public class FieldSelectMarkerPool : GameObjectPool
{
    private static FieldSelectMarkerPool instance = null;
    public static iPool<GameObject> pool
    {
        get
        {
            if(instance == null)
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
