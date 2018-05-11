using UnityEngine;
using UnityEngine.UI;

public class UI_N_TabToggle : UIBindN
{
    [SerializeField]
    Toggle toggle;

    [SerializeField]
    int idx;

    protected override void OnDataChange()
    {
        base.OnDataChange();

        if (bindedData.Value == idx && !toggle.isOn)
            toggle.isOn = true;
        else if(bindedData.Value != idx && toggle.isOn)
            toggle.isOn = false;
    }

    public void OnChangeToggle(bool bToggle)
    {
        if (bToggle && bindedData.Value != idx)
            bindedData.Value = idx;
    }
}
