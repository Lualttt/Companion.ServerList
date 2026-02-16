using HarmonyLib;
using SteamworksNative;

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
		}
	}
}
