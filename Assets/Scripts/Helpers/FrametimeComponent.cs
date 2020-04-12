using UnityEngine;
using System.Collections;


namespace EndlessGame.Helpers
{
    public class FrametimeComponent : MonoBehaviour
    {
        private string label = string.Empty;
        private float count = 0f;


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private IEnumerator Start()
        {
            GUI.depth = 2;

            while (true)
            {
                if (Time.timeScale > 0f)
                {
                    yield return new WaitForSeconds (0.1f);

                    count = 1f/Time.deltaTime;
                    label = string.Concat("FPS :", Mathf.Round (count));
                }
                else
                {
                    label = "PAUSE";
                }

                yield return new WaitForSeconds (0.5f);
            }
        }
        
        private void OnGUI ()
        {
            GUI.Label(new Rect(5, 40, 100, 25), label);
        }
    }
}