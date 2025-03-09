using System.Collections.Generic;

public class HashMap<TKey, TValue>
{
    private Dictionary<TKey, TValue> map;

    public HashMap()
    {
        map = new Dictionary<TKey, TValue>();
    }

    public void Put(TKey key, TValue value)
    {
        if (map.ContainsKey(key))
        {
            map[key] = value;
        }
        else
        {
            map.Add(key, value);
        }
    }

    public TValue Get(TKey key)
    {
        if (map.ContainsKey(key))
        {
            return map[key];
        }
        else
        {
            return default(TValue);
        }
    }

    public bool ContainsKey(TKey key)
    {
        return map.ContainsKey(key);
    }

    public void Remove(TKey key)
    {
        if (map.ContainsKey(key))
        {
            map.Remove(key);
        }
    }
}