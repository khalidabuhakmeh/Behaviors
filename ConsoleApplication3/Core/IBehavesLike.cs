namespace ConsoleApplication3.Core
{
    /// <summary>
    /// A marker class to let us know what behavior to grab
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBehavesLike<T>
        where T : class, new()
    { }
}