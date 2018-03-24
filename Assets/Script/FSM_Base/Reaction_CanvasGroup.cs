using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ReactionByState))]
[RequireComponent(typeof(CanvasGroup))]
public class Reaction_CanvasGroup : Reaction_Expand
{
    CanvasGroup canvasGroup;

    public override void Initialize()
    {
        base.Initialize();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void Show()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        UDL.Log(gameObject.name + " Show after activeSelf : " + gameObject.activeSelf, nLogOption, bIsDebug, nLogLevel);
    }

    public override void Hide()
    {
        BroadcastMessage("OnBeforeDisable", SendMessageOptions.DontRequireReceiver);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        UDL.Log(gameObject.name + " Hide after activeSelf : " + gameObject.activeSelf, nLogOption, bIsDebug, nLogLevel);
    }


    public override void Excute(int _nExcuteId)
    {
        switch(_nExcuteId)
        {
            case 0:
                StartCoroutine(AlphaTransition(1, 0));
                break;
            case 1:
                StartCoroutine(AlphaTransition(0, 1));
                break;
        }
    }

    IEnumerator AlphaTransition(float start, float end)
    {
        float animTime = 0.3f;
        float elapseTime = 0;
        float reverseTime = 1 / animTime;

        while(elapseTime < animTime)
        {
            elapseTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapseTime * reverseTime);
            yield return true;
        }

        canvasGroup.alpha = end;

        if(end == 0)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
