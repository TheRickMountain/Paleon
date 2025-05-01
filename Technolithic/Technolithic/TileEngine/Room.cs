using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Room
    {

        public int Id { get; set; }

        public List<Tile> Tiles { get; private set; }

        public List<Room> Neighbours { get; private set; }

        public int TilesCount
        {
            get { return Tiles.Count; }
        }

        public Room(int id)
        {
            Id = id;
            Tiles = new List<Tile>();
            Neighbours = new List<Room>();
        }

        public void AddTile(Tile tile)
        {
            Tiles.Add(tile);
            tile.Room = this;
        }

        public void RemoveTile(Tile tile)
        {
            Tiles.Remove(tile);
            tile.Room = null;
        }

        public Tile GetRandomTile()
        {
            if(Tiles.Count == 0) return null;

            return Tiles[MyRandom.Range(0, Tiles.Count)];
        }

    }
}
