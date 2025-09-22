using BepInEx;
using BepInEx.Logging;
using SamSWAT.FireSupport.ArysReloaded.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityToolkit.Utils;

namespace SamSWAT.FireSupport.ArysReloaded;

[BepInPlugin("com.SamSWAT.FireSupport.ArysReloaded", "SamSWAT's FireSupport: Arys Reloaded", "2.3.0")]
[BepInDependency("com.Arys.UnityToolkit", "1.3.0")]
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
		
		new ModulePatchManager(assembly).EnableAllPatches();
		
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