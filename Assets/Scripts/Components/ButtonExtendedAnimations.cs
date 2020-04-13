#pragma warning disable 0649


using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace EndlessGame.Components
{
    public enum AnimState { NORMAL, PRESSED, DISABLED }


    public class ButtonExtendedAnimations : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float duration;
        [SerializeField]
        private Ease ease;

        [Header("Sprite")]
        [SerializeField]
        private SpriteAnimation[] images;

        [Header("Color")]
        [SerializeField]
        private ColorAnimation[] colors;

        [Header("Position")]
        [SerializeField]
        private PositionAnimation[] positions;

        [Header("Scale")]
        [SerializeField]
        private ScaleAnimation[] scales;


        private AnimState m_state = AnimState.NORMAL;


        public void SetButtonState(int pButtonState)
        {
            switch (pButtonState)
            {
                case 0: m_state = AnimState.NORMAL; break;
                case 1: m_state = AnimState.NORMAL; break;
                case 2: m_state = AnimState.PRESSED; break;
                case 4: m_state = AnimState.DISABLED; break;
                default: break;
            }

            UpdateImagesSprites();
            UpdateGraphicsColors();
            UpdateTransformsPositions();
            UpdateTransformsScales();
        }

        private void UpdateImagesSprites()
        {
            if (images == null)
            {
                return;
            }

            for (int i = 0, count = images.Length; i < count; i++)
            {
                images[i].Execute(m_state);
            }
        }

        private void UpdateGraphicsColors()
        {
            if (colors == null)
            {
                return;
            }

            for (int i = 0, count = colors.Length; i < count; i++)
            {
                colors[i].Execute(m_state, duration, ease);
            }
        }

        private void UpdateTransformsPositions()
        {
            if (positions == null)
            {
                return;
            }
            
            for (int i = 0, count = positions.Length; i < count; i++)
            {
                positions[i].Execute(m_state, duration, ease);
            }
        }

        private void UpdateTransformsScales()
        {
            if (scales == null)
            {
                return;
            }
            
            for (int i = 0, count = scales.Length; i < count; i++)
            {
                scales[i].Execute(m_state, duration, ease);
            }
        }
    }

    [Serializable]
    public class SpriteAnimation
    {
        public Image image;
        public Sprite normal;
        public Sprite pressed;
        public Sprite disabled;


        public void Execute(AnimState pState)
        {
            switch (pState)
            {
                case AnimState.NORMAL: image.sprite = normal; break;
                case AnimState.PRESSED: image.sprite = pressed; break;
                case AnimState.DISABLED: image.sprite = disabled; break;
            }
        }
    }

    [Serializable]
    public class ColorAnimation
    {
        public Graphic graphic;
        public Color normal;
        public Color pressed;
        public Color disabled;


        public void Execute(AnimState pState, float pDuration, Ease pEase)
        {
            switch (pState)
            {
                case AnimState.NORMAL: graphic.DOColor(normal, pDuration).SetEase(pEase); break;
                case AnimState.PRESSED: graphic.DOColor(pressed, pDuration).SetEase(pEase); break;
                case AnimState.DISABLED: graphic.DOColor(disabled, pDuration).SetEase(pEase); break;
            }
        }
    }

    [Serializable]
    public class PositionAnimation
    {
        public RectTransform transform;
        public Vector2 normal;
        public Vector2 pressed;
        public Vector2 disabled;


        public void Execute(AnimState pState, float pDuration, Ease pEase)
        {
            switch (pState)
            {
                case AnimState.NORMAL: transform.DOAnchorPos(normal, pDuration).SetEase(pEase); break;
                case AnimState.PRESSED: transform.DOAnchorPos(pressed, pDuration).SetEase(pEase); break;
                case AnimState.DISABLED: transform.DOAnchorPos(disabled, pDuration).SetEase(pEase); break;
            }
        }
    }

    [Serializable]
    public class ScaleAnimation
    {
        public RectTransform transform;
        public Vector3 normal;
        public Vector3 pressed;
        public Vector3 disabled;


        public void Execute(AnimState pState, float pDuration, Ease pEase)
        {
            switch (pState)
            {
                case AnimState.NORMAL: transform.DOScale(normal, pDuration).SetEase(pEase); break;
                case AnimState.PRESSED: transform.DOScale(pressed, pDuration).SetEase(pEase); break;
                case AnimState.DISABLED: transform.DOScale(disabled, pDuration).SetEase(pEase); break;
            }
        }
    }
}