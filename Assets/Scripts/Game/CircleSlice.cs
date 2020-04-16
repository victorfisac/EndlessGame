#pragma warning disable 0649


using UnityEngine;
using UnityEngine.UI;


namespace EndlessGame.Game
{
    public class CircleSlice : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Image sliceImg;
        [SerializeField]
        private RectTransform rectTrans;
        [SerializeField]
        private RectTransform pivotTrans;
        [SerializeField]
        private RectTransform centerTrans;


        private Color m_sliceColor;


        public void SetData(Color pColor, float pFillAmount)
        {
            m_sliceColor = pColor;

            sliceImg.color = m_sliceColor;
            sliceImg.fillAmount = pFillAmount;

            pivotTrans.localRotation = Quaternion.Euler(0f, 0f, -pFillAmount*180f);
        }


        public Color SliceColor
        {
            get { return m_sliceColor; }
        }

        public Vector3 CenterPosition
        {
            get { return centerTrans.position; }
        }
    }
}