using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(ReactionByState))]
public class Reaction_UI_Interaction : Reaction_Expand
{
    [SerializeField]
    Selectable control;

    public override void Show()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public override void Hide()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void Enable()
    {
        control.interactable = true;
    }

    public void Disable()
    {
        control.interactable = false;
    }

    public override void Excute(int _nExcuteId)
    {
        switch(_nExcuteId)
        {
            case 1:
                Enable();
                break;
            case 2:
                Disable();
                break;
        }
    }
}
