using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxSource : MonoBehaviour
{
    private int index;
    private AudioSource source;

    public SfxSource(AudioClip clip, int index, float volume){
        this.index = index;
        source = new AudioSource();
        source.loop = false;
        source.volume = volume;

        SetAudioClip(clip);
        Play();
    }

	void Update()
	{
		if(!source.isPlaying){
            AudioManagerBetter.main.PopSfxSource(index);
        }
	}

	public int Index(){
        return index;
    }

    public void SetIndex(int index){
        this.index = index;
    }

    public AudioSource Source(){
        return source;
    }

    public void SetAudioClip(AudioClip clip){
        source.clip = clip;
    }

    public void Play(){
        source.Play();
    }

    public void Pause(){
        source.Pause();
    }

    public void UnPause(){
        source.UnPause();
    }

    public void Volume(float volume){
        source.volume = volume;
    }
}
