using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimRandomChoiceRoom : Room
{
    public GameObject[] roomChoices;
    protected TimRoomManager roomManager;
    [SerializeField] private GameObject enemySpawnerPrefab;
    public override Room createRoom(ExitConstraint requiredExits)
    {
        if (!GameObject.Find("_GameManager").GetComponent<TimRoomManager>())
        {
            GameObject.Find("_GameManager").AddComponent<TimRoomManager>();
        }
        roomManager = GameObject.Find("_GameManager").GetComponent<TimRoomManager>();

        GameObject roomPrefab = GlobalFuncs.randElem(roomChoices);

//        IEnumerable additionalConstraint =

        if (requiredExits.requiredExitLocations().Any()) {

            int exitCount = requiredExits.requiredExitLocations().Count();


            if (exitCount == 1) {
                //Debug.Log("RandomChoiceRoom: "+ new Vector2Int(roomGridX, roomGridY));
                int spawnChance = Random.Range(0, 100);
                if (spawnChance >= 50) {
                    roomPrefab = roomChoices[4]; //tunnel
                }
                else {
                    roomPrefab = roomChoices[0]; //dungeon
                }
            }
            else {
                roomPrefab = roomChoices[0]; //dungeon
            }
           
        }
        else {
            int spawnChance = Random.Range(0, 100);
            if (spawnChance <= 20) { 
                roomPrefab = roomChoices[1]; //treasure
            }else if(spawnChance>20 && spawnChance<=40) { //big dungeon
                roomPrefab = roomChoices[2];
            }else if (spawnChance>40 && spawnChance<=60) { //teleport
                if (GameManager.gameMode != GameManager.GameMode.SingleRoom) {
                    //combined mode
                    roomPrefab = roomChoices[3];
                }
                else {
                    roomPrefab = roomChoices[0];
                }
            }else { //tunnel or all walls
                roomPrefab = roomChoices[4]; //walls
            }
            
        }

        Room createdRoom = roomPrefab.GetComponent<Room>().createRoom(requiredExits);
        GameObject.Find("_GameManager").GetComponent<TimRoomManager>().TimRooms.Add(createdRoom);
        return createdRoom;
    }

    
}
