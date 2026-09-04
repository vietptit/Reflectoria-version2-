using UnityEngine;

public class SaveLoadFile : MonoBehaviour
{
    public static SaveLoadFile instance;
    const string LEVEL_Key="LEVEL_KEY";
    const string SOUND_BACKGROUND_KEY="SOUND_BACKGROUND";
    const string SOUND_EFFECT_KEY="SOUND_EFFECT";

    void Awake()
    {
        if (instance == null)
        {
            instance=this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
#region  Level
    public void SaveLevel(int next_level)
    {
        PlayerPrefs.SetInt(LEVEL_Key,next_level);
        PlayerPrefs.Save();
    }

    public int GetLevel() => PlayerPrefs.GetInt(LEVEL_Key,0);


    [ContextMenu("Reset level")]
    public void ResetLevel()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
#endregion

#region  Sound
    public float GetVolumeBackGroundMusic()=> PlayerPrefs.GetFloat(SOUND_BACKGROUND_KEY,0.5f);
    public float GetVolumeSoundEffect() => PlayerPrefs.GetFloat(SOUND_EFFECT_KEY,.5f);
    public void SetVolumeBackGroundMusic(float amount) => PlayerPrefs.SetFloat(SOUND_BACKGROUND_KEY,amount);
    public void SetVolumeSoundEffect(float amount) => PlayerPrefs.SetFloat(SOUND_EFFECT_KEY,amount);
#endregion
}
