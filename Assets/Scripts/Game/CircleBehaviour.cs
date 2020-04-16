#pragma warning disable 0649


using UnityEngine;


namespace EndlessGame.Game
{
    public class CircleBehaviour : MonoBehaviour
    {
        private float m_rotateSpeed;


        #if !UNITY_EDITOR
        private void Update()
        {
            if ((Input.touchCount == 0) || (Input.GetTouch(0).phase != TouchPhase.Moved))
            {
                return;
            }

            Vector2 _deltaPos = Input.GetTouch(0).deltaPosition;
            transform.Rotate(0f, 0f, -_deltaPos.x*rotateSpeed);
        }
        #else
        private void Update()
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(0f, 0f, -m_rotateSpeed);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(0f, 0f, m_rotateSpeed);
            }
        }
        #endif


        public float Speed
        {
            set { m_rotateSpeed = value; }
        }
    }
}