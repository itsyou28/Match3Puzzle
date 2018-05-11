using UnityEngine;
using UnityEngine.UI;

public class UI_EditFieldPropertyToggleSet : MonoBehaviour
{
    [SerializeField]
    GameObject toggleOrigin;
    [SerializeField]
    ToggleGroup group;

    Bindable<int> selectedProperty;

    Toggle[] arrToggle;

    private void Awake()
    {
        selectedProperty = BindRepo.Inst.GetBindedData(N_Bind_Idx.EDIT_SELECTED_FIELD_PROPERTY);
        selectedProperty.valueChanged += OnChangedSelectProperty;

        MakeToggle();
    }

    private void OnChangedSelectProperty()
    {
        if (!arrToggle[selectedProperty.Value].isOn)
            arrToggle[selectedProperty.Value].isOn = true;
    }

    void MakeToggle()
    {
        arrToggle = new Toggle[GlobalVal.FIELD_PROPERTY_MAX];

        for (int i = 0; i < GlobalVal.FIELD_PROPERTY_MAX; i++)
        {
            GameObject obj = Instantiate(toggleOrigin);
            obj.transform.SetParent(transform, false);
            Toggle toggle = obj.GetComponentInChildren<Toggle>();
            toggle.group = group;
            int idx = i;
            toggle.onValueChanged.AddListener((bValue)=>
            {
                if (bValue && selectedProperty.Value != idx)
                    selectedProperty.Value = idx;
            });

            arrToggle[i] = toggle;
        }
        
        Transform t;

        t = transform.GetChild(0);
        t.GetComponentInChildren<Text>().text = "Playable";

        t = transform.GetChild(1);
        t.GetComponentInChildren<Text>().text = "Nonplayable";

        t = transform.GetChild(2);
        t.GetComponentInChildren<Text>().text = "Down";
        t = transform.GetChild(3);
        t.GetComponentInChildren<Text>().text = "Left";
        t = transform.GetChild(4);
        t.GetComponentInChildren<Text>().text = "Up";
        t = transform.GetChild(5);
        t.GetComponentInChildren<Text>().text = "Right";

        t = transform.GetChild(6);
        t.GetComponentInChildren<Text>().text = "Create able";
        t = transform.GetChild(7);
        t.GetComponentInChildren<Text>().text = "Create disable";

        arrToggle[0].isOn = true;
    }
}
