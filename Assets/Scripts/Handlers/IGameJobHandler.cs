namespace MiniProceduralGeneration.Handler
{

    public interface IGameJobHandler
    {

        #region - - - - - - Methods - - - - - -

        object AwakeHandle(object request);
        IGameJobHandler SetNext(IGameJobHandler nextHandler);

        #endregion Methods

    }

}
