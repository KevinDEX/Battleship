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

        string currentPlayer;
        string otherPlayer;

        List<Ship> myShipsToPlace;

        public string lbl_currentPlayer { get { return PrintPlayerName(currentPlayer); } }
        public string lbl_opponent { get { return PrintPlayerName(otherPlayer); } }


        public ActionResult Index()
        {

            ViewBag.Title = "Battleship Game";

            if (Request.Cookies["playerName"] != null)
            {
                if (Request.Cookies["playerName"].Value == gameObj.player1Name)
                {
                    currentPlayer = gameObj.player1Name;
                    otherPlayer = gameObj.player2Name;
                    myShipsToPlace = gameObj.p1Ships;
                }
                else if (Request.Cookies["playerName"].Value == gameObj.player2Name)
                {
                    currentPlayer = gameObj.player2Name;
                    otherPlayer = gameObj.player1Name;
                    myShipsToPlace = gameObj.p2Ships;
                }
                else
                {
                    Request.Cookies.Remove("playerName");
                    otherPlayer = gameObj.player1Name;
                }
            }
            else
            {
                otherPlayer = gameObj.player1Name;
            }

            ViewData["lbl_currentPlayer"] = lbl_currentPlayer;
            ViewData["lbl_opponent"] = lbl_opponent;

            return View(WebApiApplication.BattleshipGameObj);
        }

        private string PrintPlayerName(string playerName)
        {
            if (String.IsNullOrEmpty(playerName))
            {
                return "waiting for opponent to join";
            }
            else
            {
                return playerName;
            }
        }
    }
}
