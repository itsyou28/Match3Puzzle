using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaction_Expand : MonoBehaviour, iStateReaction
{
    ReactionByState owner;
    protected bool bIsDebug { get { return owner!= null ? owner.bIsDebug:false; } }

    protected const int nLogOption = 1;
    protected const int nLogLevel = 8;
    protected const int nLogWarningLevel = 7;
    protected const int nLogErrorLevel = 6;

    public virtual void Initialize()
    {
        owner = GetComponent<ReactionByState>();
        owner.SetReaction(this);
    }

    public virtual void Show() { }
    public virtual void Hide() { }
    public virtual void Excute(int _nExcuteId) { }
    public virtual void Change() { }
}
