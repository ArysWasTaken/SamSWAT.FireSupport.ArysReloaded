using Comfort.Common;
using Cysharp.Threading.Tasks;
using SamSWAT.FireSupport.ArysReloaded.Utils;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public class FireSupportAudio : ScriptableObject, IDisposable
{
	[SerializeField] private AudioClip[] stationReminder;
	[SerializeField] private AudioClip[] stationAvailable;
	[SerializeField] private AudioClip[] stationDoesNotHear;
	[SerializeField] private AudioClip[] stationStrafeRequest;
	[SerializeField] private AudioClip[] stationStrafeEnd;
	[SerializeField] private AudioClip[] stationExtractionRequest;
	[SerializeField] private AudioClip[] jetArriving;
	[SerializeField] private AudioClip[] jetFiring;
	[SerializeField] private AudioClip[] jetLeaving;
	[SerializeField] private AudioClip[] supportHeliArriving;
	[SerializeField] private AudioClip[] supportHeliArrivingToPickup;
	[SerializeField] private AudioClip[] supportHeliPickingUp;
	[SerializeField] private AudioClip[] supportHeliHurry;
	[SerializeField] private AudioClip[] supportHeliLeaving;
	[SerializeField] private AudioClip[] supportHeliLeavingAfterPickup;
	[SerializeField] private AudioClip[] supportHeliLeavingNoPickup;

	public static FireSupportAudio Instance { get; private set; }

	public static async UniTask<FireSupportAudio> Create()
	{
		Instance = await AssetLoader.LoadAssetAsync<FireSupportAudio>("assets/content/ui/firesupport_audio.bundle");
		return Instance;
	}

	public void Dispose()
	{
		Instance = null;
		DestroyImmediate(this);
	}

	public void PlayVoiceover(EVoiceoverType voiceoverType)
	{
		AudioClip voAudioClip = voiceoverType switch
		{
			EVoiceoverType.StationReminder => stationReminder.GetRandomClip(),
			EVoiceoverType.StationAvailable => stationAvailable.GetRandomClip(),
			EVoiceoverType.StationDoesNotHear => stationDoesNotHear.GetRandomClip(),
			EVoiceoverType.StationStrafeRequest => stationStrafeRequest.GetRandomClip(),
			EVoiceoverType.StationStrafeEnd => stationStrafeEnd.GetRandomClip(),
			EVoiceoverType.StationExtractionRequest => stationExtractionRequest.GetRandomClip(),
			EVoiceoverType.JetArriving => jetArriving.GetRandomClip(),
			EVoiceoverType.JetFiring => jetFiring.GetRandomClip(),
			EVoiceoverType.JetLeaving => jetLeaving.GetRandomClip(),
			EVoiceoverType.SupportHeliArriving => supportHeliArriving.GetRandomClip(),
			EVoiceoverType.SupportHeliArrivingToPickup => supportHeliArrivingToPickup.GetRandomClip(),
			EVoiceoverType.SupportHeliPickingUp => supportHeliPickingUp.GetRandomClip(),
			EVoiceoverType.SupportHeliHurry => supportHeliHurry.GetRandomClip(),
			EVoiceoverType.SupportHeliLeaving => supportHeliLeaving.GetRandomClip(),
			EVoiceoverType.SupportHeliLeavingAfterPickup => supportHeliLeavingAfterPickup.GetRandomClip(),
			EVoiceoverType.SupportHeliLeavingNoPickup => supportHeliLeavingNoPickup.GetRandomClip(),
			_ => throw new ArgumentException("Invalid voiceover type")
		};

		const BetterAudio.AudioSourceGroupType sourceGroup = BetterAudio.AudioSourceGroupType.Nonspatial;
		float volume = PluginSettings.VoiceoverVolume.Value / 100f;
		Singleton<BetterAudio>.Instance.PlayNonspatial(voAudioClip, sourceGroup, 0, volume);
	}
}

internal static class AudioClipExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static AudioClip GetRandomClip(this AudioClip[] clips)
	{
		return clips[Random.Range(0, clips.Length)];
	}
}