using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BasicMovement.Input
{
    public class InputController : MonoBehaviour
    {
        private IMovement movementComponent;

        private void Awake()
        {
            movementComponent = this.GetComponent<IMovement>();

        }

        /// <summary>
        /// Processes input value before passing movement component for processing.
        /// </summary>
        /// <param name="value"></param>
        private void OnMovement(InputValue value)
        {
            movementComponent.ModifyMovement(new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y));
        }
    }
}
