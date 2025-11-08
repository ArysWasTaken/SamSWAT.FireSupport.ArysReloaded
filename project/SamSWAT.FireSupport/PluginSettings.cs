using BepInEx.Configuration;

namespace SamSWAT.FireSupport.ArysReloaded;

internal static class PluginSettings
{
	internal static ConfigEntry<bool> Enabled { get; private set; }
	internal static ConfigEntry<int> AmountOfStrafeRequests { get; private set; }
	internal static ConfigEntry<int> AmountOfExtractionRequests { get; private set; }
	internal static ConfigEntry<int> HelicopterWaitTime { get; private set; }
	internal static ConfigEntry<float> HelicopterExtractTime { get; private set; }
	internal static ConfigEntry<float> HelicopterSpeedMultiplier { get; private set; }
	internal static ConfigEntry<int> RequestCooldown { get; private set; }
	internal static ConfigEntry<int> VoiceoverVolume { get; private set; }
	
	internal static void Initialize(ConfigFile config)
	{
		Enabled = config.Bind(
			"",
			"Plugin state",
			true,
			new ConfigDescription("Enables/disables plugin"));
		
		AmountOfStrafeRequests = config.Bind(
			"Main Settings",
			"Amount of autocannon strafe requests",
			2,
			new ConfigDescription("",
				new AcceptableValueRange<int>(0, 10)));
		AmountOfExtractionRequests = config.Bind(
			"Main Settings",
			"Amount of helicopter extraction requests",
			1,
			new ConfigDescription("",
				new AcceptableValueRange<int>(0, 10)));
		RequestCooldown = config.Bind(
			"Main Settings",
			"Cooldown between support requests",
			300,
			new ConfigDescription("Seconds",
				new AcceptableValueRange<int>(60, 3600)));
		
		HelicopterWaitTime = config.Bind(
			"Helicopter Extraction Settings",
			"Helicopter wait time",
			30,
			new ConfigDescription("Helicopter wait time on extraction location (seconds)",
				new AcceptableValueRange<int>(10, 300)));
		HelicopterExtractTime = config.Bind(
			"Helicopter Extraction Settings",
			"Extraction time",
			10f,
			new ConfigDescription("How long you will need to stay in the exfil zone before extraction (seconds)",
				new AcceptableValueRange<float>(1f, 30f)));
		HelicopterSpeedMultiplier = config.Bind(
			"Helicopter Extraction Settings",
			"Helicopter speed multiplier",
			1f,
			new ConfigDescription("How fast the helicopter arrival animation will be played",
				new AcceptableValueRange<float>(0.8f, 1.5f)));
		
		VoiceoverVolume = config.Bind(
			"Sound Settings",
			"Voiceover volume",
			90,
			new ConfigDescription("",
				new AcceptableValueRange<int>(0, 100)));
	}
}