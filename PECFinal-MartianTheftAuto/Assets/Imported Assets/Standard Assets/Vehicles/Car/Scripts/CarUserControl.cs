using UnityEngine;
using UnityEngine.InputSystem;


namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        private float MovementInputValue;
        private float TurnInputValue;
        private float BrakeInputValue;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        // Metodo expuesto para ser invocado como Unity Event desde el PlayerInput para la accion de Move
        public void OnAccelerate(InputAction.CallbackContext context)
        {
            //Debug.Log($"Drive value {context.ReadValue<float>()}");
            MovementInputValue = context.ReadValue<float>();
        }


        // Metodo expuesto para ser invocado como Unity Event desde el PlayerInput para la accion de Turn
        public void OnTurn(InputAction.CallbackContext context)
        {
            //Debug.Log($"Turn value {context.ReadValue<float>()}");
            TurnInputValue = context.ReadValue<float>();
        }
        // Metodo expuesto para ser invocado como Unity Event desde el PlayerInput para la accion de Brake
        public void OnBrake(InputAction.CallbackContext context)
        {
            //Debug.Log($"Brake value {context.ReadValue<float>()}");
            BrakeInputValue = context.ReadValue<float>();
        }

        public void FixedUpdate()
        {
            // pass the input to the car!
            float h = TurnInputValue;
            float v = MovementInputValue;
            float handbrake = BrakeInputValue;
            //Debug.Log($"-----   {h},{v},{v},{handbrake}");
            m_Car.Move(h, v, v, handbrake);
        }
    }
}
