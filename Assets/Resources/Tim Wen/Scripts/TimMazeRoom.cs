using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimMazeRoom : Room {
    protected TimRoomManager roomManager;
    private LevelGenerator generator;
    private void Awake() {
        roomManager = GameObject.Find("_GameManager").GetComponent<TimRoomManager>();
    }
   
    

    
}
