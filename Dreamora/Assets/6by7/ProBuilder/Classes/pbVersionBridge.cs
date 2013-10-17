/**
 *	\brief Hax!  DLLs cannot interpret preprocessor directives, so this class acts as a "bridge"
 */
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class pbVersionBridge
{
	public static int GetMajorVersion()
	{

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
		return 3;
#else
		return 4;
#endif	
	}

	public static bool IsPro()
	{
		bool isPro = false;
#if UNITY_EDITOR
		isPro = PlayerSettings.advancedLicense;
#endif
		return isPro;
	}

	public static bool StaticBatchingEnabled(GameObject go)
	{
#if UNITY_EDITOR
		return (GameObjectUtility.GetStaticEditorFlags(go) & StaticEditorFlags.BatchingStatic) == StaticEditorFlags.BatchingStatic;
#else
		return true;
#endif
	}

	public static void SetActive(GameObject go, bool isActive)
	{
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
		go.active = isActive;
#else
		go.SetActive(isActive);
#endif
	}
}

public static class GameObjectExtensions
{
	public static void SetActive(this GameObject go, bool isActive)
	{
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5	
		go.active = isActive;
#else
		go.SetActive(isActive);
#endif
	}
}