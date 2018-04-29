using UnityEngine;
using UnityEngine.UI;

public class UI_EditBlockProperty : UI_CircleMenu
{
    protected override void ChildAwake()
    {
        radius = 200;
        maxNumber = 12;
    }


    protected override void SetBtnEvent()
    {
        Transform t;

        t = transform.GetChild(0);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => 
            {
                EditManager.i.SetBlockRandom();
            });
        t.GetComponentInChildren<Text>().text = "SetRandom";

        for (int i = 1; i < maxNumber; i++)
        {
            int blockType = i;
            t = transform.GetChild(i);
            t.GetComponentInChildren<Button>().onClick.AddListener(
                () => 
                {
                    EditManager.i.SetBlockType(blockType);
                });
            t.GetComponentInChildren<Text>().text = i.ToString();
        }
    }
}