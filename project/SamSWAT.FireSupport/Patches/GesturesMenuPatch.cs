using Comfort.Common;
using EFT;
using EFT.Airdrop;
using EFT.InputSystem;
using EFT.InventoryLogic;
using EFT.UI.Gestures;
using HarmonyLib;
using JetBrains.Annotations;
using SamSWAT.FireSupport.ArysReloaded.Unity;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZLinq;

namespace SamSWAT.FireSupport.ArysReloaded.Patches;

[UsedImplicitly]
public class GesturesMenuPatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return typeof(GesturesMenu).GetMethod(nameof(GesturesMenu.Init));
	}

	[PatchPostfix]
	private static async void PatchPostfix(GesturesMenu __instance)
	{
		try
		{
			if (!IsFireSupportAvailable())
			{
				return;
			}

			var owner = Singleton<GameWorld>.Instance.MainPlayer.GetComponent<GamePlayerOwner>();
			var fireSupportController = await FireSupportController.Create(__instance);
			Traverse.Create(owner)
				.Field<List<InputNode>>("_children")
				.Value
				.Add(fireSupportController);

			var gesturesBindPanel =
				__instance.gameObject.GetComponentInChildren<GesturesBindPanel>(includeInactive: true);
			gesturesBindPanel.transform.localPosition = new Vector3(0, -530, 0);
		}
		catch (Exception ex)
		{
			FireSupportPlugin.LogSource.LogError(ex);
		}
	}

	private static bool IsFireSupportAvailable()
	{
		if (!PluginSettings.Enabled.Value)
		{
			return false;
		}

		GameWorld gameWorld = Singleton<GameWorld>.Instance;
		if (gameWorld == null)
		{
			return false;
		}

		Player player = gameWorld.MainPlayer;
		if (player == null)
		{
			return false;
		}

		bool locationIsSuitable = player.Location.ToLower() == "sandbox" || LocationScene.GetAll<AirdropPoint>().AsValueEnumerable().Any();
		if (!locationIsSuitable)
		{
			return false;
		}

		bool hasRangefinder = player.Profile.Inventory.AllRealPlayerItems.AsValueEnumerable().Any(IsRangefinder);
		return hasRangefinder;
	}

	private static bool IsRangefinder(Item item)
	{
		return item.TemplateId == ItemConstants.RANGEFINDER_TPL;
	}
}