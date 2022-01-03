using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicMovement
{
    public interface IMovement
    {
        void ModifyMovement(Vector3 inputValue);
    }

    public class BasicMovementTranslation : MonoBehaviour, IMovement
    {
        private Transform objectTransform;
        private Vector3 activeVelocity;
        public float speed;

        private void Awake()
        {
            objectTransform = gameObject.transform;
        }

        private void Update()
        {
            RunMovement();
        }

        private void RunMovement()
        {
            transform.position += activeVelocity;
        }

        /// <summary>
        /// Adds movement velocity from inputted direction value.
        /// </summary>
        /// <param name="inputValue"></param>
        public void ModifyMovement(Vector3 inputValue)
        {
            activeVelocity = speed * Time.deltaTime * inputValue;
        }
    }
}
