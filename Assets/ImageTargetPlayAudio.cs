using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Audio;
using Vuforia;

public class ImageTargetPlayAudio : MonoBehaviour, ITrackableEventHandler {


	//initialize audio and video sources and vuforia trackable behaviour
	private TrackableBehaviour mTrackableBehaviour;
	public AudioSource BirthdayAudio;
	public VideoPlayer BirthdayVideo;



	void Start()
	{

		//get existing components and add audio
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		BirthdayAudio = gameObject.AddComponent<AudioSource>();
		BirthdayVideo = GetComponent<VideoPlayer>();

		//prevent audio from playing
		BirthdayAudio.playOnAwake = false;

		//configure audio to play with video
		BirthdayVideo.audioOutputMode = VideoAudioOutputMode.AudioSource;
		BirthdayVideo.EnableAudioTrack (0, true);
		BirthdayVideo.SetTargetAudioSource (0, BirthdayAudio);
		BirthdayVideo.Prepare ();


		if (mTrackableBehaviour)
		{//if the trackable behaviour was initialized, track changes in the status
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}



	}
		  

	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{//triggers when registered trackables change in status

		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{//if a target is found
			
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

