using UnityEngine;
using System.Collections;

public interface iStage
{
    void SwapBlock(BlockField selectField, BlockField targetField);
}

public class DummyStageManager : iStage
{
    public void SwapBlock(BlockField select, BlockField target)
    {
        Debug.LogWarning("Dummy Swap Block");
    }
}

public class StageManager : MonoBehaviour, iStage
{
    private static StageManager instance = null;
    private static DummyStageManager dummy = null;
    public static iStage i
    {
        get
        {
            if (instance == null)
            {
                if (dummy == null)
                    dummy = new DummyStageManager();

                return dummy;
            }
            return instance;
        }
    }

    BlockFieldManager fieldMng;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitFieldManager();
    }

    void InitFieldManager()
    {
        fieldMng = new BlockFieldManager("TestField2");
        fieldMng.BlockInitialize();
    }

    bool isMatching = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (fieldMng.FindMatch())
            {
                isMatching = true;
                fieldMng.ExcuteMatch();
            }
        }

        if (isMatching && fieldMng.IsNotEmpty())
        {
            Debug.Log("AllStop");
            isMatching = false;
            StartCoroutine(DelayMatch());
        }
    }

    IEnumerator DelayMatch()
    {
        if (fieldMng.FindMatch())
        {
            yield return new WaitForSeconds(0.2f);
            isMatching = true;
            fieldMng.ExcuteMatch();
        }
    }

    public void SwapBlock(BlockField select, BlockField target)
    {
        fieldMng.SwapBlock(select, target,
            () =>
            {
                if (fieldMng.FindMatch())
                {
                    isMatching = true;
                    fieldMng.ExcuteMatch();
                }
            });
    }
}
