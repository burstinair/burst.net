using System;
using System.Collections.Generic;

namespace Burst
{
    public class Cache<KT, VT>
    {
        internal class CacheItem
        {
            public KT key;
            public DateTime sign;
            public VT data;

            public CacheItem(KT key, DateTime sign, VT data)
            {
                this.key = key;
                this.sign = sign;
                this.data = data;
            }
        }

        private int _maxCount;
        public int MaxCount
        {
            get { return _maxCount; }
            set
            {
                _maxCount = value;
                _adjust();
            }
        }
        private int _maxMilliSeconds;
        public int MaxMilliSeconds
        {
            get { return _maxMilliSeconds; }
            set
            {
                _maxMilliSeconds = value;
                _adjust();
            }
        }

        public KT[] Keys
        {
            get
            {
                List<KT> res = new List<KT>();
                foreach (CacheItem ci in _cache)
                    res.Add(ci.key);
                return res.ToArray();
            }
        }
        public VT[] Values
        {
            get
            {
                List<VT> res = new List<VT>();
                foreach (CacheItem ci in _cache)
                    res.Add(ci.data);
                return res.ToArray();
            }
        }

        public Cache()
        {
            _maxCount = Settings.CacheMaxCount;
            _maxMilliSeconds = Settings.CacheMaxMilliSeconds;
            _dic = new Dictionary<KT, CacheItem>();
            _cache = new LinkedList<CacheItem>();
        }
        public Cache(int MaxCount, int MaxMilliSeconds)
        {
            _maxCount = MaxCount;
            _maxMilliSeconds = MaxMilliSeconds;
            _dic = new Dictionary<KT, CacheItem>();
            _cache = new LinkedList<CacheItem>();
        }

        private Dictionary<KT, CacheItem> _dic;
        private LinkedList<CacheItem> _cache;

        private bool _pass(CacheItem item)
        {
            if (item == null)
                return true;
            return (DateTime.Now - item.sign).TotalMilliseconds >= _maxMilliSeconds && !(_maxMilliSeconds == 0);
        }
        private void _adjust()
        {
            lock (this)
            {
                while (true)
                {
                    if (_cache.Count == 0 || _maxCount == 0)
                        break;
                    if (_cache.Count <= _maxCount && !_pass(_cache.First.Value))
                        break;
                    _dic.Remove(_cache.First.Value.key);
                    _cache.RemoveFirst();
                }
            }
        }

        public VT GetCache(KT key)
        {
            if (key == null)
                return default(VT);
            lock (this)
            {
                if (_dic.ContainsKey(key))
                {
                    if (_pass(_dic[key]))
                    {
                        _cache.Remove(_dic[key]);
                        _dic.Remove(key);
                        return default(VT);
                    }
                    return _dic[key].data;
                }
                else
                    return default(VT);
            }
        }
        public void SaveCache(KT key, VT data)
        {
            if (key == null)
                return;
            lock (this)
            {
                CacheItem item = new CacheItem(key, DateTime.Now, data);
                if (_dic.ContainsKey(key))
                {
                    _cache.Remove(_dic[key]);
                    _dic[key] = item;
                }
                else
                    _dic.Add(key, item);
                _cache.AddLast(item);
                _adjust();
            }
        }
        public void RemoveCache(params KT[] keys)
        {
            if (keys == null)
                return;
            lock (this)
            {
                if (keys.Length == 0)
                {
                    _cache.Clear();
                    _dic.Clear();
                }
                else
                {
                    foreach (var key in keys)
                    {
                        if (_dic.ContainsKey(key))
                        {
                            _cache.Remove(_dic[key]);
                            _dic.Remove(key);
                        }
                    }
                }
            }
        }
    }
}
