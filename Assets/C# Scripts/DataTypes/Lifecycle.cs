

public abstract class Lifecycle
{
    protected Lifecycle()
    {
        OnStart();
    }

    ~Lifecycle()
    {
        OnDestroy();
    }

    /// <summary>
    /// Called when the class is created
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// Called when the class is de-constructed (GC collected, UNRELIABLE timing)
    /// </summary>
    protected virtual void OnDestroy() { }
}