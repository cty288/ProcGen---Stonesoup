using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimRandomChoiceRoom : Room
{
    public GameObject[] roomChoices;
    
    public override Room createRoom(ExitConstraint requiredExits)
    {
        if (!GameObject.Find("_GameManager").GetComponent<TimRoomManager>())
        {
            GameObject.Find("_GameManager").AddComponent<TimRoomManager>();
        }


        GameObject roomPrefab = GlobalFuncs.randElem(roomChoices);

        if (requiredExits.requiredExitLocations().Count() > 0) {
            roomPrefab = roomChoices[0]; //dungeon
        }
        else {
            int spawnChance = Random.Range(0, 100);
            if (spawnChance <= 20) {
                roomPrefab = roomChoices[1];
            }else if(spawnChance>20 && spawnChance<=50) {
                roomPrefab = roomChoices[2];
            }else if (spawnChance>50 && spawnChance<=80) {
                if (GameManager.gameMode != GameManager.GameMode.SingleRoom) {
                    //combined mode
                    roomPrefab = roomChoices[3];
                }
                else {
                    roomPrefab = roomChoices[0];
                }
            }else {
                roomPrefab = roomChoices[0]; //walls
            }
            
        }

        Room createdRoom = roomPrefab.GetComponent<Room>().createRoom(requiredExits);
        GameObject.Find("_GameManager").GetComponent<TimRoomManager>().TimRooms.Add(createdRoom);
        return createdRoom;
    }
}
