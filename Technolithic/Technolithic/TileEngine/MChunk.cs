using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MChunk
    {
        public const int CHUNK_SIZE = 8;

        public List<Room> Rooms { get; private set; }
        public List<MChunk> Neighbours { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public MChunk(int x, int y)
        {
            X = x;
            Y = y;

            Rooms = new List<Room>() { new Room(0) };
            Neighbours = new List<MChunk>();
        }

        public void AddTile(Tile tile)
        {
            tile.Chunk = this;
            Rooms[0].AddTile(tile);
        }

        public void AddNeighbour(MChunk chunk)
        {
            Rooms[0].Neighbours.Add(chunk.Rooms[0]);
            Neighbours.Add(chunk);
        }

        public void UpdateTile(Tile tile)
        {
            if (tile.IsWalkable)
                ConnectRooms(tile);
            else
                SplitRoom(tile);

            ProcessRooms();
        }

        private void RemoveEmptyRooms()
        {
            for (int i = 0; i < Rooms.Count; i++)
            {
                if (Rooms[i].TilesCount == 0)
                {
                    Rooms.Remove(Rooms[i]);
                    --i;
                }
            }
        }

        private void SplitRoom(Tile tile)
        {
            tile.Room.RemoveTile(tile);
            tile.Room = null;

            // Процесс удаления комнат
            foreach (Tile t1 in tile.GetNeighbourTiles())
            {
                if (t1.Room != null && t1.Chunk == this)
                {
                    foreach (Room r1 in t1.Room.Neighbours)
                    {
                        r1.Neighbours.Remove(t1.Room);
                    }
                }
            }

            List<Room> newRooms = new List<Room>();

            // Процесс создания комнаты
            foreach (Tile t1 in tile.GetNeighbourTiles())
            {
                if (t1.Room != null && t1.Chunk == this)
                {
                    // Проверяем не был ли тайл уже захвачен другой комнатой
                    if (!Contains(t1, newRooms))
                    {
                        // Cоздаем новую комнату для одной стороны света внутри текущего чанка
                        Room r2 = new Room(IdGenerator.Generate());
                        newRooms.Add(r2);
                        ProcessTile(t1, r2);
                    }
                }
            }

            // Добавляем комнату в общий список комнат чанков
            Rooms.AddRange(newRooms);

            // Удаляем комнату в которых нет тайлов
            RemoveEmptyRooms();
        }

        private void ConnectRooms(Tile tile)
        {
            // Создаем новую комнату и добавляем в нее тайл
            tile.Room = new Room(IdGenerator.Generate());
            tile.Room.AddTile(tile);

            // Процесс удаления комнат
            foreach (Tile t1 in tile.GetNeighbourTiles())
            {
                if (t1.Room != null && t1.Chunk == this)
                {
                    foreach (Room r1 in t1.Room.Neighbours)
                    {
                        r1.Neighbours.Remove(t1.Room);
                    }
                }
            }

            foreach (Tile t1 in tile.GetNeighbourTiles())
            {
                if (t1.Room != null)
                {
                    if (t1.Chunk == this)
                    {
                        if (t1.Room.ZoneId != tile.Room.ZoneId)
                            ProcessTile(t1, tile.Room);
                    }
                    else
                    {
                        t1.Room.Neighbours.Add(tile.Room);
                        tile.Room.Neighbours.Add(t1.Room);
                    }
                }
            }

            // Добавляем комнату в общий список комнат чанков
            Rooms.Add(tile.Room);

            // Удаляем комнату в которых нет тайлов
            RemoveEmptyRooms();
        }

        private void ProcessTile(Tile tile, Room room)
        {
            // Добавляем в комнату захваченные тайлы параллельно собирая соседей для нее
            Queue<Tile> queue = new Queue<Tile>();
            queue.Enqueue(tile);

            while (queue.Count > 0)
            {
                Tile t1 = queue.Dequeue();
                t1.Room.RemoveTile(t1);
                room.AddTile(t1);

                foreach (Tile t2 in t1.GetNeighbourTiles())
                {
                    if (t2.Room != null)
                    {
                        if (t2.Chunk == this)
                        {
                            // Если еще не состоит в новой комнате
                            if (t2.Room != room)
                                queue.Enqueue(t2);
                        }
                        else
                        {
                            // добавляемся в соседи к новой комнате из другого чанка
                            if (!t2.Room.Neighbours.Contains(room))
                                t2.Room.Neighbours.Add(room);

                            // добавляем в соседи комнату с другого чанка
                            if (!room.Neighbours.Contains(t2.Room))
                                room.Neighbours.Add(t2.Room);
                        }
                    }
                }
            }
        }

        private void ProcessRooms()
        {
            foreach (Room room in Rooms)
            {
                if (!Contains(room, Rooms))
                {
                    Queue<Room> queue = new Queue<Room>();
                    queue.Enqueue(room);

                    while (queue.Count > 0)
                    {
                        Room r1 = queue.Dequeue();

                        foreach (Room r2 in r1.Neighbours)
                        {
                            if (r2.ZoneId != r1.ZoneId)
                            {
                                r2.ZoneId = r1.ZoneId;
                                queue.Enqueue(r2);
                            }
                        }
                    }
                }
            }
        }

        private bool Contains(Room room, List<Room> rs)
        {
            foreach (Room r in rs)
            {
                if (room != r && room.ZoneId == r.ZoneId)
                    return true;
            }
            return false;
        }

        private bool Contains(Tile tile, List<Room> rs)
        {
            foreach (Room r in rs)
            {
                if (tile.Room.ZoneId == r.ZoneId)
                    return true;
            }
            return false;
        }

    }
}
