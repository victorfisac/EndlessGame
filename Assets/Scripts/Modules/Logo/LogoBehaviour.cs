﻿using UnityEngine;
using UnityEngine.UI;


namespace EndlessGame.Modules.Logo
{
    public class LogoBehaviour : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float circleSpeed;
        [SerializeField]
        private float ballSpeed;
        [SerializeField]
        private float circleRadius;
        [SerializeField]
        private float deltaAngle;

        [Header("Colors")]
        [SerializeField]
        private Color[] ballColors;

        [Header("References")]
        [SerializeField]
        private Image m_ballImg;
        [SerializeField]
        private RectTransform ballTrans;
        [SerializeField]
        private RectTransform circleTrans;


        private Vector2 m_direction = new Vector2(1f, 1f);


        private void Update()
        {
            UpdateLimitRotation();
            UpdateBallPosition();
        }

        private void UpdateLimitRotation()
        {
            Vector3 _rotation = circleTrans.rotation.eulerAngles;
            _rotation.z = Time.realtimeSinceStartup*circleSpeed;
            circleTrans.rotation = Quaternion.Euler(_rotation);
        }

        private void UpdateBallPosition()
        {
            Vector2 _position = ballTrans.anchoredPosition;
            _position += m_direction*ballSpeed*Time.deltaTime;
            ballTrans.anchoredPosition = _position;

            float _distance = Vector2.Distance(_position, Vector2.zero);

            if (_distance > circleRadius)
            {
                ChangeDirection();
                UpdateBallColor();
            }
        }

        private void ChangeDirection()
        {
            Vector2 _random = new Vector2(Random.Range(-deltaAngle, deltaAngle), Random.Range(-deltaAngle, deltaAngle));
            m_direction = _random - ballTrans.anchoredPosition;
            m_direction.Normalize();
        }

        private void UpdateBallColor()
        {
            int _closestIndex = GetClosestChild(circleTrans, ballTrans);
            m_ballImg.color = ballColors[_closestIndex];
        }

        private int GetClosestChild(Transform pParent, Transform pTransform)
        {
            int _result = -1;
            float _minDistance = Mathf.Infinity;

            for (int i = 0, count = pParent.childCount; i < count; i++)
            {
                float _distance = Vector3.Distance(pParent.GetChild(i).position, pTransform.position);

                if (_distance < _minDistance)
                {
                    _minDistance = _distance;
                    _result = i;
                }
            }

            return _result;
        } 
    }
}