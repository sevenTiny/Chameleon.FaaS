using Newtonsoft.Json;
using SevenTiny.Bantina.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Faas.Common
{
#error 这个还是独立Chameleon.Common项目统一比较好，这里引包即可
    /// <summary>
    /// Faas组件上下文
    /// </summary>
    [Serializable]
    public class FaasContext
    {
        private IDictionary<string, object> _map;

        private static readonly string _key = "Chameleon.Context.FaasContext";

        public static FaasContext Current
        {
            get
            {
                FaasContext current = (FaasContext)CallContext.GetData(FaasContext._key);
                if (current == null)
                {
                    current = new FaasContext();
                    CallContext.SetData(FaasContext._key, current);
                }
                return current;
            }
            set
            {
                CallContext.SetData(FaasContext._key, value);
            }
        }

        public FaasContext()
        {
            this._map = new Dictionary<string, object>();
        }

        internal FaasContext(IDictionary<string, object> values)
        {
            this._map = values;
        }

        public string ApplicationName
        {
            get
            {
                return this.Get("Chameleon.Context.XApplicationName");
            }
            set
            {
                this.Put("Chameleon.Context.XApplicationName", value);
            }
        }

        public string SessionId
        {
            get
            {
                return this.Get("Chameleon.Context.XSessionId");
            }
            set
            {
                this.Put("Chameleon.Context.XSessionId", value);
            }
        }

        public int TenantId
        {
            get
            {
                string value = this.Get("Chameleon.Context.XTenantId");
                if (value != null)
                {
                    return System.Convert.ToInt32(value);
                }
                return -1;
            }
            set
            {
                this.Put("Chameleon.Context.XTenantId", value);
            }
        }

        public int UserId
        {
            get
            {
                string value = this.Get("Chameleon.Context.XUserId");
                if (value != null)
                {
                    return System.Convert.ToInt32(value);
                }
                return -1;
            }
            set
            {
                this.Put("Chameleon.Context.XUserId", value);
            }
        }

        public bool HasValue
        {
            get
            {
                return this._map != null && this._map.Count > 0;
            }
        }

        internal void Put(string key, int value)
        {
            this._map[key] = value.ToString();
        }

        public void Put(string key, string value)
        {
            this._map[key] = value;
        }

        public void Put(string key, object value)
        {
            this._map[key] = JsonConvert.SerializeObject(value);
        }

        public bool Contains(string key)
        {
            return this._map.ContainsKey(key);
        }

        public string Get(string key)
        {
            object value;
            this._map.TryGetValue(key, out value);
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }

        public T Get<T>(string key)
        {
            object value;
            this._map.TryGetValue(key, out value);
            if (value != null)
            {
                return JsonConvert.DeserializeObject<T>(value.ToString());
            }
            return default(T);
        }

        public bool Remove(string key)
        {
            return this._map.Remove(key);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this._map);
        }

        public static FaasContext FromJson(string json)
        {
            IDictionary<string, object> map = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            return new FaasContext(map);
        }

        public FaasContext Clone()
        {
            return new FaasContext(this._map.ToDictionary((KeyValuePair<string, object> k) => k.Key, (KeyValuePair<string, object> k) => k.Value));
        }

        public static void Clear()
        {
            CallContext.Remove(FaasContext._key);
        }
    }
}
