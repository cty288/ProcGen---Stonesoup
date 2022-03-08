using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimValidatedRoomPicker : Room
{
    public List<Room> RoomChoices;

    public override Room createRoom(ExitConstraint requiredExits)
    {
        List<Room> RoomsThatMeetsConstraints = new List<Room>();
        foreach (Room room in RoomChoices)
        {
            TimValidatedRoom validatedRoom = room.GetComponent<TimValidatedRoom>();
            Debug.Log(validatedRoom.MeetsConstraints(requiredExits));
            if (validatedRoom && validatedRoom.MeetsConstraints(requiredExits))
            {
                RoomsThatMeetsConstraints.Add(validatedRoom);
            }

        }
        return GlobalFuncs.randElem(RoomsThatMeetsConstraints).createRoom(requiredExits);
    }
}
