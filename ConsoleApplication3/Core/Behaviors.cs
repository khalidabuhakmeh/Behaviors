using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication3.Core
{
    public abstract class Behaviors
    {
        /// <summary>
        /// We could replace this with an IOC Container call
        /// </summary>
        public static Func<Type, object> TypeCreator
            = type => Activator.CreateInstance(type.GenericTypeArguments[0]);

        protected Behaviors()
        {
            Internals = GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IBehavesLike<>))
                .ToDictionary(x => x.GenericTypeArguments[0], v => TypeCreator(v));
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