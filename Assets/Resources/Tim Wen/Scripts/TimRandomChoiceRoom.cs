using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimRandomChoiceRoom : Room
{
    public GameObject[] roomChoices;
    public List<GameObject> designedRooms;

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
                Debug.Log("Exit count: 1");
                //Debug.Log("RandomChoiceRoom: "+ new Vector2Int(roomGridX, roomGridY));
                int spawnChance = Random.Range(0, 100);
                if (spawnChance >= 70) {
                    roomPrefab = roomChoices[4]; //tunnel
                }
                else if(spawnChance>=40){
                    roomPrefab = roomChoices[0]; //dungeon
                }
                else {
                    if (Random.Range(0, 100) >= 50) {
                        roomPrefab = roomChoices[6]; //Wen room
                    }
                    else {
                        roomPrefab = GlobalFuncs.randElem(designedRooms); //Wen room
                    }
                }
            }
            else {
                int roomChance = Random.Range(0, 100);
                if (roomChance > 65) {
                    // roomPrefab = roomChoices[5]; //Wen's room
                    roomPrefab = roomChoices[0]; //dungeon
                }
                else if(roomChance>=55){
                    roomPrefab = roomChoices[5]; //grid
                }
                else if(roomChance>=10){
                    if (Random.Range(0, 100) >= 70)
                    {
                        roomPrefab = roomChoices[6]; //Wen room
                    }
                    else
                    {
                        roomPrefab = GlobalFuncs.randElem(designedRooms); //Wen room
                    }
                }
                else {
                    roomPrefab = roomChoices[4]; //tunnel
                }
               
            }
           
        }
        else {
            int spawnChance = Random.Range(0, 100);
            if (spawnChance <= 10) { 
                roomPrefab = roomChoices[1]; //treasure
            }else if (spawnChance >=10 && spawnChance <= 20) {
                roomPrefab = roomChoices[4]; //tunnel
            } else if(spawnChance>20 && spawnChance<=40) { //big dungeon
                roomPrefab = roomChoices[2];
            }else if (spawnChance>40 && spawnChance<=45) { //teleport
                if (GameManager.gameMode != GameManager.GameMode.SingleRoom) {
                    //combined mode
                    roomPrefab = roomChoices[3];
                }
                else {
                    roomPrefab = roomChoices[0];
                }
            }else if(spawnChance>45 && spawnChance<=90){ //tunnel or all walls
                if (Random.Range(0, 100) >= 70)
                {
                    roomPrefab = roomChoices[6]; //Wen room
                }
                else
                {
                    roomPrefab = GlobalFuncs.randElem(designedRooms); //Wen room
                }
            }
            else {
                roomPrefab = roomChoices[0]; //walls
            }
            
        }

        Room createdRoom = roomPrefab.GetComponent<Room>().createRoom(requiredExits);
        GameObject.Find("_GameManager").GetComponent<TimRoomManager>().TimRooms.Add(createdRoom);
        return createdRoom;
    }

    
}
