using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleApplication3.Core
{
    public abstract class Behaviors
    {
        /// <summary>
        /// We could replace this with an IOC Container call
        /// </summary>
        public static Func<Behaviors, Type, object> TypeCreator
            = (caller, type) =>
                  {
                      var itemType = type.GenericTypeArguments[0];
                      var callerType = caller.GetType();
                      var item = Activator.CreateInstance(itemType);
                      var properties = itemType.GetProperties();

                      // I'll set the value if it is the same
                      // or the property is dynamic
                      foreach (var propertyInfo in properties)
                      {
                          if (propertyInfo.PropertyType.IsAssignableFrom(callerType))
                              propertyInfo.SetValue(item, caller);
                      }

                      return item;
                  };

        protected Behaviors()
        {
            Internals = GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IBehavesLike<>))
                .ToDictionary(x => x.GenericTypeArguments[0], v => TypeCreator(this, v));
        }

        private IDictionary<Type, object> Internals { get; set; }

        /// <summary>
        /// Helps us get the behavior we are trying to invoke
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Like<T>()
            where T : class, new()
        {
            if (typeof(T) == GetType())
                return this as T;

            object behavior;

            if (Internals.TryGetValue(typeof(T), out behavior))
                return (T)behavior;

            throw new ArgumentException(string.Format("A {0} cannot behave like a {1}", GetType().Name, typeof(T).Name));
        }
    }
}