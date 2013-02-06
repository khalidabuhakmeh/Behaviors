using System;
using ConsoleApplication3.Core;
using Raven.Client;
using Raven.Client.Document;
using Raven.Imports.Newtonsoft.Json;

namespace ConsoleApplication3
{
    internal class Program
    {
        private static readonly IDocumentStore Store = new DocumentStore
        {
            ConnectionStringName = "RavenDB",
        }.Initialize();

        private static void Main(string[] args)
        {
            var dog = new Dog();

            dog.Like<Car>().Model = "Audi";
            dog.Like<Dog>().Name = "Billy";

            try
            {
                dog.Like<Program>().ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            using (var session = Store.OpenSession())
            {
                // We can serialize behaviors
                session.Store(dog);
                session.SaveChanges();
            }

            using (var session = Store.OpenSession())
            {
                var deez = session.Load<Dog>(dog.Id);

                // We can also deserialize behaviors
                Console.WriteLine(deez.Name);
                Console.WriteLine(deez.Like<Car>().Vroom());
                Console.WriteLine(deez.Like<Car>().Model);
            }

            Console.ReadLine();
        }
    }

    public class Dog
        : Behaviors,
          IBehavesLike<Car>,
          INamed
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    // We need this so RavenDB doesn't freak (StackOverflow exception)
    [JsonObject(IsReference = true)]
    public interface INamed
    {
        string Name { get; }
    }

    public class Car
    {
        // Should be the parent
        public INamed Named { get; set; }

        public string Model { get; set; }
        public string Vroom()
        {
            return Named != null
                ? string.Format("{0} says Vrooooooooom!", Named.Name)
                : "Vrooooooooom!";
        }
    }
}
