/* Injektor 1.0 - (C) 2016 Premysl Fara 
 
Injektor 1.0 and newer are available under the zlib license :

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
 
 */

namespace Injektor
{
    using System;
    using System.Collections.Generic;


    #region classes

    /// <summary>
    /// An instance of this type is not registered exception.
    /// </summary>
    public class InstanceNotRegisteredException : Exception
    {
        public InstanceNotRegisteredException(string message)
            : base(message)
        {
        }
    }
     
    /// <summary>
    /// An instance of this type is already registered exception.
    /// </summary>
    public class InstanceAlreadyRegisteredException : Exception
    {
        public InstanceAlreadyRegisteredException(string message)
            : base(message)
        {
        }
    }

    #endregion


    /// <summary>
    /// Static class for remembering reusable class instances.
    /// </summary>
    public static class Registry
    {
        /// <summary>
        /// The lock object for multithreading.
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// The type-based instances dictionary.
        /// </summary>
        private static readonly Dictionary<Type, object> InstancesDictionary = new Dictionary<Type, object>();


        /// <summary>
        /// Checks, if an instance of a specific type is already registered.
        /// </summary>
        /// <typeparam name="T">A type of an instance.</typeparam>
        /// <returns>True, if an instance of a certain type is already registered.</returns>
        public static bool IsRegistered<T>() where T : class
        {
            lock (Lock)
            {
                return InstancesDictionary.ContainsKey(typeof(T));
            }
        }
        
        /// <summary>
        /// Registers an instance of a type.
        /// </summary>
        /// <typeparam name="T">A type of an instance.</typeparam>
        /// <param name="instance">An instance.</param>
        public static void RegisterInstance<T> (T instance) where T : class
        {
            lock (Lock)
            {
                var instanceType = instance.GetType();

                // Do not allow to register a type more than once.
                if (InstancesDictionary.ContainsKey(instanceType))
                {
                    throw new InstanceAlreadyRegisteredException(instanceType.FullName);
                }

                InstancesDictionary.Add(instanceType, instance);    
            }
        }

        /// <summary>
        /// Registers an instance of a type.
        /// Ex: Registry.RegisterInstance(instanceWithSomeType, typeof(IInterfaceThatIsIplementedByTheInstanceWithSomeType));
        /// </summary>
        /// <typeparam name="T">A type of an instance.</typeparam>
        /// <param name="instance">An instance.</param>
        /// <param name="asType">As which type this instance should be registered</param>
        public static void RegisterInstance<T>(T instance, Type asType) where T : class
        {
            lock (Lock)
            {
                // Do not allow to register a type more than once.
                if (InstancesDictionary.ContainsKey(asType))
                {
                    throw new InstanceAlreadyRegisteredException(asType.FullName);
                }

                InstancesDictionary.Add(asType, instance);
            }
        }

        /// <summary>
        /// Gets an instance of a specific type.
        /// </summary>
        /// <typeparam name="T">A type of an instance.</typeparam>
        /// <returns>An instance of a certain type.</returns>
        public static T Get<T>() where T : class
        {
            lock (Lock)
            {
                var instanceType = typeof(T);

                if (InstancesDictionary.ContainsKey(instanceType))
                {
                    return InstancesDictionary[instanceType] as T;
                }

                throw new InstanceNotRegisteredException(instanceType.FullName);    
            }
        }

        /// <summary>
        /// Removes an instance of a specific type.
        /// </summary>
        /// <typeparam name="T">A type of an instance.</typeparam>
        public static void Remove<T>() where T : class
        {
            lock (Lock)
            {
                var instanceType = typeof(T);

                if (InstancesDictionary.ContainsKey(instanceType))
                {
                    InstancesDictionary.Remove(instanceType);
                }

                throw new InstanceNotRegisteredException(instanceType.FullName);    
            }
        }

        /// <summary>
        /// Removes all registered instances.
        /// </summary>
        public static void RemoveAll()
        {
            lock (Lock)
            {
                InstancesDictionary.Clear();    
            }
        }
    }
}
