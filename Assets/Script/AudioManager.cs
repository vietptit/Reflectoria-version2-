using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // THÊM DÒNG NÀY

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] List<AudioSource> audioSources;
    // 0 click
    //1 mouseClick
    //2 move
    //3 rotate
    //4 audio mirror
    //5 magic
    //6 spaceship
    //7 beam 
    //8 dash

    AudioSource source; // lưu nhạc nền
    [SerializeField] AudioClip music_menu;
    [SerializeField] AudioClip music_play;
    [SerializeField] Transform parentsound_Effect;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        audioSources.AddRange(parentsound_Effect.GetComponentsInChildren<AudioSource>());
        source = GetComponent<AudioSource>();
        SwitchMenu();
    }

    public void SwitchMenu() => SwithAudioBackGround(music_menu);
    public void SwitchPlay() => SwithAudioBackGround(music_play);

    public void SwithAudioBackGround(AudioClip audioClip)
    {
        source.clip = audioClip;
        source.Play();
    }

    public void PlayAudioClick() => audioSources[0].Play();
    public void PLayAudioClickButClickMouse() => audioSources[1].Play();
    public void PlayAudioMove() => audioSources[2].Play();
    public void PlayAudioRotate() => audioSources[3].Play();
    public void PlayAudioMirrorMove() => audioSources[4].Play();
    public void PlayAudioWhenFindPath() => audioSources[5].Play();


  #region  SplaceShip
    public void PlayAudioSpaceShipFadeIn(float fadeDuration, float maxVolume = 1f)
    {
        AudioSource shipSource = audioSources[6];
        shipSource.volume = 0f; 
        shipSource.Play();
        shipSource.DOFade(maxVolume, fadeDuration);
    }

  
    public Tween GetSpaceShipFadeOutTween(float fadeDuration)
    {
        return audioSources[6].DOFade(0f, fadeDuration);
    }

   
    public void StopAudioSpaceShip()
    {
        AudioSource shipSource = audioSources[6];
        shipSource.Stop();
        shipSource.volume = 1f; 
    }
    #endregion

    public void PlayAudioWhiskAwayPlayer() => audioSources[7].Play();


    public void StopAudioWhiskAwayPlayer()=>audioSources[7].Stop();
    public void PlayAudioDashSpaceShip()=>audioSources[8].Play();
}