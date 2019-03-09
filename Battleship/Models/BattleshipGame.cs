using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace Battleship.Models
{
    public class BattleshipGame
    {
        private Player player1;
        private Player player2;

        public GAME_STATES gameState;
        public Player winner;


        public BattleshipGame()
        {
            gameState = GAME_STATES.WAITING_FOR_PLAYERS;

        }

        public Player JoinGame(string name)
        {
            if (player1 == null)
            {
                player1 = new Player(name);

                return player1;
            }
            else if (player2 == null)
            {
                player2 = new Player(name);

                gameState = GAME_STATES.PLACING_SHIPS;

                return player2;
            }

            return null;
        }

        public void startGame()
        {
            WebApiApplication.BattleshipGameObj.gameState = GAME_STATES.PLAYER1_TURN;
        }

        public Player GetPlayer(string playerName)
        {
            if (player1 != null && playerName == player1.Name)
            {
                return player1;
            }
            else if (player2 != null && playerName == player2.Name)
            {
                return player2;
            }

            return null;
        }

        public Player GetOpponent(string playerName)
        {
            if (player1 != null && playerName == player1.Name)
            {
                return player2;
            }
            else if (player2 != null &&  playerName == player2.Name)
            {
                return player1;
            }

            return null;
        }

        public GRID_SHOT TakeShot(string playerName, Coordinate shotLocation)
        {
            Player shooter = GetActivePlayer();
            Player opponent = GetOpponent(shooter.Name);
            GRID_SHOT shotResult;

            if (shooter.Name == playerName)
            {
                //If There is no shot here already
                if (opponent.PlayerBoard.playerGrid[shotLocation.BoardX, shotLocation.BoardY].shot == GRID_SHOT.CLEAR)
                {
                    //If there's a ship here - mark a hit
                    if (opponent.PlayerBoard.playerGrid[shotLocation.BoardX, shotLocation.BoardY].ship != null)
                    {
                        opponent.PlayerBoard.playerGrid[shotLocation.BoardX, shotLocation.BoardY].ship.hits.Add(shotLocation);
                        opponent.PlayerBoard.playerGrid[shotLocation.BoardX, shotLocation.BoardY].shot = GRID_SHOT.HIT;
                        shotResult = GRID_SHOT.HIT;

                        //Check if ship is sunk
                        if (opponent.PlayerBoard.playerGrid[shotLocation.BoardX, shotLocation.BoardY].ship.hits.Count ==
                            opponent.PlayerBoard.playerGrid[shotLocation.BoardX, shotLocation.BoardY].ship.length)
                        {
                            opponent.SunkShips.Add(opponent.PlayerBoard.playerGrid[shotLocation.BoardX, shotLocation.BoardY].ship);

                            winner = CheckGameOver();
                        }
                    }
                    else //No ship, mark it a miss
                    {
                        opponent.PlayerBoard.playerGrid[shotLocation.BoardX, shotLocation.BoardY].shot = GRID_SHOT.MISS;
                        shotResult = GRID_SHOT.MISS;
                    }

                    TogglePlayerTurn();
                    return shotResult;
                }
                else //Already a shot here.
                {
                    return GRID_SHOT.ALREADY_FIRED;
                }
            }
            else //Not your turn
            {
                return GRID_SHOT.NOT_YOUR_TURN;
            }
        }

        public Ship PlaceShip(string playerName, Ship ship, Coordinate location)
        {
            Player p = GetPlayer(playerName);
            Ship nextShip = p.PlaceShip(ship,location);

            CheckReadyToStart();

            return nextShip;
        }

        public Player GetActivePlayer()
        {
            if (gameState == GAME_STATES.PLAYER1_TURN)
            {
                return player1;
            }
            else if (gameState == GAME_STATES.PLAYER2_TURN)
            {
                return player2;
            }

            return null;
        }

        public GAME_STATES TogglePlayerTurn(){
            if(gameState == GAME_STATES.PLAYER1_TURN)
            {
                gameState = GAME_STATES.PLAYER2_TURN;
            }
            else if (gameState == GAME_STATES.PLAYER2_TURN)
            {
                gameState = GAME_STATES.PLAYER1_TURN;
            }

            return gameState;
        }

        public Player CheckGameOver()
        {
            //ALL PLAYER 1 SHIPS SUNK, PLAYER 2 WINS
            if (player1.SunkShips.Count == 5 )
            {
                gameState = GAME_STATES.GAME_OVER;
                return player2;
            }

            //ALL PLAYER 2 SHIPS SUNK, PLAYER 1 WINS
            if (player2.SunkShips.Count == 5)
            {
                gameState = GAME_STATES.GAME_OVER;
                return player1;
            }

            //No Winner Yet
            return null;
        }

        public bool CheckReadyToStart()
        {
            if (player1.ShipsToPlace.Count == 0 && player2.ShipsToPlace.Count == 0)
            {
                startGame();
                return true;
            }
            else
            {
                return false;
            }
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
            for (int i = 0; i < s.length; i++)
            {
                if (s.orientation == SHIP_DIRECTION.HORIZONTAL)
                {

                    if (playerGrid[s.startCoordinate.BoardX, s.startCoordinate.BoardY + i].ship == null)
                    {
                        shipSquares.Add(playerGrid[s.startCoordinate.BoardX, s.startCoordinate.BoardY + i]);
                    }
                    else
                    {
                        return false;
                    }

                  
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

                    
                }

            }

            foreach (GameSquare g in shipSquares)
            {
                g.ship = s;
            }




            return true;
        }


    }

    public class Coordinate
    {
        public char X;
        public int Y;

        public int BoardX { get { return ((int)X) - ((int)'A'); } }
        public int BoardY { get { return Y - 1; } }
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

    public class Player
    {
        public string Name;
        public Board PlayerBoard;

        public List<Ship> ShipsToPlace;
        public List<Ship> SunkShips;

        public Player(string name)
        {
            Name = name;
            PlayerBoard = new Board();
            ShipsToPlace = new List<Ship>();

            ShipsToPlace.Add(new Ship { name = "Carrier", length = 5, orientation = SHIP_DIRECTION.HORIZONTAL });
            ShipsToPlace.Add(new Ship { name = "Battleship", length = 4, orientation = SHIP_DIRECTION.HORIZONTAL });
            ShipsToPlace.Add(new Ship { name = "Cruiser", length = 3, orientation = SHIP_DIRECTION.HORIZONTAL });
            ShipsToPlace.Add(new Ship { name = "Submarine", length = 3, orientation = SHIP_DIRECTION.HORIZONTAL });
            ShipsToPlace.Add(new Ship { name = "Destroyer", length = 2, orientation = SHIP_DIRECTION.HORIZONTAL });

            SunkShips = new List<Ship>();

        }

        public Ship PlaceShip(Ship ship, Coordinate location)
        {
            bool shipPlaced = false;


            shipPlaced = PlayerBoard.placeShip(ship, location);
            if (shipPlaced)
            {
                ShipsToPlace.Remove(ShipsToPlace.Last());
            }

            //returns next ship to place
            if (ShipsToPlace.Count > 0)
            {
                return ShipsToPlace.Last();

            }

            return null;
        }

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
        HIT,
        ALREADY_FIRED,
        NOT_YOUR_TURN
    }

    public class OpponentBoard{
        public PrivateGameSquare[,] playerGrid;
    }

    public class PrivateGameSquare
    {
        public Coordinate coordinates;
        public GRID_SHOT shot;
    }
}