#pragma warning disable 0649


using UnityEngine;
using UnityEngine.UI;


namespace EndlessGame.Game
{
    public delegate void OnBallCollision();


    public class BallBehaviour : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float deltaAngle;
        

        [Header("References")]
        [SerializeField]
        private Image ballImg;
        [SerializeField]
        private RectTransform rectTrans;


        private float m_circleRadius = 0f;
        private float m_speed = 0f;
        private bool m_movement = false;
        private Vector2 m_direction = new Vector2(0f, 0f);
        private OnBallCollision m_onBallCollisionCallback = null;

        private const float DIRECTION_INIT_VALUE = 0.75f;


        private void Awake()
        {
            int _randomX = Random.Range(0, 1);
            int _randomY = Random.Range(0, 1);

            m_direction.x = ((_randomX == 1) ? DIRECTION_INIT_VALUE : -DIRECTION_INIT_VALUE);
            m_direction.y = ((_randomX == 1) ? DIRECTION_INIT_VALUE : -DIRECTION_INIT_VALUE);
        }


        private void Update()
        {
            if (!m_movement)
            {
                return;
            }
            
            Vector2 _position = rectTrans.anchoredPosition;
            _position += m_direction*m_speed*Time.deltaTime;
            rectTrans.anchoredPosition = _position;

            float _distance = Vector2.Distance(_position, Vector2.zero);

            if (_distance > m_circleRadius)
            {
                ChangeDirection();
                
                if (m_onBallCollisionCallback != null)
                {
                    m_onBallCollisionCallback();
                }
            }
        }

        private void ChangeDirection()
        {
            Vector2 _random = new Vector2(Random.Range(-deltaAngle, deltaAngle), Random.Range(-deltaAngle, deltaAngle));
            m_direction = _random - rectTrans.anchoredPosition;
            m_direction.Normalize();
        }


        public bool Enabled
        {
            set { gameObject.SetActive(value); }
        }

        public bool Movement
        {
            set { m_movement = value; }
        }

        public OnBallCollision OnBallCollisionCallback
        {
            get { return m_onBallCollisionCallback; }
            set { m_onBallCollisionCallback = value; }
        }

        public Color Color
        {
            get { return ballImg.color; }
            set { ballImg.color = value; }
        }

        public float LimitDistance
        {
            set { m_circleRadius = value - rectTrans.sizeDelta.x - rectTrans.sizeDelta.x/2f; }
        }

        public float Speed
        {
            set { m_speed = value; }
        }
    }
}