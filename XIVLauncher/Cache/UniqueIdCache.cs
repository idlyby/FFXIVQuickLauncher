﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace XIVLauncher.Cache
{
    class UniqueIdCache
    {
        private const int DaysToTimeout = 3;

        private List<UniqueIdCacheEntry> _cache;

        public UniqueIdCache()
        {
            _cache = Settings.GetUniqueIdCache();
        }

        private void DeleteOldCaches()
        {
            _cache.RemoveAll(entry => (entry.TimeoutDate - DateTime.Now).TotalDays >= DaysToTimeout);
        }

        public bool HasValidCache(string userName)
        {
            return _cache.Any(entry => entry.UserName == userName && (entry.TimeoutDate - DateTime.Now).TotalDays < DaysToTimeout);
        }

        public (string Uid, int Region) GetCachedUid(string userName)
        {
            DeleteOldCaches();

            var cache = _cache.First(entry => entry.UserName == userName && (entry.TimeoutDate - DateTime.Now).TotalDays < DaysToTimeout);

            if(cache == null)
                throw new Exception("Could not find a valid cache.");

            return (cache.UniqueId, cache.Region);
        }

        public void AddCachedUid(string userName, string uid, int region)
        {
             _cache.Add(new UniqueIdCacheEntry
             {
                 TimeoutDate = DateTime.Now.AddDays(DaysToTimeout),
                 UserName = userName,
                 UniqueId = uid,
                 Region = region
             });

             Settings.SetUniqueIdCache(_cache);
             Settings.Save();
        }
    }
}