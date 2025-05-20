using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public sealed class HeliExfiltrationService(
	FireSupportSpotter spotter,
	CancellationToken token,
	int maxRequests) : FireSupportService(maxRequests)
{
	public override ESupportType SupportType => ESupportType.Extract;
	
	public override async UniTaskVoid PlanRequest()
	{
		try
		{
			Vector3 position = await spotter.SetLocation(checkSpace: true, token);
			await spotter.ConfirmLocation(token);
			ConfirmRequest(position).Forget();
		}
		catch (OperationCanceledException) {}
		catch (Exception ex)
		{
			FireSupportPlugin.LogSource.LogError(ex);
		}
	}
	
	private async UniTaskVoid ConfirmRequest(Vector3 position)
	{
		try
		{
			requestAvailable = false;
			availableRequests--;
			
			IFireSupportBehaviour uh60 = FireSupportPoolManager.Instance.TakeFromPool(SupportType);
			FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.StationExtractionRequest);
			await UniTask.WaitForSeconds(8f, cancellationToken: token);
			
			var randomEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
			uh60.ProcessRequest(position, Vector3.zero, randomEulerAngles, token);
			FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.SupportHeliArrivingToPickup);
			await UniTask.WaitForSeconds(35f + PluginSettings.HelicopterWaitTime.Value, cancellationToken: token);
			
			FireSupportController.Instance
				.StartCooldown(PluginSettings.RequestCooldown.Value, token, OnCooldownOver)
				.Forget();
		}
		catch (OperationCanceledException) {}
		catch (Exception ex)
		{
			FireSupportPlugin.LogSource.LogError(ex);
		}
	}
	
	private void OnCooldownOver()
	{
		requestAvailable = true;
	}
}