using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildingManager
    {

        public bool CheckTileByGroundPattern(BuildingTemplate buildingTemplate, char[,] groundPatternMatrix, Tile mouseTile, Tile checkTile)
        {
            if (mouseTile == null)
                return false;

            if (checkTile == null)
                return false;

            char groundPatternId = groundPatternMatrix[checkTile.X - mouseTile.X, checkTile.Y - mouseTile.Y];

            switch (buildingTemplate.BuildingType)
            {
                default:
                    {
                        if (checkTile == null)
                        {
                            return false;
                        }

                        if (checkTile.Entity != null)
                        {
                            return false;
                        }

                        if (CheckGroundPatternId(groundPatternId, checkTile) == false)
                        {
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        private bool CheckGroundPatternId(char groundPatternId, Tile checkTile)
        {
            switch(groundPatternId)
            {
                case 'A':
                    {
                        if (checkTile.GroundTopType == GroundTopType.None)
                        {
                            return true;
                        }
                    }
                    break;
                case 'B':
                    {
                        if(checkTile.GroundTopType == GroundTopType.Water)
                        {
                            if(checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case 'C':
                    {
                        if (checkTile.GroundTopType == GroundTopType.Clay ||
                               checkTile.GroundTopType == GroundTopType.Copper ||
                               checkTile.GroundTopType == GroundTopType.Iron ||
                               checkTile.GroundTopType == GroundTopType.Stone ||
                               checkTile.GroundTopType == GroundTopType.Tin)
                        {
                            return true;
                        }
                    }
                    break;
                case 'D':
                    {
                        if (checkTile.GroundTopType == GroundTopType.IrrigationCanalEmpty ||
                        checkTile.GroundTopType == GroundTopType.IrrigationCanalFull)
                        {
                            if (checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case 'G':
                    {
                        if (checkTile.GroundTopType == GroundTopType.None)
                        {
                            return true;
                        }
                        else if (checkTile.GroundTopType == GroundTopType.Water || checkTile.GroundTopType == GroundTopType.DeepWater)
                        {
                            if(checkTile.SurfaceId != -1)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case 'H':
                    {
                        if (checkTile.GroundTopType == GroundTopType.Water || checkTile.GroundTopType == GroundTopType.DeepWater)
                        {
                            if (checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case 'I':
                    {
                        if (checkTile.GroundTopType == GroundTopType.Water
                            || checkTile.GroundTopType == GroundTopType.DeepWater
                            || checkTile.GroundTopType == GroundTopType.IrrigationCanalEmpty
                            || checkTile.GroundTopType == GroundTopType.IrrigationCanalFull)
                        {
                            if (checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case 'J':
                    {
                        if (checkTile.GroundTopType == GroundTopType.None)
                        {
                            if (checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case 'K':
                    {
                        if(checkTile.GroundTopType == GroundTopType.None)
                        {
                            if(checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                        else if(checkTile.GroundTopType == GroundTopType.Water)
                        {
                            if(checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case 'L':
                    {
                        if (checkTile.GroundTopType == GroundTopType.None)
                        {
                            if (checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                        else if (checkTile.GroundTopType == GroundTopType.IrrigationCanalEmpty ||
                        checkTile.GroundTopType == GroundTopType.IrrigationCanalFull)
                        {
                            if (checkTile.SurfaceId == -1)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case 'M':
                    return checkTile.SurfaceId != -1;
            }

            return false;
        }

    }
}
