namespace AsyncFlow.Helpers;

public class DelegateProgress<T>:IProgress<T>
{
    private readonly Action<T> _action;

    public DelegateProgress(Action<T> action)
    {
        _action = action;
    }

    public void Report(T value)
    {
        _action(value);
    }
}
