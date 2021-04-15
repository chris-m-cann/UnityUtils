namespace Util
{
    [System.Serializable]
    public struct Pair<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public Pair(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 First => Item1;
        public T2 Second => Item2;

        public T1 Key => Item1;
        public T2 Value => Item2;

        public void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = Item1;
            item2 = Item2;
        }
    }
}