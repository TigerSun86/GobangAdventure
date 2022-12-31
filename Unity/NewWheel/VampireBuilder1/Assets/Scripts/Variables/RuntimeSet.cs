using System.Collections.Generic;

public abstract class RuntimeSet<T> : VariableBase
{
    public List<T> Items = new List<T>();

    public void Add(T thing)
    {
        if (!Items.Contains(thing))
        {
            Items.Add(thing);
        }
    }

    public void Remove(T thing)
    {
        if (Items.Contains(thing))
            Items.Remove(thing);
    }

    public void NotifyChanged()
    {
        this.variableChangeEvent.Raise();
    }
}
