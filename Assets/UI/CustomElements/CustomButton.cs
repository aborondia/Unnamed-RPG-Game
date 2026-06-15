using System;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomButton : Button
{
    private Action<string> onClickAction;
    private string onClickValueSetter;

    public void InitializeButton(Action<string> onClickAction)
    {
        this.onClickAction = onClickAction;
        this.RegisterCallback<ClickEvent>(evt => this.onClickAction?.Invoke(this.onClickValueSetter));
    }

    public void ChangeOnClickReturnValue(string value)
    {
        this.onClickValueSetter = value;
    }

    public void ClearOnClickReturnValue()
    {
        this.onClickValueSetter = String.Empty;
    }
}