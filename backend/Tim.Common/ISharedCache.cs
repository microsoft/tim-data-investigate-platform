// <copyright file="ISharedCache.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Common
{
    using System;

    using Newtonsoft.Json;

    public interface ISharedCache
    {
        bool TryGetStringFromCache(string key, out string value);

        void InsertOrUpdateString(string key, string value, TimeSpan timeToLive);

        void ClearValue(string key);
    }

    public static class CacheExtensions
    {
        public static bool TryGetFromCache<T>(this ISharedCache cache, string key, out T value)
        {
            return TryGetFromCache(cache, key, out value, s => JsonConvert.DeserializeObject<T>(s));
        }

        public static bool TryGetFromCache<T>(this ISharedCache cache, string key, out T value, Func<string, T> parser)
        {
            try
            {
                if (cache.TryGetStringFromCache(key, out var stringValue))
                {
                    value = parser(stringValue);
                    return true;
                }
            }
            catch (Exception)
            {
            }

            value = default;
            return false;
        }

        public static void InsertOrUpdateObject<T>(this ISharedCache cache, string key, T value, TimeSpan timeToLive)
        {
            InsertOrUpdateObject(cache, key, value, v => JsonConvert.SerializeObject(v), timeToLive);
        }

        public static void InsertOrUpdateObject<T>(this ISharedCache cache, string key, T value, Func<T, string> serializer, TimeSpan timeToLive)
        {
            var stringValue = serializer(value);
            cache.InsertOrUpdateString(key, stringValue, timeToLive);
        }

        public static T AddOrGet<T>(this ISharedCache cache, string key, Func<T> getObject, TimeSpan timeToLive)
        {
            if (cache.TryGetFromCache(key, out T value))
            {
                return value;
            }

            var val = getObject();
            InsertOrUpdateObject(cache, key, val, timeToLive);
            return val;
        }
    }
}
