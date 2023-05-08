using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<gunStats> gunList;
    public int selectedGun;
    public Vector3 playerPos;
    public SerializableDictionary<string, bool> gunsPickedUp;

    public GameData()
    {
        this.gunList = new List<gunStats>();
        selectedGun = 0;
        playerPos = Vector3.zero;
        gunsPickedUp = new SerializableDictionary<string, bool>();
    }
}
