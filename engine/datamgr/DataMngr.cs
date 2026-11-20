using System.Collections;
using System.Linq.Expressions;
using System.Net.Security;
using System.Runtime.CompilerServices;

namespace CEF;
public interface IHasID
{    public int ID { get;  }
}

public class DtaTable<T> : IEnumerable<T> where T: class, IHasID, new()
{
    private T[] _items;
    private int _count;
    private Dictionary<int, int> _idMap = new Dictionary<int, int>(); 


    private static int GetID(T item)
    {
        if (item == null) return -1;
      
            return item.ID; 

    }

private static int currId;
    public  int NextID()
    {
        //No try catch if fails one must now, n need to throw also.
        if (_items == null) return -1;
        if (_items.Length == 0 ) 
        {
        currId = 0;
        return 0;
        }       
        if (!_idMap.ContainsKey(++currId))
        {            
            return currId; // 
        }else {
            currId = _idMap.Keys.Max()+1;
             return currId;
        }
    }

    public void FastFillMap()
    {
        for (int i = 0; i < _count; i++)
        {
            if (_items[i] != null)
            {
                int id = GetID(_items[i]);
                if (id != -1) _idMap[id] = i;
            }
        }
    }
    public void ReSyncMap()
    {
       var newMap = new Dictionary<int, int>(_count);
        for (int i = 0; i < _count; i++)
        {
            if (_items[i] != null)
            {
                int id = GetID(_items[i]);
                if (id != -1 && !newMap.ContainsKey(id))
                    newMap.Add(id, i);
            }
        }
        _idMap = newMap;
    }


    public int FindIndexByID(int id)
    {
        if (_idMap.TryGetValue(id, out int index))
        {
            if (index < _count && _items[index] != null && GetID(_items[index]) == id)
                return index;
        }
        for (int i = 0; i < _count; i++)
        {
            if (_items[i] != null && GetID(_items[i]) == id)
            {
                _idMap[id] = i; 
                return i;
            }
        }
        return -1; 
    }




    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)  
            yield return _items[i];
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public DtaTable()
    {
        _items = new T[10]; 
        for (int i = 0; i < _items.Length; i++)
        {
  
                _items[i] = null; 
            
        }
        _count = 0;
    }
    public void Clear()
    {
        _items = new T[10]; 
        for (int i = 0; i < _items.Length; i++)
        {

                _items[i] = null;
            
        }
        _count = 0;
    }
    public int Count
    {
        get
        {
            return _count;
        }
    }
    public T this[int index]
    {
        get
        {
            if (index < 0) return null;
            EnsureCapacity(index);
            if (_items[index] == null)
            {
                _items[index] =null;
                int id = GetID(_items[index]);
                if (id != -1) _idMap[id] = index;
            }
            if (index >= _count) _count = index + 1;
            return _items[index];
        }
        set
        {
            EnsureCapacity(index);
            if (_items[index] != null)
            {
                int oldId = GetID(_items[index]);
                if (oldId != -1) _idMap.Remove(oldId);
            }
            _items[index] = value;
            if (index >= _count) _count = index + 1;
            if (value != null)
            {
                int newId = GetID(value);
                if (newId != -1) _idMap[newId] = index;
            }
        }
    }
    public int CountPublic
    {
        get { return _count; }
        set { _count = value; } 
    }
    public List<T> Items
    {
        get
        {
            var list = new List<T>();
            for (int i = 0; i < _count; i++)
                if (_items[i] != null)
                    list.Add(_items[i]);
            return list;
        }
        set
        {
            _items = value.ToArray();
            _count = value?.Count ?? 0;
        }
    }
    public T RefItem(int index)
    {
        EnsureCapacity(index);
        if (index >= _count)
            _count = index + 1;
        return _items[index];
    }

    public T FindByID(int id) 
    {
        var idx = FindIndexByID(id);
        if(idx<0) return null ;

        return _items[idx];
    }


  

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count) return;
        
        if (_items[index] != null) _idMap.Remove(GetID(_items[index]));
        if (index < _count - 1)
            Array.Copy(_items, index + 1, _items, index, _count - index - 1);
        _count--;
        _items[_count] = null;
        ReSyncMap(); 
    }

    public void Remove(T item) 
    {
        var idx = FindIndexByID(item.ID);
        RemoveAt(idx);
    }

    public void Remove(int Id) 
    {
        var idx = FindIndexByID(Id);
        RemoveAt(idx);
    }

    
    public int Add(T value)
    {
       
        EnsureCapacity(_count);
        _items[_count] = value;
        if (value != null)
        {
            int id = GetID(value);
            if (id != -1) _idMap[id] = _count;
        }
        _count += 1;
        return _count - 1;
    }
    private void EnsureCapacity(int min)
    {
        if (_items is null)
        {
            _items = new T[Math.Max(9, min) + 1];
            if (typeof(T).IsClass)
            {
                for (int i = 0; i < _items.Length; i++)
                    _items[i] = null;
            }
            return;
        }
        if (min >= _items.Length)
        {
            int newCapacity = _items.Length * 2;
            if (newCapacity <= min)
                newCapacity = min + 1;
            T[] newArray = new T[newCapacity];
            Array.Copy(_items, newArray, _items.Length);
            if (typeof(T).IsClass)
            {
                for (int i = _items.Length; i < newArray.Length; i++)
                    newArray[i] = null;
            }
            _items = newArray;
        }
    }

}
