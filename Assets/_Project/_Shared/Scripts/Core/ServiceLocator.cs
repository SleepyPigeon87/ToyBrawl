using System;
using System.Collections.Generic;
using UnityEngine;

namespace Brawler.Core
{
    /// <summary>
    /// A global registry for storing and retrieving shared services.
    /// Think of it like a phone book: register a service by type, look it up later.
    ///
    /// Usage:
    ///   Register in Awake():   ServiceLocator.Register&lt;AudioService&gt;(this);
    ///   Get in Start():        var audio = ServiceLocator.Get&lt;AudioService&gt;();
    ///   Unregister on destroy: ServiceLocator.Unregister&lt;AudioService&gt;();
    ///
    /// IMPORTANT: Cache the result in Start() - don't call Get&lt;T&gt;() every frame.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new();

        /// <summary>
        /// Register a service so other scripts can find it.
        /// Call this in Awake() of your service MonoBehaviour.
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);

            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] {type.Name} is already registered. Replacing it.");
            }

            services[type] = service;
            Debug.Log($"[ServiceLocator] Registered {type.Name}");
        }

        /// <summary>
        /// Get a registered service. Returns null if not found.
        /// Cache the result in Start() - don't call this every frame.
        /// </summary>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);

            if (services.TryGetValue(type, out var service))
            {
                return service as T;
            }

            Debug.LogError($"[ServiceLocator] {type.Name} not found! Did you forget to Register it?");
            return null;
        }

        /// <summary>
        /// Remove a service from the registry.
        /// Call this in OnDestroy() if your service can be destroyed.
        /// </summary>
        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);

            if (services.Remove(type))
            {
                Debug.Log($"[ServiceLocator] Unregistered {type.Name}");
            }
        }

        /// <summary>
        /// Check if a service is registered.
        /// Useful for optional dependencies.
        /// </summary>
        public static bool Has<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Clear all services. Call this on scene transitions if needed.
        /// </summary>
        public static void Clear()
        {
            Debug.Log($"[ServiceLocator] Clearing {services.Count} services");
            services.Clear();
        }
    }
}
