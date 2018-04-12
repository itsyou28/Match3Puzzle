using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface iEditManager
{
    void AddMarker(Collider col);
    void RemoveMarker(Collider col);
    void SetPlayable();
    void SetNonplayable();
    void SetDirection(int dir);
    void SetCreate(bool bValue);
}

public class DummyEditManager : iEditManager
{
    public void AddMarker(Collider col) { }
    public void RemoveMarker(Collider col) { }
    public void SetPlayable() { }
    public void SetNonplayable() { }
    public void SetDirection(int dir) { }
    public void SetCreate(bool bValue) { }
}

public class EditManager : MonoBehaviour, iEditManager
{
    private static EditManager instance = null;
    private static DummyEditManager dummy = null;
    public static iEditManager i
    {
        get
        {
            if (instance == null)
            {
                if (dummy == null)
                    dummy = new DummyEditManager();

                return dummy;
            }
            return instance;
        }
    }

    BlockFieldManager fieldMng;

    LinkedList<BlockField> selectedList = new LinkedList<BlockField>();
    LinkedList<Collider> markerList = new LinkedList<Collider>();

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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OffSelect();
        }
    }

    public void AddMarker(Collider col)
    {
        markerList.AddLast(col);
        selectedList.AddLast(fieldMng.GetBlockField(
            fieldMng.RowLength - (int)col.transform.localPosition.y, (int)col.transform.localPosition.x));

    }
    public void RemoveMarker(Collider col)
    {
        markerList.Remove(col);
        selectedList.Remove(fieldMng.GetBlockField(
            fieldMng.RowLength - (int)col.transform.localPosition.y, (int)col.transform.localPosition.x));
    }
    public void OffSelect()
    {
        foreach (var collider in markerList)
        {
            collider.gameObject.SetActive(false);
        }

        markerList.Clear();
        selectedList.Clear();
    }

    public void SetPlayable()
    {
        foreach (var field in selectedList)
        {
            field.SetPlayable();
        }
    }
    public void SetNonplayable()
    {
        foreach (var field in selectedList)
        {
            field.SetNonPlayable();
        }
    }

    public void SetDirection(int dir)
    {
        foreach (var field in selectedList)
        {
            field.SetDirection(dir);
        }
    }

    public void SetCreate(bool bValue)
    {
        foreach (var field in selectedList)
        {
            field.SetCreateField(bValue);
        }
    }
}
