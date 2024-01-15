public abstract class Search
{
    public abstract Move Think (Board board, long timeLeft, long increment);
    public abstract void Reset();
}