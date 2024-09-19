using System;
using System.Collections.Generic;

public class DataParam<K> : Dictionary<K, object>, IDisposable
{
    // 파라미터 추가
    public DataParam<K> Set<V> (K key, V value)
    {
        this[key] = value;
        return this;
    }

    // 파라미터 얻기
    public bool GetParma<V> (K key, out V value)
    {
        if (this.TryGetValue (key, out object objValue) && objValue is V castValue)
        {
            value = castValue;
            return true;
        }
        value = default;
        return false;
    }

    // 파라미터 지우기
    public bool RemoveParma (K key)
    {
        return this.Remove (key);
    }

    public void Dispose ()
    {
        this.Clear ();
    }
}