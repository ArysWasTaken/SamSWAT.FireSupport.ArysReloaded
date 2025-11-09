using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services.Mod;
using System.Reflection;

namespace SamSWAT.FireSupport.ArysReloaded;

public record ServerModMetadata : AbstractModMetadata
{
	public override string ModGuid { get; init; } = "com.samswat.firesupport.arysreloaded";
	public override string Name { get; init; } = "SamSWAT's FireSupport: Arys Reloaded";
	public override string Author { get; init; } = "SamSWAT, Arys";
	public override List<string>? Contributors { get; init; }
	public override SemanticVersioning.Version Version { get; init; } = new(ModMetadata.VERSION);
	public override SemanticVersioning.Range SptVersion { get; init; } = new($"~{ModMetadata.TARGET_SPT_VERSION}");
	public override List<string>? Incompatibilities { get; init; }
	public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
	public override string? Url { get; init; }
	public override bool? IsBundleMod { get; init; }
	public override string License { get; init; } = "Creative Commons BY-NC 3.0";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ServerMod(
	ConfigServer configServer,
	ModHelper modHelper,
	CustomItemService customItemService)
	: IOnLoad
{
	private readonly ItemConfig _itemConfig = configServer.GetConfig<ItemConfig>();
	private readonly TraderConfig _traderConfig = configServer.GetConfig<TraderConfig>();
	private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();
	private readonly ScavCaseConfig _scavCaseConfig = configServer.GetConfig<ScavCaseConfig>();

	public Task OnLoad()
	{
		AddCustomItems();

		return Task.CompletedTask;
	}

	private void AddCustomItems()
	{
		string pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
		string pathToModDatabase = Path.Combine(pathToMod, "database");
		string[] databaseFiles = Directory.GetFiles(pathToModDatabase, "*.json");

		foreach (string databaseFile in databaseFiles)
		{
			var newItemDetails = modHelper.GetJsonDataFromFile<NewCustomItemDetails>(pathToModDatabase, databaseFile);
			customItemService.CreateItem(newItemDetails);
			AddItemToBlacklists(newItemDetails);
		}
	}

	private void AddItemToBlacklists(NewCustomItemDetails newItemDetails)
	{
		if (newItemDetails.BlacklistDetails == null) return;
		
		MongoId itemId = newItemDetails.NewItem!.Id;

		if (!newItemDetails.BlacklistDetails.BlacklistGlobally)
		{
			if (newItemDetails.BlacklistDetails.BlacklistFromFence)
			{
				_traderConfig.Fence.Blacklist.Add(itemId);
			}

			if (newItemDetails.BlacklistDetails.BlacklistFromFlea)
			{
				_ragfairConfig.Dynamic.Blacklist.Custom.Add(itemId);
			}

			if (newItemDetails.BlacklistDetails.BlacklistFromScavCase)
			{
				_scavCaseConfig.RewardItemParentBlacklist.Add(itemId);
			}
		}
		else
		{
			_itemConfig.Blacklist.Add(itemId);
		}
	}
}