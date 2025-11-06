using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SamSWAT.FireSupport.ArysReloaded.Database;

[UsedImplicitly]
internal class AddItemToDatabasePatch : ModulePatch
{
	private static readonly string s_databasePath = Path.Combine(FireSupportPlugin.Directory, "database");

	protected override MethodBase GetTargetMethod()
	{
		Type fieldType = AccessTools.Field(typeof(ItemFactoryClass), nameof(ItemFactoryClass.ItemTemplates)).FieldType;
		return AccessTools.Method(fieldType, "Init");
	}

	[PatchPostfix]
	private static void PatchPostfix(ref Dictionary<MongoID, ItemTemplate> __instance)
	{
		var gau8Ammo = GetItemTemplate<AmmoTemplate>("ammo_30x173_gau8_avenger.json");
		AddItemTo(gau8Ammo, ref __instance);

		var gau8Weapon = GetItemTemplate<WeaponTemplate>("weapon_ge_gau8_avenger_30x173.json");
		AddItemTo(gau8Weapon, ref __instance);
	}

	private static T GetItemTemplate<T>(string jsonFileName)
	{
		string jsonPath = Path.Combine(s_databasePath, jsonFileName);
		string json = File.ReadAllText(jsonPath);
		return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettingsClass.Converters);
	}

	private static void AddItemTo(ItemTemplate itemTemplate, ref Dictionary<MongoID, ItemTemplate> dictionary)
	{
		dictionary.Add(itemTemplate._id, itemTemplate);

		MongoID? parentId = itemTemplate.ParentId;
		if (parentId.HasValue)
		{
			dictionary[parentId.Value].AddChild(itemTemplate);
		}
	}
}