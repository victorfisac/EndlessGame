#pragma warning disable 0649


using System.Collections.Generic;
using UnityEngine;


namespace EndlessGame.Game
{
    public delegate void OnScoreEvent(int pScore);


    public class GameManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private GameConfig gameConfig;

        [Header("Behaviours")]
        [SerializeField]
        private CircleBehaviour circle;
        [SerializeField]
        private BallBehaviour ball;

        [Header("Circle")]
        [SerializeField]
        private GameObject slicePrefab;
        [SerializeField]
        private RectTransform circleContainer;


        private System.Random m_random = null;
        private int m_colorsCount = 0;
        private Color[] m_currentColors = null;
        private int m_score = 0;
        private List<CircleSlice> m_instances = new List<CircleSlice>();
        private OnScoreEvent m_onBallCollisionCallback = null;
        private OnScoreEvent m_onGameplayEndCallback = null;


        private void Awake()
        {
            m_colorsCount = gameConfig.InitColors;
            ball.OnBallCollisionCallback += OnBallCollision;
            
            DefineColors(false);
            BuildCircle();

            circle.enabled = false;
            circle.Speed = gameConfig.CircleSpeed;
        }

        public void StartGame()
        {
            m_random = new System.Random((int)Time.realtimeSinceStartup);

            circle.enabled = true;

            ball.Enabled = true;
            ball.Movement = true;
            ball.LimitDistance = circleContainer.rect.width/2f;
            ball.Speed = gameConfig.BallSpeed;

            SetRandomBallColor();
        }

        public void SetPause(bool pPause)
        {
            ball.Movement = !pPause;
        }

        private void OnBallCollision()
        {
            int _closestIndex = GetClosestIndex();

            bool _sameColor = (ball.Color.Equals(gameConfig.Colors[_closestIndex]));

            if (_sameColor)
            {
                m_score++;

                IncreaseDifficulty();
                SetRandomBallColor();

                if (m_onBallCollisionCallback != null)
                {
                    m_onBallCollisionCallback(m_score);
                }
            }
            else
            {
                circle.enabled = false;
                ball.Movement = false;

                if (m_onGameplayEndCallback != null)
                {
                    m_onGameplayEndCallback(m_score);
                }
            }
        }

        private void IncreaseDifficulty()
        {
            ball.Speed = gameConfig.BallSpeed + gameConfig.BallSpeedFactor*m_score;
            
            bool _isNextLevel = ((m_score % gameConfig.ColorsFactor) == 0);
            bool _nextLevelExists = ((m_colorsCount + 1) < gameConfig.Colors.Length);

            if (_isNextLevel && _nextLevelExists)
            {
                m_colorsCount++;
                DefineColors(true);

                ClearCircle();
                BuildCircle();
            }
        }

        private void DefineColors(bool pRandomize)
        {
            m_currentColors = new Color[m_colorsCount];

            for (int i = 0; i < m_colorsCount; i++)
            {
                int _randomColor = i;

                if (pRandomize)
                {
                    _randomColor = m_random.Next(0, gameConfig.Colors.Length);
                }

                m_currentColors[i] = gameConfig.Colors[_randomColor];
            }
        }

        private void BuildCircle()
        {
            for (int i = 0; i < m_colorsCount; i++)
            {
                float _sliceLength = 360f/m_colorsCount;
                GameObject _obj = Instantiate(slicePrefab, circleContainer);

                RectTransform _rectTrans = _obj.GetComponent<RectTransform>();
                _rectTrans.anchoredPosition = Vector2.zero;
                _rectTrans.localScale = Vector3.one;
                _rectTrans.rotation = Quaternion.Euler(0f, 0f, _sliceLength*i);

                CircleSlice _circleSlice = _obj.GetComponent<CircleSlice>();
                _circleSlice.SetData(m_currentColors[i], _sliceLength/360f);
                m_instances.Add(_circleSlice);
            }
        }

        private void ClearCircle()
        {
            for (int i = 0, count = m_instances.Count; i < count; i++)
            {
                Destroy(m_instances[i].gameObject);
            }

            m_instances.Clear();
        }

        private void SetRandomBallColor()
        {
            Color _newColor = ball.Color;
            
            while (_newColor.Equals(ball.Color))
            {
                _newColor = m_currentColors[Random.Range(0, m_currentColors.Length)];
            }

            ball.Color = _newColor;
        }

        private int GetClosestIndex()
        {
            int _index = -1;
            float _minDistance = Mathf.Infinity;

            for (int i = 0, count = m_instances.Count; i < count; i++)
            {
                float _distance = Vector3.Distance(ball.transform.position, m_instances[i].CenterPosition);
                
                if (_distance < _minDistance)
                {
                    _index = i;
                    _minDistance = _distance;
                }
            }

            return _index;
        }


        public OnScoreEvent OnGameplayEndCallback
        {
            get { return m_onGameplayEndCallback; }
            set { m_onGameplayEndCallback = value; }
        }

        public OnScoreEvent OnBallCollisionCallback
        {
            get { return m_onBallCollisionCallback; }
            set { m_onBallCollisionCallback = value; }
        }
    }
}