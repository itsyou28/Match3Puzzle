using UnityEngine;
using UnityEngine.UI;
public class UI_B_Toggle : UIBindB
{
    [SerializeField]
    Toggle toggle;

    [SerializeField]
    bool flip;

    protected override void OnDataChange()
    {
        base.OnDataChange();

        if (flip)
            toggle.isOn = !bindedData.Value;
        else
            toggle.isOn = bindedData.Value;
    }

    public void OnChangeToggle(bool bToggle)
    {
        if (flip)
        {
            if (bToggle != !bindedData.Value)
                bindedData.Value = !bindedData.Value;
        }
        else if(bindedData.Value != bToggle)
            bindedData.Value = bToggle;
    }
}
