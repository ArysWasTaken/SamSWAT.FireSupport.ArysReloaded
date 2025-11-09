using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using SPTarkov.Server.Core.Utils.Cloners;
using System.Reflection;

namespace SamSWAT.FireSupport.ArysReloaded;

[Injectable]
public class CustomItemServiceExtended(
	ISptLogger<CustomItemService> logger,
	ConfigServer configServer,
	DatabaseService databaseService,
	ItemHelper itemHelper,
	ItemBaseClassService itemBaseClassService,
	ModItemCacheService modItemCacheService,
	ICloner cloner)
	: CustomItemService(logger, databaseService, itemHelper, itemBaseClassService, modItemCacheService, cloner)
{
	private readonly ItemConfig _itemConfig = configServer.GetConfig<ItemConfig>();
	private readonly TraderConfig _traderConfig = configServer.GetConfig<TraderConfig>();
	private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();
	private readonly ScavCaseConfig _scavCaseConfig = configServer.GetConfig<ScavCaseConfig>();
	
	private static readonly MethodInfo s_addModItem =
		typeof(ModItemCacheService).GetMethod("AddModItem", BindingFlags.NonPublic | BindingFlags.Instance)!;

	// Thanks to SPT Devs for the code, I've borrowed the CustomItemService logic and modified it for my own use.
	public void CreateItem(NewCustomItemDetails newItemDetails)
	{
		DatabaseTables tables = databaseService.GetTables();

		TemplateItem newItem = newItemDetails.NewItem!;

		// Fail if itemId already exists
		if (tables.Templates.Items.TryGetValue(newItem.Id, out TemplateItem? item))
		{
			logger.Warning($"ItemId already exists. {item.Name}");
			return;
		}

		AddToItemsDb(newItem.Id, newItem);

		AddToLocaleDbs(newItemDetails.Locales!, newItem.Id);
		
		AddToBlacklists(newItemDetails.BlacklistDetails, newItem.Id);

		itemBaseClassService.HydrateItemBaseClassCache();

		s_addModItem.Invoke(modItemCacheService, [Assembly.GetExecutingAssembly(), newItem.Id]);
	}

	private void AddToBlacklists(BlacklistDetails? blacklistDetails, MongoId newItemId)
	{
		if (blacklistDetails == null) return;
		
		if (blacklistDetails.BlacklistGlobally)
		{
			_itemConfig.Blacklist.Add(newItemId);
		}
		
		if (blacklistDetails.BlacklistFromFence)
		{
			_traderConfig.Fence.Blacklist.Add(newItemId);
		}

		if (blacklistDetails.BlacklistFromFlea)
		{
			_ragfairConfig.Dynamic.Blacklist.Custom.Add(newItemId);
		}

		if (blacklistDetails.BlacklistFromScavCase)
		{
			_scavCaseConfig.RewardItemParentBlacklist.Add(newItemId);
		}
	}
}