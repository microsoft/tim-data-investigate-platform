// <copyright file="InMemoryCache.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Common
{
    using System;
    using System.Collections.Concurrent;

    public class InMemoryCache : ISharedCache
    {
        private readonly ConcurrentDictionary<string, Tuple<DateTime, string>> m_cache = new();

        public void ClearValue(string key)
        {
            m_cache.TryRemove(key, out _);
        }

        public void InsertOrUpdateString(string key, string value, TimeSpan timeToLive)
        {
            m_cache.AddOrUpdate(key, s => new Tuple<DateTime, string>(DateTime.UtcNow.Add(timeToLive), value), (s, t) => new Tuple<DateTime, string>(DateTime.UtcNow.Add(timeToLive), value));
        }

        public bool TryGetStringFromCache(string key, out string value)
        {
            if (m_cache.TryGetValue(key, out var existing))
            {
                if (existing.Item1 >= DateTime.UtcNow)
                {
                    value = existing.Item2;
                    return true;
                }
                else
                {
                    ClearValue(key);
                }
            }

            value = null;
            return false;
        }
    }
}
