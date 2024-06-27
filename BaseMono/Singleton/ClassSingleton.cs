using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityUtil.Singleton
{
    public abstract class ClassSingleton<T> where T : class,new()
    {
        private static T instance = null;
        private static readonly object locker = new object();

        public static T Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        T t = new T();
                        instance = t;
                    }
                    return instance;
                }
            }
        }
    }
}
