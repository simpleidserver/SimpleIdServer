namespace FormBuilder.Components;

public class ActionState
{
    private readonly Action _act;

    public ActionState(Action act)
    {
        _act = act;
    }

    public ActionState(Action act, string value) : this(act)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public void Finish() => _act();
}
