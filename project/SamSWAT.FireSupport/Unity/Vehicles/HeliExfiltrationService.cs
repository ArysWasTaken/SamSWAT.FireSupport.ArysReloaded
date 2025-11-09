using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public sealed class HeliExfiltrationService(FireSupportSpotter spotter, int maxRequests)
	: FireSupportService(maxRequests)
{
	public override ESupportType SupportType => ESupportType.Extract;

	public override async UniTaskVoid PlanRequest(CancellationToken cancellationToken)
	{
		Vector3 position = await spotter.SetLocation(checkSpace: true, cancellationToken);
		
		if (await spotter.ConfirmLocation(cancellationToken))
		{
			ConfirmRequest(position, cancellationToken).Forget();
		}
	}

	private async UniTaskVoid ConfirmRequest(Vector3 position, CancellationToken cancellationToken)
	{
		requestAvailable = false;
		availableRequests--;
		FireSupportController.Instance.CanCallSupport(false);

		IFireSupportBehaviour uh60 = FireSupportPoolManager.Instance.TakeFromPool(SupportType);
		FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.StationExtractionRequest);
		await UniTask.WaitForSeconds(8f, cancellationToken: cancellationToken);

		var randomEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
		uh60.ProcessRequest(position, Vector3.zero, randomEulerAngles, cancellationToken);
		FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.SupportHeliArrivingToPickup);
		await UniTask.WaitForSeconds(35f + PluginSettings.HelicopterWaitTime.Value,
			cancellationToken: cancellationToken);

		FireSupportController.Instance
			.StartCooldown(PluginSettings.RequestCooldown.Value, cancellationToken, OnCooldownOver)
			.Forget();
	}

	private void OnCooldownOver()
	{
		requestAvailable = true;
	}
}