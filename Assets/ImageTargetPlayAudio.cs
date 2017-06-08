using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Audio;
using Vuforia;

public class ImageTargetPlayAudio : MonoBehaviour, ITrackableEventHandler {

	private TrackableBehaviour mTrackableBehaviour;
	public AudioSource BirthdayAudio;
	public VideoPlayer BirthdayVideo;



	void Start()
	{
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		BirthdayAudio = gameObject.AddComponent<AudioSource>();
		BirthdayVideo = GetComponent<VideoPlayer>();

//		BirthdayAudio.playOnAwake = false;

		BirthdayVideo.audioOutputMode = VideoAudioOutputMode.AudioSource;
		BirthdayVideo.EnableAudioTrack (0, true);
		BirthdayVideo.SetTargetAudioSource (0, BirthdayAudio);
		BirthdayVideo.Prepare ();


		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}




	}

	//	void Update () {
	//		StateManager sm = TrackerManager.Instance.GetStateManager ();
	//
	//		// Query the StateManager to retrieve the list of
	//		// currently 'active' trackables 
	//		//(i.e. the ones currently being tracked by Vuforia)
	//		IList<TrackableBehaviour> activeTrackables = (IList<TrackableBehaviour>) sm.GetActiveTrackableBehaviours ();
	//
	//		// Iterate through the list of active trackables
	//		foreach (TrackableBehaviour tb in activeTrackables) {
	//			if ((tb.CurrentStatus == TrackableBehaviour.Status.TRACKED) ||
	//			    (tb.CurrentStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) ||
	//			    (tb.CurrentStatus == TrackableBehaviour.Status.DETECTED)) {
	//
	//				if (!BirthdayAudio.isPlaying) {
	//					BirthdayAudio.Play();
	//				}
	//
	//			} 
	//
	//		}
	//
	//		if (activeTrackables.Count == 0) {
	//			BirthdayAudio.Pause();
	//		}
	//			
	//
	//
	//	}   

	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{

		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			// Play audio when target is found
			BirthdayAudio.Play();
			BirthdayVideo.Play();

		}
		else
		{
			// Stop audio when target is lost
			BirthdayVideo.Pause();
			BirthdayAudio.Pause();


		}
	} 

}

