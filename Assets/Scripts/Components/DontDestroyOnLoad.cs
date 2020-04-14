using UnityEngine;
using System.Collections;


namespace EndlessGame.Components
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private static DontDestroyOnLoad m_instance = null;


        private void Awake()
        {
            if (m_instance != null)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            m_instance = this;
        }
    }
}