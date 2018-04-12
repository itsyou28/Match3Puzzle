using UnityEngine;
using System.Collections;

public interface iBlockFieldGO
{
    void SetBlockField(BlockField field);
    void ChangeFieldProperty();
    void PushBack();
}

public class BlockFieldGO : MonoBehaviour, iBlockFieldGO
{
    [SerializeField]
    GameObject dirImg;
    [SerializeField]
    GameObject mask;

    public BlockField Field { get; private set; }

    System.Action curUpdate = null;

    void Awake()
    {
        curUpdate = UpdateEditorMode;
    }

    //BlockField와 연동하여 필드에서 속성 정보를 표시한다.     
    public void SetBlockField(BlockField field)
    {
        Field = field;

        ChangeFieldProperty();
        
        transform.localPosition = new Vector3(field.X, field.Y);
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
    }

    public void PushBack()
    {
        gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        curUpdate();
    }

    void UpdateEditorMode()
    {
    }

    void UpdatePlayMode()
    {
        if (Field != null && Field.IsPlayable)
            Field.Update();
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
}