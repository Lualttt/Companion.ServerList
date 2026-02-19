using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using HarmonyLib;
using SteamworksNative;
using BepInEx.IL2CPP.Utils;

namespace ServerList
{
	internal static class Patches
	{
		//   Anti Bepinex detection (Thanks o7Moon: https://github.com/o7Moon/CrabGame.AntiAntiBepinex)
		[HarmonyPatch(typeof(EffectManager), nameof(EffectManager.Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0))] // Ensures effectSeed is never set to 4200069 (if it is, modding has been detected)
		[HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.Method_Private_Void_0))] // Ensures connectedToSteam stays false (true means modding has been detected)
																																										 // [HarmonyPatch(typeof(Deobf_MenuSnowSpeedModdingDetector), nameof(Deobf_MenuSnowSpeedModdingDetector.Method_Private_Void_0))] // Would ensure snowSpeed is never set to Vector3.zero (though it is immediately set back to Vector3.one due to an accident on Dani's part lol)
		[HarmonyPrefix]
		internal static bool PreBepinexDetection()
				=> false;

		[HarmonyPatch(typeof(SerevrUIPrefab), nameof(SerevrUIPrefab.SetUI))]
		[HarmonyPostfix]
		internal static void PrefabSetUI(SerevrUIPrefab __instance, CSteamID param_1, int param_2)
		{
			string titleColor = SteamMatchmaking.GetLobbyData(param_1, "LobbyNameColor");
			if (titleColor != string.Empty)
			{
				__instance.title.richText = true;
				__instance.title.text = titleColor;
			}

			string iconURL = SteamMatchmaking.GetLobbyData(param_1, "LobbyIcon");
			if (iconURL != string.Empty)
			{
				__instance.previewImg.gameObject.SetActive(false);
				__instance.StartCoroutine(PrefabSetIcon(__instance, iconURL));
			}

			string serverType = SteamMatchmaking.GetLobbyData(param_1, "ServerType");
			if (serverType != string.Empty)
				__instance.versionNumber.text = serverType + " " + __instance.versionNumber.text;
		}

		internal static IEnumerator PrefabSetIcon(SerevrUIPrefab instance, string url)
		{
			UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
			yield return request.SendWebRequest();

			if (request.result != UnityWebRequest.Result.Success)
				yield break;

			Texture texture = DownloadHandlerTexture.GetContent(request);
			instance.previewImg.texture = texture;

			instance.previewImg.gameObject.SetActive(true);
		}
	}
}
