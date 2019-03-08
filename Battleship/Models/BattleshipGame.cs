using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace Battleship.Models
{
    public class BattleshipGame
    {
        public string player1Name;
        public string player2Name;

        public Board player1Grid;
        public Board player2Grid;

        public List<Ship> p1Ships;
        public List<Ship> p2Ships;

        public List<Ship> p1SunkShips;
        public List<Ship> p2SunkShips;

        public GAME_STATES gameState;
        

        public BattleshipGame()
        {
            gameState = GAME_STATES.WAITING_FOR_PLAYERS;

            player1Grid = new Board();
            player2Grid = new Board();

            p1Ships = new List<Ship>();
            p2Ships = new List<Ship>();

            p1SunkShips = new List<Ship>();
            p2SunkShips = new List<Ship>();

            p1Ships.Add(new Ship { name = "Carrier", length = 5, orientation = SHIP_DIRECTION.HORIZONTAL });
            p1Ships.Add(new Ship { name = "Battleship", length = 4, orientation = SHIP_DIRECTION.HORIZONTAL });
            p1Ships.Add(new Ship { name = "Cruiser", length = 3, orientation = SHIP_DIRECTION.HORIZONTAL });
            p1Ships.Add(new Ship { name = "Submarine", length = 3, orientation = SHIP_DIRECTION.HORIZONTAL });
            p1Ships.Add(new Ship { name = "Destroyer", length = 2, orientation = SHIP_DIRECTION.HORIZONTAL });

            p2Ships.Add(new Ship { name = "Carrier", length = 5, orientation = SHIP_DIRECTION.HORIZONTAL });
            p2Ships.Add(new Ship { name = "Battleship", length = 4, orientation = SHIP_DIRECTION.HORIZONTAL });
            p2Ships.Add(new Ship { name = "Cruiser", length = 3, orientation = SHIP_DIRECTION.HORIZONTAL });
            p2Ships.Add(new Ship { name = "Submarine", length = 3, orientation = SHIP_DIRECTION.HORIZONTAL });
            p2Ships.Add(new Ship { name = "Destroyer", length = 2, orientation = SHIP_DIRECTION.HORIZONTAL });

        }

        public void startGame()
        {
            WebApiApplication.BattleshipGameObj.gameState = GAME_STATES.PLAYER1_TURN;

        }
    }

    public class Board
    {
        public GameSquare[,] playerGrid;

        public Board()
        {
            playerGrid = new GameSquare[10, 10];

            for (char x = 'A'; x <= 'J'; x++)
            {

                for (int y = 1; y <= 10; y++)
                {
                    string id = "P2" + x.ToString() + y.ToString();

                    Coordinate c = new Coordinate { X = x, Y = y };

                    playerGrid[c.BoardX, c.BoardY] = new GameSquare { coordinates = c };
                }
            }
        }


        public bool placeShip(Ship s, Coordinate c)
        {
            List<GameSquare> shipSquares = new List<GameSquare>();

            s.startCoordinate = c;
            for(int i = 0; i < s.length; i++)
            {
                if(s.orientation == SHIP_DIRECTION.HORIZONTAL)
                {

                    if(playerGrid[s.startCoordinate.BoardX, s.startCoordinate.BoardY + i].ship == null)
                    {
                        shipSquares.Add(playerGrid[s.startCoordinate.BoardX, s.startCoordinate.BoardY + i]);
                    }
                    else
                    {
                        return false;
                    }

                    //playerGrid[s.startCoordinate.BoardX, s.startCoordinate.BoardY+i].ship = s;
                    //playerGrid[s.startCoordinate.BoardX+i, s.startCoordinate.BoardY] = new GameSquare { coordinates = s.startCoordinate, ship = s, shot = false };
                }
                else if (s.orientation == SHIP_DIRECTION.VERTICAL)
                {
                    if (playerGrid[s.startCoordinate.BoardX + i, s.startCoordinate.BoardY].ship == null)
                    {
                        shipSquares.Add(playerGrid[s.startCoordinate.BoardX + i, s.startCoordinate.BoardY]);
   
                    }
                    else
                    {
                        return false;
                    }
                    
                    //playerGrid[s.startCoordinate.BoardX+i, s.startCoordinate.BoardY].ship = s;
                    //playerGrid[s.startCoordinate.BoardX, s.startCoordinate.BoardY+i] = new GameSquare { coordinates = s.startCoordinate, ship = s, shot = false };
                }
                
            }

            foreach(GameSquare g in shipSquares)
            {
                g.ship = s;
            }


            if(WebApiApplication.BattleshipGameObj.p1Ships.Count == 0 && WebApiApplication.BattleshipGameObj.p2Ships.Count == 0)
            {
                WebApiApplication.BattleshipGameObj.startGame();
            }

            return true;
        }

    } 

    public class Coordinate
    {
        public char X;
        public int Y;

        public int BoardX { get { return ((int)X) - ((int)'A'); } }
        public int BoardY { get { return Y-1; } }
    }

    public class GameSquare {

        public Coordinate coordinates;
        public Ship ship;
        public GRID_SHOT shot;
    }

    public class Ship {

        public string name;
        public int length;
        public Coordinate startCoordinate;

        public SHIP_DIRECTION orientation;
        public List<Coordinate> hits = new List<Coordinate>();
    }

    public enum SHIP_DIRECTION
    {
        HORIZONTAL,
        VERTICAL
    }

    public enum GAME_STATES
    {
        WAITING_FOR_PLAYERS,
        PLACING_SHIPS,
        PLAYER1_TURN,
        PLAYER2_TURN,
        GAME_OVER
    }

    public enum GRID_SHOT
    {
        CLEAR,
        MISS,
        HIT
    }
}