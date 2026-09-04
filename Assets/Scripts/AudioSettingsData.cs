using UnityEngine;
using UnityEngine.UI; // Đổi thành UnityEngine.UI cho Slider của Canvas
using UnityEngine.Audio; // Khai báo thư viện Audio

public class AudioSettingsData : MonoBehaviour
{
    [SerializeField] Slider soundBase_sl;
    [SerializeField] Slider soundEffect_sl;
    
    [Header("Gán MainMixer vào đây")]
    [SerializeField] AudioMixer mainMixer;

    private const string bgmParam = "BGM"; 
    private const string sfxParam = "SFX";

    void OnEnable()
    {
        
        float savedBGM = SaveLoadFile.instance.GetVolumeBackGroundMusic();
        float savedSFX = SaveLoadFile.instance.GetVolumeSoundEffect();

        
        soundBase_sl.value = savedBGM;
        soundEffect_sl.value = savedSFX;

        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);


        soundBase_sl.onValueChanged.AddListener(SetBGMVolume);
        soundEffect_sl.onValueChanged.AddListener(SetSFXVolume);
    }

    void OnDisable()
    {
        
        soundBase_sl.onValueChanged.RemoveListener(SetBGMVolume);
        soundEffect_sl.onValueChanged.RemoveListener(SetSFXVolume);

      
        SaveLoadFile.instance.SetVolumeBackGroundMusic(soundBase_sl.value);
        SaveLoadFile.instance.SetVolumeSoundEffect(soundEffect_sl.value);
    }

    
    private void SetBGMVolume(float value)
    {
      
        float clampedValue = Mathf.Clamp(value, 0.0001f, 1f); 
        mainMixer.SetFloat(bgmParam, Mathf.Log10(clampedValue) * 20f);
    }

    private void SetSFXVolume(float value)
    {
        float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);
        mainMixer.SetFloat(sfxParam, Mathf.Log10(clampedValue) * 20f);
    }
}