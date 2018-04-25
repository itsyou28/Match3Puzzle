using UnityEngine;
using System.Collections;

public class DeadlineMarkerPool : GameObjectPool
{
    private static DeadlineMarkerPool instance = null;
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
