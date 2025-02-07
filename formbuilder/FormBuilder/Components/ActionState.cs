namespace FormBuilder.Components;

public class ActionState
{
    private readonly Action _act;

    public ActionState(Action act)
    {
        _act = act;
    }

    public void Finish() => _act();
}
