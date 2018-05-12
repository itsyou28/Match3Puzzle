using UnityEngine;
using UnityEngine.UI;

public class UI_EditBlockPropertyBtnSet : MonoBehaviour
{
    [SerializeField]
    GameObject btnOrigin;

    private void Awake()
    {
        MakeBtn();
    }

    void MakeBtn()
    {
        GameObject obj = Instantiate(btnOrigin);
        obj.transform.SetParent(transform, false);

        obj.GetComponentInChildren<Button>().onClick.AddListener(
            () =>
            {
                EditManager.i.SetBlockRandom();
            });
        obj.GetComponentInChildren<Text>().text = "SetRandom";

        for (int i = 1; i < GlobalVal.BLOCK_PROPERTY_MAX; i++)
        {
            obj = Instantiate(btnOrigin);
            obj.transform.SetParent(transform, false);

            int blockType = i;
            obj.GetComponentInChildren<Button>().onClick.AddListener(
                () =>
                {
                    EditManager.i.SetBlockType(blockType);
                });
            obj.GetComponentInChildren<Text>().text = i.ToString();
        }
    }
}
