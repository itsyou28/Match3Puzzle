using UnityEngine;
using System.Collections;

public class UIBind : MonoBehaviour
{
    protected const int nLogOption = (int)LogOption.UI_Binder;

    public bool isDebug = false;

    protected System.Type enumType;
    protected int UI_IDX;  

    protected virtual void Awake()
    {
    }    

    protected virtual void OnDataChange()
    {
    }

    protected virtual void OnEnable()
    {
        OnDataChange();
    }

    protected virtual void OnDestroy()
    {
    }
}

public class UIBind<T> : UIBind
{
    protected Bindable<T> bindedData;

    protected override void Awake()
    {
        if (bindedData == null)
            UDL.LogError("No binded data " + gameObject.name + " // " + System.Enum.Format(enumType, UI_IDX, "g"));

        bindedData.valueChanged += OnDataChange;

        UDL.Log(gameObject.name + " // " + System.Enum.Format(enumType, UI_IDX, "g"), nLogOption);
    }

    public Bindable<T> GetData()
    {
        return bindedData;
    }

    public void SetData(Bindable<T> data)
    {
        UDL.Log("SetData : " + System.Enum.Format(enumType, UI_IDX, "g"), nLogOption);
        bindedData = data;
        bindedData.valueChanged += OnDataChange;
    }
    protected override void OnDataChange()
    {
        UDL.LogWarning(System.Enum.Format(enumType, UI_IDX, "g") + " // bindedData.Value : " + bindedData.Value, nLogOption, isDebug);
    }

    protected override void OnDestroy()
    {
        UDL.LogWarning(System.Enum.Format(enumType, UI_IDX, "g") + " // OnDestroy!! // bindedData.Value : " + bindedData.Value, nLogOption, isDebug);
        bindedData.valueChanged -= OnDataChange;
        bindedData = null;
    }
}

public class UIBindN : UIBind<int>
{
    [SerializeField]
    N_Bind_Idx bindTarget;

    protected override void Awake()
    {
#if DEBUG_LOG
        enumType = bindTarget.GetType();
        UI_IDX = (int)bindTarget;
#endif
        bindedData = BindRepo.Inst.GetBindedData(bindTarget);

        base.Awake();
    }
}

public class UIBindF : UIBind<float>
{
    [SerializeField]
    F_Bind_Idx bindTarget;

    protected override void Awake()
    {
#if DEBUG_LOG
        enumType = bindTarget.GetType();
        UI_IDX = (int)bindTarget;
#endif
        bindedData = BindRepo.Inst.GetBindedData(bindTarget);

        base.Awake();
    }
}

public class UIBindS : UIBind<string>
{
    [SerializeField]
    S_Bind_Idx bindTarget;

    protected override void Awake()
    {
#if DEBUG_LOG
        enumType = bindTarget.GetType();
        UI_IDX = (int)bindTarget;
#endif
        bindedData = BindRepo.Inst.GetBindedData(bindTarget);

        base.Awake();
    }
}
