using BepInEx;
using BepInEx.Logging;
using SamSWAT.FireSupport.ArysReloaded.Unity;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SamSWAT.FireSupport.ArysReloaded;

[BepInPlugin("com.samswat.firesupport.arysreloaded", "SamSWAT's FireSupport: Arys Reloaded", ModMetadata.VERSION)]
[BepInDependency("com.SPT.core", ModMetadata.TARGET_SPT_VERSION)]
[BepInDependency("com.arys.unitytoolkit", "2.0.0")]
public class FireSupportPlugin : BaseUnityPlugin
{
	private readonly List<UpdatableComponentBase> _componentsToUpdate = [];
	private Predicate<UpdatableComponentBase> _isMarkedForRemovalPredicate;

	public static FireSupportPlugin Instance { get; private set; }

	internal static string Directory { get; private set; }
	internal static ManualLogSource LogSource { get; private set; }

	private void Awake()
	{
		var assembly = Assembly.GetExecutingAssembly();

		Instance = this;
		LogSource = Logger;
		Directory = Path.GetDirectoryName(assembly.Location);

		new PatchManager(this, true).EnablePatches();

		PluginSettings.Initialize(Config);
	}

	private void Update()
	{
		UpdateComponents();
	}

	public void RegisterComponent(UpdatableComponentBase component)
	{
		_componentsToUpdate.Add(component);
	}

	private void UpdateComponents()
	{
		if (_componentsToUpdate.Count == 0)
		{
			return;
		}

		_componentsToUpdate.RemoveAll(_isMarkedForRemovalPredicate ??= UpdatableComponentBase.IsMarkedForRemoval);

		int count = _componentsToUpdate.Count;
		for (var i = 0; i < count; i++)
		{
			UpdatableComponentBase component = _componentsToUpdate[i];

			if (!component.IsMarkedForRemoval() && component.HasFinishedInitialization)
			{
				component.ManualUpdate();
			}
		}
	}
}