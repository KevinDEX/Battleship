using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Battleship.Models;

namespace Battleship.Controllers
{
    public class BattleshipGameController : Controller
    {

        BattleshipGame gameObj = WebApiApplication.BattleshipGameObj;

        Player currentPlayer;
        Player opponentPlayer;

        List<Ship> myShipsToPlace;

        public string lbl_currentPlayer { get { return PrintPlayerName(currentPlayer); } }
        public string lbl_opponent { get { return PrintPlayerName(opponentPlayer); } }


        public ActionResult Index()
        {

            ViewBag.Title = "Battleship Game";

            if (Request.Cookies["playerName"] != null)
            {
                string playerName = Request.Cookies["playerName"].Value;
                currentPlayer = gameObj.GetPlayer(playerName);
                opponentPlayer = gameObj.GetOpponent(playerName);
            }

            ViewData["lbl_currentPlayer"] = lbl_currentPlayer;
            ViewData["lbl_opponent"] = lbl_opponent;

            return View(WebApiApplication.BattleshipGameObj);
        }

        private string PrintPlayerName(Player player)
        {
            if (player == null)
            {
                return "waiting for player to join";
            }
            else
            {
                return player.Name;
            }
        }
    }
}
