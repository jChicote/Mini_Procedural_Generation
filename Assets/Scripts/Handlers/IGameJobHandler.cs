namespace MiniProceduralGeneration.Handler
{
    public interface IGameJobHandler
    {
        object AwakeHandle(object request);
        IGameJobHandler SetNext(IGameJobHandler nextHandler);
    }
}
