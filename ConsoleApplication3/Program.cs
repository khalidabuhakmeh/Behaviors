using System;
using ConsoleApplication3.Core;
using Raven.Client;
using Raven.Client.Document;

namespace ConsoleApplication3
{
    internal class Program
    {
        private static readonly IDocumentStore Store = new DocumentStore
        {
            ConnectionStringName = "RavenDB"
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
          IBehavesLike<Car>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Car
    {
        public string Model { get; set; }
        public string Vroom()
        {
            return "Vrooooooooom!";
        }
    }
}
