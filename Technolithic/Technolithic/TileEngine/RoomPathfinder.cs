using System.Collections.Generic;

namespace Technolithic
{
    public static class RoomPathfinder
    {
        private static readonly Queue<Room> _queue = new Queue<Room>();
        private static readonly HashSet<Room> _visited = new HashSet<Room>();
        private static readonly Dictionary<Room, int> _distances = new Dictionary<Room, int>();

        public static int? FindDistance(Room startRoom, Room endRoom)
        {
            if (startRoom.ZoneId != endRoom.ZoneId)
            {
                return null;
            }

            if (startRoom == endRoom)
            {
                return 0;
            }

            _queue.Clear();
            _visited.Clear();
            _distances.Clear();

            _queue.Enqueue(startRoom);
            _visited.Add(startRoom);
            _distances[startRoom] = 0;

            while (_queue.Count > 0)
            {
                var currentRoom = _queue.Dequeue();
                var currentDistance = _distances[currentRoom];

                if (currentRoom == endRoom)
                {
                    return currentDistance;
                }

                foreach (var neighbour in currentRoom.Neighbours)
                {
                    if (!_visited.Contains(neighbour))
                    {
                        _visited.Add(neighbour);
                        _distances[neighbour] = currentDistance + 1;
                        _queue.Enqueue(neighbour);
                    }
                }
            }

            return null;
        }
        
       
    }
}