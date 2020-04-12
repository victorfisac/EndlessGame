using UnityEngine;
using System.Collections;


namespace EndlessGame.Helpers
{
    public class FrametimeComponent : MonoBehaviour
    {
        private string m_label = string.Empty;
        private float m_count = 0f;
        private GUIStyle m_style = null;


        private void Awake()
        {
            m_style = new GUIStyle();
            m_style.fontSize = 30;
            m_style.normal.textColor = Color.white;

            DontDestroyOnLoad(gameObject);
        }
        
        private IEnumerator Start()
        {
            GUI.depth = 2;

            while (true)
            {
                if (Time.timeScale > 0f)
                {
                    yield return new WaitForSeconds(0.1f);

                    m_count = 1f/Time.deltaTime;
                    m_label = Mathf.Round(m_count).ToString();
                }
                else
                {
                    m_label = "PAUSE";
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
        
        private void OnGUI()
        {
            GUI.Label(new Rect(40, 40, 100, 35), m_label, m_style);
        }
    }
}