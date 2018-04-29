using UnityEngine;
using System.Collections;

public interface iBlockFieldGO
{
    void SetBlockField(BlockField field);
    void ChangeFieldProperty();
    void PushBack();
    void Active(bool bMode);
}

public class BlockFieldGO : MonoBehaviour, iBlockFieldGO
{
    [SerializeField]
    GameObject dirImg;
    [SerializeField]
    GameObject mask;
    [SerializeField]
    bool isDebug = false;

    public BlockField Field { get; private set; }

    System.Action curUpdate = null;

    void Awake()
    {
        curUpdate = UpdateEmpty;
    }

    public void Active(bool isEditMode)
    {
        if (isEditMode)
        {
            dirImg.GetComponent<SpriteRenderer>().color = Color.red;
            curUpdate = UpdateEditorMode;
        }
        else
        {
            dirImg.GetComponent<SpriteRenderer>().color = Color.blue;
            curUpdate = UpdatePlayMode;
        }
    }

    //BlockField와 연동하여 필드에서 속성 정보를 표시한다.     
    public void SetBlockField(BlockField field)
    {
        Field = field;
        transform.localPosition = new Vector3(field.X, field.Y);
        ChangeFieldProperty();
        gameObject.SetActive(true);
    }

    public void ChangeFieldProperty()
    {
        mask.SetActive(!Field.IsPlayable);

        switch (Field.Direction)
        {
            case 0://down
                dirImg.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1://left
                dirImg.transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
            case 2://up
                dirImg.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3://right
                dirImg.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
        }

        DispEmptyMarker();
        DispCreateMarker();
        DispDeadlinMarker();
    }

    public void PushBack()
    {
        PushbackCreateMarker();
        PushbackEmptyMarker();
        PushbackDeadlineMarker();

        curUpdate = UpdateEmpty;
        gameObject.SetActive(false);
    }

    GameObject emptyMarker;
    GameObject createMarker;
    GameObject deadlineMarker;

    void FixedUpdate()
    {
        curUpdate();

        if (isDebug)
            Debug.Log(Field.IsEmpty);

    }

    private void DispEmptyMarker()
    {
        if (Field != null)
        {
            if (Field.IsEmpty && emptyMarker == null)
            {
                emptyMarker = EmptyFieldMarkerPool.pool.Pop();
                emptyMarker.transform.localPosition = transform.localPosition;
                emptyMarker.SetActive(true);
            }
            else if (!Field.IsEmpty)
            {
                PushbackEmptyMarker();
            }
        }
    }

    private void PushbackEmptyMarker()
    {
        if (emptyMarker != null)
        {
            emptyMarker.SetActive(false);
            EmptyFieldMarkerPool.pool.Push(emptyMarker);
            emptyMarker = null; 
        }
    }

    void DispCreateMarker()
    {
        if (Field != null)
        {
            if (Field.IsCreateField && createMarker == null)
            {
                createMarker = CreateFieldMarkerPool.pool.Pop();
                createMarker.transform.localPosition = transform.localPosition;
                createMarker.SetActive(true);
            }
            else if (!Field.IsCreateField)
            {
                PushbackCreateMarker();
            }
        }

    }

    private void PushbackCreateMarker()
    {
        if (createMarker != null)
        {
            createMarker.SetActive(false);
            CreateFieldMarkerPool.pool.Push(createMarker);
            createMarker = null;
        }
    }

    void DispDeadlinMarker()
    {
        if (Field != null)
        {
            if (Field.IsDeadline && deadlineMarker == null)
            {
                deadlineMarker = DeadlineMarkerPool.pool.Pop();
                deadlineMarker.transform.localPosition = transform.localPosition;
                deadlineMarker.SetActive(true);
            }
            else if (!Field.IsDeadline)
            {
                PushbackDeadlineMarker();
            }
        }
    }
    private void PushbackDeadlineMarker()
    {
        if (deadlineMarker != null)
        {
            deadlineMarker.SetActive(false);
            DeadlineMarkerPool.pool.Push(deadlineMarker);
            deadlineMarker = null;
        }
    }


    void UpdateEditorMode()
    {
        DispEmptyMarker();
    }

    void UpdatePlayMode()
    {
        if (Field != null && Field.IsPlayable)
            Field.Update();

        DispEmptyMarker();
    }

    void UpdateEmpty()
    {
    }
}

public class BlockFieldGODummy : iBlockFieldGO
{
    public void SetBlockField(BlockField field)
    {
    }
    public void ChangeFieldProperty()
    {
    }
    public void PushBack()
    {
    }

    public void Active(bool bMode)
    {
    }
}