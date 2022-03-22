using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimRoomTest : Room {

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
      

        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {

                if (x != 0 && x != LevelGenerator.ROOM_WIDTH - 1 &&
                    y != 0 && y != LevelGenerator.ROOM_HEIGHT - 1)
                    continue;

                if (requiredExits.downExitRequired && y == 0) {
                    //leave a gap for a door
                    if (x == 4 || x == 5)
                        continue;
                }

                if (requiredExits.upExitRequired && y == LevelGenerator.ROOM_HEIGHT - 1) {
                    //leave a gap for a door
                    if (x == 4 || x == 5)
                        continue;
                }

                if (requiredExits.leftExitRequired && x == 0) {
                    //leave a gap for a door
                    if (y == 3 || y == 4)
                        continue;
                }

                if (requiredExits.rightExitRequired && x == LevelGenerator.ROOM_WIDTH - 1) {
                    //leave a gap for a door
                    if (y == 3 || y == 4)
                        continue;
                }

                Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
            }
        }

    }
}


