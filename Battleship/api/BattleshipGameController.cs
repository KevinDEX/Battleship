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
        BattleshipGame gameObj = WebApiApplication.BattleshipGameObj;

        [HttpPost]
        [Route("JoinGame")]
        public Player JoinGame([FromBody]string playerName)
        {

            return gameObj.JoinGame(playerName);

        }

        [HttpPost]
       [Route("PlaceShip")]
        public Ship PlaceShip(ShipPlacementParams args)
        {

            return gameObj.PlaceShip(args.player, args.ship, args.location);

        }

        [HttpPost]
        [Route("GetNextShipToPlace")]
        public Ship GetNextShipToPlace([FromBody]string playerName)
        {
            Player p = gameObj.GetPlayer(playerName);
            if(p != null)
            {
                if (p.ShipsToPlace.Count > 0)
                {
                    return p.ShipsToPlace.Last();
                }
            }

            return null;
        }

        [HttpPost]
        [Route("GetMyBoard")]
        public Board GetMyBoard([FromBody]string playerName)
        {
            Player p = gameObj.GetPlayer(playerName);

            if(p != null)
            {
                return p.PlayerBoard;
            }

            return null;
        }

        [HttpPost]
        [Route("GetOpponentBoard")]
        public Board GetOpponentBoard([FromBody]string playerName)
        {
            Player p = gameObj.GetOpponent(playerName);

            if (p != null)
            {
                return p.PlayerBoard;
            }

            return null;

        }

        [HttpPost]
        [Route("GetPlayer")]
        public Player GetPlayer([FromBody]string playerName)
        {
           return gameObj.GetPlayer(playerName);
        }

        [HttpPost]
        [Route("GetOpponent")]
        public Player GetOpponent([FromBody]string playerName)
        {
            return gameObj.GetOpponent(playerName);
        }


        [HttpPost]
        [Route("TakeShot")]
        public GRID_SHOT TakeShot(TakeShotParams args)
        {
            return gameObj.TakeShot(args.playerShooting, args.shotCoordinate);

        }

        [AcceptVerbs("GET")]
        [Route("GameState")]
        public string GameState()
        {
            return Enum.GetName(typeof(GAME_STATES), gameObj.gameState);
        }

        [AcceptVerbs("GET")]
        [Route("GameObj")]
        public BattleshipGame GameObj()
        {
            return gameObj;
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