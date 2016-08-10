using System;
using System.Collections;
using System.Threading;

public class SignalEnumerator<T> : IEnumerator
{
    private bool signaled { get; set; }
    public T data { get; set; }

    public object Current
    {
        get
        {
            return null;
        }
    }

    public SignalEnumerator()
    {
        signaled = false;
    }

    public bool MoveNext()
    {
        if (signaled)
        {
            Thread.MemoryBarrier();
            return false;
        }
        return true;
    }

    public void Reset()
    {
        signaled = false;
    }
    public void Notify()
    {
        signaled = true;
        Thread.MemoryBarrier();
    }
    public void Notify(T result)
    {
        data = result;
        Notify();   
    }
}
public class AsyncExecutor<T> : SignalEnumerator<T>
{
    public AsyncExecutor(Action action)
    {
        (new Thread(() =>
        {
            action();
            Notify();
        })).Start();
    }
    public AsyncExecutor(Action<AsyncExecutor<T>> action)
    {
        (new Thread(() =>
        {
            action(this);
            Notify();
        })).Start();
    }
}
