using UnityEngine;
using UnityEngine.UI;

public class UI_EditBlockPropertyToggleSet : MonoBehaviour
{

    [SerializeField]
    GameObject toggleOrigin;
    [SerializeField]
    ToggleGroup group;

    Bindable<int> selectedProperty;

    Toggle[] arrToggle;

    private void Awake()
    {
        selectedProperty = BindRepo.Inst.GetBindedData(N_Bind_Idx.EDIT_SELECTED_BLOCK_PROPERTY);
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
        arrToggle = new Toggle[GlobalVal.BLOCK_PROPERTY_MAX];

        for (int i = 0; i < GlobalVal.BLOCK_PROPERTY_MAX; i++)
        {
            GameObject obj = Instantiate(toggleOrigin);
            obj.transform.SetParent(transform, false);
            Toggle toggle = obj.GetComponentInChildren<Toggle>();
            toggle.group = group;
            int idx = i;
            toggle.onValueChanged.AddListener((bValue) =>
            {
                if (bValue && selectedProperty.Value != idx)
                    selectedProperty.Value = idx;
            });

            if (i == 0)
                obj.GetComponentInChildren<Text>().text = "SetRandom";
            else
                obj.GetComponentInChildren<Text>().text = i.ToString();

            arrToggle[i] = toggle;
        }

        arrToggle[0].isOn = true;
    }
}
