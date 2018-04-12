using UnityEngine;
using System.Collections;

public class UI_CircleMenu : MonoBehaviour
{

    [SerializeField]
    float r = 200;
    [SerializeField]
    int maxNumber = 6;

    private void Awake()
    {
        MakeBtn();
        SetBtnEvent();
    }

    void MakeBtn()
    {
        float x, y;
        float angle = 6.28319f / maxNumber;

        for (int i = 0; i < maxNumber; i++)
        {
            x = Mathf.Sin(angle * i) * r;
            y = Mathf.Cos(angle * i) * r;

            GameObject obj = Instantiate(Resources.Load("Prefab/UI/CircleBtn") as GameObject);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(x, y);
        }
    }

    protected virtual void SetBtnEvent()
    {
    }
}
