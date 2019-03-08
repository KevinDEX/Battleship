using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Battleship.Models;
using System.Web.Http.Results;

namespace Battleship.api
{
    [RoutePrefix("api/BattleshipGame")]
    public class BattleshipGameController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("JoinGame")]
        public IHttpActionResult JoinGame([FromBody]string playerName)
        {
            BattleshipGame gameObj = WebApiApplication.BattleshipGameObj;

            if (String.IsNullOrEmpty(gameObj.player1Name))
            {
                gameObj.player1Name = playerName;
                return new OkResult(this.Request);
            }

            else if (String.IsNullOrEmpty(gameObj.player2Name))
            {
                gameObj.player2Name = playerName;

                WebApiApplication.BattleshipGameObj.gameState = GAME_STATES.PLACING_SHIPS;

                return new OkResult(this.Request);
            }

            return new OkResult(this.Request);
        }

        [HttpPost]
       [Route("PlaceShip")]
        public Ship PlaceShip(ShipPlacementParams args)
        {
            BattleshipGame gameObj = WebApiApplication.BattleshipGameObj;
            bool shipPlaced = false;
            
            if(args.player == gameObj.player1Name)
            {
                shipPlaced = gameObj.player1Grid.placeShip(args.ship, args.location);
                if(shipPlaced)
                {
                    gameObj.p1Ships.Remove(gameObj.p1Ships.Last());
                }

                //returns next ship to place
                if (gameObj.p1Ships.Count > 0)
                {
                    return gameObj.p1Ships.Last();
                }
            }

            if (args.player == gameObj.player2Name)
            {
                shipPlaced = gameObj.player2Grid.placeShip(args.ship, args.location);
                if (shipPlaced)
                {
                    gameObj.p2Ships.Remove(gameObj.p2Ships.Last());
                }
 

                //returns next ship to place
                if (gameObj.p2Ships.Count > 0)
                {
                    return gameObj.p2Ships.Last();
                }
            }

            if (WebApiApplication.BattleshipGameObj.p1Ships.Count == 0 && WebApiApplication.BattleshipGameObj.p2Ships.Count == 0)
            {
                WebApiApplication.BattleshipGameObj.startGame();
            }


            return null;
        }

        [HttpPost]
        [Route("GetNextShipToPlace")]
        public Ship GetNextShipToPlace([FromBody]string playerName)
        {
            BattleshipGame gameObj = WebApiApplication.BattleshipGameObj;


            if(playerName == gameObj.player1Name)
            {
                if (gameObj.p1Ships.Count > 0)
                {
                    return gameObj.p1Ships.Last();
                }

            }

            if (playerName == gameObj.player2Name)
            {
                if (gameObj.p2Ships.Count > 0)
                {
                    return gameObj.p2Ships.Last();
                }

            }

            return null;
        }

        [HttpPost]
        [Route("GetMyBoard")]
        public Board GetMyBoard([FromBody]string playerName)
        {
            BattleshipGame gameObj = WebApiApplication.BattleshipGameObj;


            if (playerName == gameObj.player1Name)
            {
                //var resp = new OkResult()
                return gameObj.player1Grid;
            }
            if (playerName == gameObj.player2Name)
            {
                //var resp = new OkResult()
                return gameObj.player2Grid;
            }


            return null;
        }

        [HttpPost]
        [Route("GetOpponentBoard")]
        public Board GetOpponentBoard([FromBody]string playerName)
        {
            BattleshipGame gameObj = WebApiApplication.BattleshipGameObj;


            if (playerName == gameObj.player1Name)
            {
                //var resp = new OkResult()
                return gameObj.player2Grid;
            }
            if (playerName == gameObj.player2Name)
            {
                //var resp = new OkResult()
                return gameObj.player1Grid;
            }


            return null;
        }


        [HttpPost]
        [Route("TakeShot")]
        public IHttpActionResult TakeShot(TakeShotParams args)
        {
            BattleshipGame gameObj = WebApiApplication.BattleshipGameObj;

            if(args.playerShooting == gameObj.player1Name)
            {
                if(gameObj.gameState == GAME_STATES.PLAYER1_TURN && 
                    gameObj.player2Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].shot == GRID_SHOT.CLEAR)
                {

                    if (gameObj.player2Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship != null)
                    {
                        gameObj.player2Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship.hits.Add(args.shotCoordinate);
                        gameObj.player2Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].shot = GRID_SHOT.HIT;

                        if(gameObj.player2Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship.hits.Count ==
                            gameObj.player2Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship.length)
                        {
                            gameObj.p2SunkShips.Add(gameObj.player2Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship);
                        }
                    }
                    else
                    {
                        gameObj.player2Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].shot = GRID_SHOT.MISS;
                    }
                    gameObj.gameState = GAME_STATES.PLAYER2_TURN;
                }
            }
            else if (args.playerShooting == gameObj.player2Name)
            {
                if (gameObj.gameState == GAME_STATES.PLAYER2_TURN &&
                    gameObj.player1Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].shot == GRID_SHOT.CLEAR)
                {

                    if (gameObj.player1Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship != null)
                    {
                        gameObj.player1Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship.hits.Add(args.shotCoordinate);
                        gameObj.player1Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].shot = GRID_SHOT.HIT;
                        if (gameObj.player1Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship.hits.Count ==
                           gameObj.player1Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship.length)
                        {
                            gameObj.p1SunkShips.Add(gameObj.player1Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].ship);
                        }
                    }
                    else
                    {
                        gameObj.player1Grid.playerGrid[args.shotCoordinate.BoardX, args.shotCoordinate.BoardY].shot = GRID_SHOT.MISS;
                    }
                    gameObj.gameState = GAME_STATES.PLAYER1_TURN;
                }

            }

            if(gameObj.p1SunkShips.Count == 5 || gameObj.p2SunkShips.Count == 5)
            {
                gameObj.gameState = GAME_STATES.GAME_OVER;
            }
            return new OkResult(this.Request);
        }

        [AcceptVerbs("GET")]
        [Route("GameState")]
        public string GameState()
        {
            return Enum.GetName(typeof(GAME_STATES),WebApiApplication.BattleshipGameObj.gameState);
        }

        [AcceptVerbs("GET")]
        [Route("GameObj")]
        public BattleshipGame GameObj()
        {
            return WebApiApplication.BattleshipGameObj;
        }
    }

    public class ShipPlacementParams
    {
        public string player;
        public Ship ship;
        public Coordinate location;
    }

    public class TakeShotParams
    {
        public string playerShooting;
        public Coordinate shotCoordinate;
    }
}