using UnityEngine;

namespace MiniProceduralGeneration.TerrainCore
{

    public interface ITargetObjectTracker
    {
        Vector3 TargetPositionInWorldSpace { get; }
    }

    public class TargetObjectTracker : MonoBehaviour, ITargetObjectTracker
    {

        #region - - - - - - Fields - - - - - -

        public Transform targetObject;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        public Vector3 TargetPositionInWorldSpace => targetObject.position;

        #endregion Properties

    }

}
