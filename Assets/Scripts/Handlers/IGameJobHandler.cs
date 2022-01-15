namespace MiniProceduralGeneration.Handler
{
    public interface IGameJobHandler
    {
        object Handle(object request);
        IGameJobHandler SetNext(IGameJobHandler nextHandler);
    }
}
