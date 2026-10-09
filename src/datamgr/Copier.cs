using System.Reflection;
namespace CEF;
public abstract class Copyable<T> where T : Copyable<T>, new()
{
    private static long _lastID = 0;
    protected static long NewID() => Interlocked.Increment(ref _lastID);
    public T Copy()
    {
        T instance = new();
        CloneFields(instance, changeID: false, identic: false);
        return instance;
    }
    public T Clone()
    {
        T instance = new();
        CloneFields(instance, changeID: true, identic: false);
        return instance;
    }
    public T IdenticClone()
    {
        T instance = new();
        CloneFields(instance, changeID: false, identic: true);
        return instance;
    }
    private void CloneFields(T target, bool changeID, bool identic)
    {
        var fields = this.GetType().GetFields(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        long newID = 0;
        if (changeID) newID = NewID();
        foreach (var f in fields)
        {
            var value = f.GetValue(this);
            if (!identic)
            {
                if (changeID && f.Name == "ID")
                {
                    f.SetValue(target, Convert.ChangeType(newID, f.FieldType));
                    continue;
                }
                if (changeID && f.Name == "label")
                {
                    f.SetValue(target, newID.ToString());
                    continue;
                }
            }
            f.SetValue(target, value);
        }
    }
}