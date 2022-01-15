using UnityEngine;

namespace MiniProceduralGeneration.Handler
{

    public abstract class GameHandler : MonoBehaviour, IGameJobHandler
    {
        #region ------ Fields ------

        private IGameJobHandler nextGameJobHandler;

        #endregion Fields

        #region ------ Methods ------

        public virtual object Handle(object request)
        {
            if (this.nextGameJobHandler != null)
            {
                return this.nextGameJobHandler.Handle(request);
            }

            return null;
        }

        public IGameJobHandler SetNext(IGameJobHandler handler)
        {
            this.nextGameJobHandler = handler;
            return handler;
        }

        #endregion Methods
    }

}
