using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Controller_level_gr : MonoBehaviour
{
    
    public static Controller_level_gr instance;
    Dictionary<IAPProductKey,LevelManager> DIC_LV_IAPKEY;
    void Awake()
    {
        instance=this;
    }

    void Start()
    {
        DIC_LV_IAPKEY= new Dictionary<IAPProductKey, LevelManager>();
        List<LevelManager> levelManagers= new List<LevelManager>();
        levelManagers.AddRange(GetComponentsInChildren<LevelManager>());
        foreach(var level in levelManagers)
        {
            DIC_LV_IAPKEY.Add(level.iAPProductKey, level);
        }
    }

    public void UnlockedLevel(IAPProductKey iAPProductKey)
    {
        if(DIC_LV_IAPKEY.TryGetValue(iAPProductKey,out LevelManager levelManager))
        {
            levelManager.UnLock();
        }
    }
    
    
}
