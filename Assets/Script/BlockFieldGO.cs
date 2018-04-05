using UnityEngine;
using System.Collections;

public interface iBlockFieldGO
{
    void SetBlockField(BlockField field);
    void PushBack();
}

public class BlockFieldGO : MonoBehaviour, iBlockFieldGO
{
    //배경 이미지
    //진행방향
    //기타 등등?
    [SerializeField]
    GameObject dirImg;

    //BlockField와 연동하여 필드에서 속성 정보를 표시한다. 

    public void SetBlockField(BlockField field)
    {
        switch(field.Direction)
        {
            case 0://down
                dirImg.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1://left
                dirImg.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case 2://up
                dirImg.transform.localRotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3://right
                dirImg.transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        transform.localPosition = new Vector3(field.X, field.Y);
        gameObject.SetActive(true);
    }

    public void PushBack()
    {
        gameObject.SetActive(false);
    }
}

public class BlockFieldGODummy : iBlockFieldGO
{
    public void SetBlockField(BlockField field)
    {
    }
    public void PushBack()
    {
    }
}