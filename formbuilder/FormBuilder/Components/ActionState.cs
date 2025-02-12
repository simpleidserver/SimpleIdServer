namespace FormBuilder.Components
{
    public class ActionState<TContent, TResult>
    {
        public Action<TResult> Callback { get; private set; }
        public TContent Content { get; private set; }

        public ActionState(Action<TResult> callback, TContent content)
        {
            Callback = callback;
            Content = content;
        }
    }
}
