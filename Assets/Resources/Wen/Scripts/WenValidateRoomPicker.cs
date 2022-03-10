using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WenValidateRoomPicker : Room
{
    public List<Room> RoomChoices;

    public override Room createRoom(ExitConstraint requiredExits)
    {
        List<Room> RoomsThatMeetsConstraints = new List<Room>();
        foreach (Room room in RoomChoices)
        {
            WenValidateRoom validatedRoom = room.GetComponent<WenValidateRoom>();
            if (validatedRoom.MeetsConstraints(requiredExits))
            {
                RoomsThatMeetsConstraints.Add(validatedRoom);
            }

        }
        return GlobalFuncs.randElem(RoomsThatMeetsConstraints).createRoom(requiredExits);
    }
}
