using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimRoomManager : MonoBehaviour {
    private Dictionary<Vector2Int, ExitConstraint> additionalExitConstraints =
        new Dictionary<Vector2Int, ExitConstraint>();

    public List<Room> TimRooms = new List<Room>();

    public void SetAdditionalExitsForNeighbours(Vector2Int selfRooomPosition, Dir selfAddedExits) {
        //neighbour down
        if (selfAddedExits == Dir.Down) {
            SetAdditionalExits(selfRooomPosition + new Vector2Int(0, -1), Dir.Up);
        }

        if (selfAddedExits == Dir.Left) {
            SetAdditionalExits(selfRooomPosition + new Vector2Int(-1, 0), Dir.Right);
        }

        if (selfAddedExits == Dir.Right) {
            SetAdditionalExits(selfRooomPosition + new Vector2Int(1, 0), Dir.Left);
        }

        if (selfAddedExits == Dir.Up) {
            SetAdditionalExits(selfRooomPosition + new Vector2Int(0, 1), Dir.Down);
        }
    }

    private void SetAdditionalExits(Vector2Int targetPosition, Dir dir) {
        if (additionalExitConstraints.ContainsKey(targetPosition)) {
            additionalExitConstraints[targetPosition].addDirConstraint(dir);
        }
        else {
            ExitConstraint constraint = new ExitConstraint();
            constraint.addDirConstraint(dir);
            additionalExitConstraints.Add(targetPosition, constraint);
        }
    }

    public ExitConstraint GetAdditionalExits(Vector2Int position) {
        if (additionalExitConstraints.ContainsKey(position)) {
            return additionalExitConstraints[position];
        }
        else {
            return new ExitConstraint();
        }
    }

}
