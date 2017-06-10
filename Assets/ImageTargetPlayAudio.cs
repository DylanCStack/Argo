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

		BirthdayAudio.playOnAwake = false;

		BirthdayVideo.audioOutputMode = VideoAudioOutputMode.AudioSource;
		BirthdayVideo.EnableAudioTrack (0, true);
		BirthdayVideo.SetTargetAudioSource (0, BirthdayAudio);
		BirthdayVideo.Prepare ();


		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}



	}
		  

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

