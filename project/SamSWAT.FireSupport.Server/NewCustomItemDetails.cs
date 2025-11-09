using SPTarkov.Server.Core.Models.Spt.Mod;
using System.Text.Json.Serialization;

namespace SamSWAT.FireSupport.ArysReloaded;

public record NewCustomItemDetails : NewItemDetails
{
	[JsonPropertyName("blacklist")]
	public BlacklistDetails? BlacklistDetails { get; set; }
}

public class BlacklistDetails
{
	[JsonPropertyName("all")]
	public bool BlacklistGlobally { get; set; }
	
	[JsonPropertyName("flea")]
	public bool BlacklistFromFlea { get; set; }
	
	[JsonPropertyName("fence")]
	public bool BlacklistFromFence { get; set; }
	
	[JsonPropertyName("scavCase")]
	public bool BlacklistFromScavCase { get; set; }
}