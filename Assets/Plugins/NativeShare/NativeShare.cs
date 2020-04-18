#pragma warning disable 0414


using UnityEngine;
using System.IO;
using System.Collections.Generic;


namespace EndlessGame.Plugins
{
	public class NativeShare
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		private static AndroidJavaClass m_ajc = null;
		private static AndroidJavaObject m_context = null;
		#endif


		private string m_title;
		private string m_subject;
		private string m_text;
		private string m_targetPackage;
		private string m_targetClass;
		private List<string> m_files;
		private List<string> m_mimes;


		public NativeShare()
		{
			m_subject = string.Empty;
			m_text = string.Empty;
			m_title = string.Empty;

			m_targetPackage = string.Empty;
			m_targetClass = string.Empty;

			m_files = new List<string>(0);
			m_mimes = new List<string>(0);
		}

		public NativeShare SetSubject(string pSubject)
		{
			if (pSubject != null)
			{
				this.m_subject = pSubject;
			}

			return this;
		}

		public NativeShare SetText(string pText)
		{
			if (pText != null)
			{
				this.m_text = pText;
			}

			return this;
		}

		public NativeShare SetTitle(string pTitle)
		{
			if (pTitle != null)
			{
				this.m_title = pTitle;
			}

			return this;
		}

		public NativeShare SetTarget(string pAndroidPackageName, string pAndroidClassName = null)
		{
			if (!string.IsNullOrEmpty(pAndroidPackageName))
			{
				m_targetPackage = pAndroidPackageName;

				if (pAndroidClassName != null)
				{
					m_targetClass = pAndroidClassName;
				}
			}

			return this;
		}

		public NativeShare AddFile(string pFilePath, string mime = null)
		{
			if (!string.IsNullOrEmpty(pFilePath) && File.Exists(pFilePath))
			{
				m_files.Add(pFilePath);
				m_mimes.Add(mime ?? string.Empty);
			}
			else
			{
				Debug.LogErrorFormat("NativeShare: file does not exist at path or permission denied: {0}.", pFilePath);
			}

			return this;
		}

		public void Share()
		{
			if ((m_files.Count == 0) && (m_subject.Length == 0) && (m_text.Length == 0))
			{
				Debug.LogWarning("NativeShare: parameters are not configured properly.");
				return;
			}

			#if UNITY_EDITOR
			Debug.Log("NativeShare: opening native shared dialog.");
			#elif UNITY_ANDROID
			AJC.CallStatic("Share", Context, m_targetPackage, m_targetClass, m_files.ToArray(), m_mimes.ToArray(), m_subject, m_text, m_title);
			#else
			Debug.Log("NativeShare: platform not supported.");
			#endif
		}

		public static bool TargetExists(string pAndroidPackageName, string pAndroidClassName = null)
		{
			#if !UNITY_EDITOR && UNITY_ANDROID
			if (string.IsNullOrEmpty(pAndroidPackageName))
			{
				return false;
			}

			if (pAndroidClassName == null)
			{
				pAndroidClassName = string.Empty;
			}

			return AJC.CallStatic<bool>("TargetExists", Context, pAndroidPackageName, pAndroidClassName);
			#else
			return true;
			#endif
		}

		public static bool FindTarget(out string pAndroidPackageName, out string pAndroidClassName, string pPackageNameRegex, string pClassNameRegex = null)
		{
			pAndroidPackageName = null;
			pAndroidClassName = null;

			#if !UNITY_EDITOR && UNITY_ANDROID
			if (string.IsNullOrEmpty(pPackageNameRegex))
			{
				return false;
			}

			if (pPackageNameRegex == null)
			{
				pPackageNameRegex = string.Empty;
			}

			string result = AJC.CallStatic<string>("FindMatchingTarget", Context, pPackageNameRegex, pClassNameRegex);
			
			if (string.IsNullOrEmpty(result))
			{
				return false;
			}

			int splitIndex = result.IndexOf('>');

			if ((splitIndex <= 0) || (splitIndex >= (result.Length - 1)))
			{
				return false;
			}

			pAndroidPackageName = result.Substring(0, splitIndex);
			pAndroidClassName = result.Substring(splitIndex + 1);

			return true;
			#else
			return false;
			#endif
		}


		#if !UNITY_EDITOR && UNITY_ANDROID
		private static AndroidJavaClass AJC
		{
			get
			{
				if (m_ajc == null)
				{
					m_ajc = new AndroidJavaClass("com.yasirkula.unity.NativeShare");
				}

				return m_ajc;
			}
		}
		
		private static AndroidJavaObject Context
		{
			get
			{
				if (m_context == null)
				{
					using (AndroidJavaObject _unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
					{
						m_context = _unityClass.GetStatic<AndroidJavaObject>("currentActivity");
					}
				}

				return m_context;
			}
		}
		#endif
	}
}