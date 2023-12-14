namespace CycladeBase.Utils
{
    public class Optional<T>
    {
        public T Value;
        public bool HasValue => Value != null;

        public Optional(T value)
        {
            Value = value;
        }
    }
}