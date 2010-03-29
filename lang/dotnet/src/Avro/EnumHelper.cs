using System;
using System.Collections.Generic;
using System.Text;

namespace Avro
{
    public static class EnumHelper<T>
    {
        public static readonly T[] AllValues;
        public static readonly string[] AllKeys;
        private static readonly Type Type;

        private static readonly Dictionary<string, T> KeyLookup;
        private static readonly Dictionary<T, string> ValueLookup;

        static EnumHelper()
        {
            Type = typeof(T);

            if (!Type.IsEnum)
                throw new NotSupportedException("Type \"" + Type.FullName + "\" is not supported. Must be an enum");

            Dictionary<string, T> keyLookup=new Dictionary<string,T>();
            Dictionary<T, string> valueLookup=new Dictionary<T, string>();

            string[] allKeys = Enum.GetNames(Type);
            for (int i = 0; i < allKeys.Length; i++)
                allKeys[i] = allKeys[i].ToLower();
            AllKeys = allKeys;
            AllValues = (T[])Enum.GetValues(Type);

            foreach(string key in AllKeys)
            {
                T value = (T) Enum.Parse(Type, key, true);

                keyLookup.Add(key, value);
                
                if(!valueLookup.ContainsKey(value))
                    valueLookup.Add(value, key);
            }

            KeyLookup = keyLookup;
            ValueLookup = valueLookup;
        }


        public static T[] CreateArray(params T[] values)
        {
            return values;
        }

        static IDictionary<string, T> CreateKeyLookup(Type type, IEnumerable<T> values)
        {
            Dictionary<string, T> keyLookup = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

            foreach (T value in values)
            {
                keyLookup.Add(Enum.GetName(type, value), value);
            }

            return keyLookup;
        }

        public static IDictionary<string, T> CreateKeyLookup(IEnumerable<T> values)
        {
            return CreateKeyLookup(typeof(T), values);
        }

        public static IDictionary<string, T> CreateKeyLookup(params T[] types)
        {
            return CreateKeyLookup(typeof(T), types);
        }

        public static IDictionary<T, string> CreateValueLookup(IEnumerable<T> values)
        {
            return CreateValueLookup(typeof(T), values);
        }

        public static IDictionary<T, string> CreateValueLookup(params T[] types)
        {
            return CreateValueLookup(typeof(T), types);
        }

        static IDictionary<T, string> CreateValueLookup(Type type, IEnumerable<T> values)
        {
            Dictionary<T, string> keyLookup = new Dictionary<T, string>();

            foreach (T value in values)
            {
                keyLookup.Add(value, Enum.GetName(type, value));
            }

            return keyLookup;
        }


        public static string ToString(T t)
        {
            string s = null;

            if(ValueLookup.TryGetValue(t, out s))
            {
                return s;
            }

            //TODO: Figure out a better exception.
            throw new Exception();

        }

        public static bool TryParse(string s, out T value)
        {
            if (string.IsNullOrEmpty(s))
            {
                value = default(T);
                return false; 
            }
            return KeyLookup.TryGetValue(s, out value);
        }

        public static T Parse(string s)
        {
            T value = default(T);

            if (KeyLookup.TryGetValue(s, out value))
            {
                return value;
            }
            //TODO: Figure out a better exception.
            throw new Exception(s);
        }





        //public static string ToStringLower(SchemaType type)
        //{
        //    return type.ToString().ToLower();
        //}
    }
}
