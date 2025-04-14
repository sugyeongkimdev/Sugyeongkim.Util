using System;
using System.Collections.Generic;
using System.Linq;

// 삽입 순서를 유지하는 딕셔너리
public class OrderedDictionary<TKey, TValue>
{
    private readonly Dictionary<TKey, TValue> _dict = new ();  // 키-값 저장
    private readonly List<TKey> _keys = new ();                // 삽입 순서 기록

    public int Count => _keys.Count;

    public void Add (TKey key, TValue value)
    {
        if (_dict.ContainsKey (key))
            throw new ArgumentException ("이미 존재하는 키입니다: " + key);

        _dict[key] = value;
        _keys.Add (key);
    }

    // 이미 있는 키면 false, 없으면 추가하고 true
    public bool TryAdd (TKey key, TValue value)
    {
        if (_dict.ContainsKey (key))
            return false;

        _dict[key] = value;
        _keys.Add (key);
        return true;
    }

    public bool Remove (TKey key)
    {
        if (!_dict.ContainsKey (key)) return false;

        _dict.Remove (key);
        _keys.Remove (key);
        return true;
    }

    public void Clear ()
    {
        _dict.Clear ();
        _keys.Clear ();
    }

    public bool ContainsKey (TKey key) => _dict.ContainsKey (key);

    public bool TryGetValue (TKey key, out TValue value) => _dict.TryGetValue (key, out value);

    public TValue this[TKey key]
    {
        get => _dict[key];
        set
        {
            if (!_dict.ContainsKey (key))
                _keys.Add (key);
            _dict[key] = value;
        }
    }

    public KeyValuePair<TKey, TValue> First ()
    {
        if (_keys.Count == 0)
            throw new InvalidOperationException ("비어있음");
        var key = _keys[0];
        return new KeyValuePair<TKey, TValue> (key, _dict[key]);
    }

    public KeyValuePair<TKey, TValue> Last ()
    {
        if (_keys.Count == 0)
            throw new InvalidOperationException ("비어있음");
        var key = _keys[^1];
        return new KeyValuePair<TKey, TValue> (key, _dict[key]);
    }

    public TKey FirstKey => _keys.Count > 0 ? _keys[0] : throw new InvalidOperationException ("비어있음");
    public TValue FirstValue => _dict[FirstKey];

    public TKey LastKey => _keys.Count > 0 ? _keys[^1] : throw new InvalidOperationException ("비어있음");
    public TValue LastValue => _dict[LastKey];

    public bool Any () => _keys.Count > 0;

    public IEnumerable<TKey> Keys => _keys;
    public IEnumerable<TValue> Values => _keys.Select (k => _dict[k]);
    public IEnumerable<KeyValuePair<TKey, TValue>> Items => _keys.Select (k => new KeyValuePair<TKey, TValue> (k, _dict[k]));
}
