using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services.Mod;
using System.Reflection;

namespace SamSWAT.FireSupport.ArysReloaded;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ServerMod(
	ISptLogger<ServerMod> logger,
	ModHelper modHelper,
	CustomItemService customItemService) : IOnLoad
{
	public Task OnLoad()
	{
		string pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
		string pathToModDatabase = System.IO.Path.Combine(pathToMod, "database");
		string[] databaseFiles = Directory.GetFiles(pathToModDatabase, "*.json");

		foreach (string databaseFile in databaseFiles)
		{
			var templateItem = modHelper.GetJsonDataFromFile<TemplateItem>(pathToModDatabase, databaseFile);
			var newItemDetails = new NewItemDetails
			{
				NewItem = templateItem,
				
			};
			customItemService.CreateItem(newItemDetails);
		}

		return Task.CompletedTask;
	}
}