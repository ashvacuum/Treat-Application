using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firebase
{
    public class ServiceLocator
    {
        private ServiceLocator() { }
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private static ServiceLocator _instance;

        public static ServiceLocator Instance
        {
            get
            {
                _instance ??= new ServiceLocator();
                return _instance;
            }
        }

        public void RegisterService<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type.Name} already registered. Overwriting...");
            }
            _services[type] = service;
        }

        public T GetService<T>()
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type, out var service))
            {
                throw new Exception($"Service of type {type.Name} not found.");
            }
            return (T)service;
        }

        public void Reset()
        {
            _services.Clear();
        }
    }

}
