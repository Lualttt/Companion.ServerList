using UnityEngine;
using System.Collections;
using HarmonyLib;
using SteamworksNative;
using BepInEx.IL2CPP.Utils;
using System.Net.Http;
using System;
using System.Threading.Tasks;

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
			Task<HttpResponseMessage> responseTask;
			try
			{
				responseTask = ServerList.Client.GetAsync(url);
			}
			catch (Exception e)
			{
				ServerList.Instance.Log.LogWarning($"get request unsuccessful for \"{url}\" error: {e.Message}");
				yield break;
			}
			while (!responseTask.IsCompleted)
				yield return null;
			using HttpResponseMessage response = responseTask.Result;

			if (!response.IsSuccessStatusCode)
			{
				ServerList.Instance.Log.LogWarning($"get request unsuccessful for \"{url}\" code: {response.StatusCode}");
				yield break;
			}

			Task<byte[]> bytesTask;
			try
			{
				bytesTask = response.Content.ReadAsByteArrayAsync();
			}
			catch (Exception e)
			{
				ServerList.Instance.Log.LogWarning($"failed to read bytes for \"{url}\" error: {e.Message}");
				yield break;
			}
			while (!bytesTask.IsCompleted)
				yield return null;
			UnhollowerBaseLib.Il2CppStructArray<byte> bytes = new(bytesTask.Result);

			Texture2D texture = new(2, 2, TextureFormat.RGBA32, false);
			bool successful = ImageConversion.LoadImage(texture, bytes);
			if (!successful)
			{
				ServerList.Instance.Log.LogWarning($"failed to load image for \"{url}\"");
				yield break;
			}

			instance.previewImg.texture = texture;
			instance.previewImg.gameObject.SetActive(true);
		}
	}
}
