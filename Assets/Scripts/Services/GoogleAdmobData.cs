using UnityEngine;


namespace EndlessGame.Services
{
    [CreateAssetMenu(fileName = "NewGoogleAdmobData", menuName = "EndlessGame/Create Google Admob data")]
    public class GoogleAdmobData : ScriptableObject
    {
        public string bannerId;
        public string intersticialId;
        public string[] testingDevices;
    }
}