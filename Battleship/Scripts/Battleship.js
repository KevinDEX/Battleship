

var selectedShip;
  //  = { name: "Carrier", "length": 5, "startCoordinate": {}, "orientation":"HORIZONTAL" };

var mouseCoordinates = { "X": "A", "X": 1 };


var shipsToPlace = [];

var username = getCookie("playername");
var myBoard;
var opponentBoard;
var gameState;

var fullGameState;

$(document).ready(function () {

    initPieces();
    refreshBoard();

    window.setInterval(function () {
        updateGameState();
        updateGameObj();
        refreshBoard();
       drawOpponentBoard();
    }, 2000);
});


function highlightShip(x, y) {


   // $("td[id^=P2]").css("background-color", "white");
    drawBoard(myBoard);
    
        for (s = 0; s < selectedShip.length; s++) {
            if (selectedShip.orientation == 0) {
                if (parseInt(y) >= 1 && parseInt(y) + selectedShip.length-1 <= 10) {
                    $('#P2' + x + (parseInt(y) + s).toString()).css('background-color', "darkgray");
                }
            }
            else if (selectedShip.orientation == 1) {
                if (x >= 'A' && String.fromCharCode(x.charCodeAt(0) + selectedShip.length-1) <= 'J') {
                    $('#P2' + String.fromCharCode(x.charCodeAt(0)+s) + y).css('background-color', "darkgray");
                }
            }
    }

    mouseCoordinates = { "X": x, "Y": y };
}

function rotateShip() {
    if (selectedShip.orientation == 0) {
        selectedShip.orientation = 1;
    }
    else if (selectedShip.orientation == 1) {
        selectedShip.orientation = 0;
    }
    highlightShip(mouseCoordinates.X, mouseCoordinates.Y);
}

function placeShip() {

    selectedShip.startCoordinate = mouseCoordinates;

    var args = { "player": username, "ship": selectedShip, "location": mouseCoordinates };

    $.ajax({
        contentType:"application/json",
        url: "/api/BattleshipGame/PlaceShip",
        data: JSON.stringify(args),
        type:"POST",
        success: function (result) {
            selectedShip = result;
            refreshBoard();
        }
    });
}

function takeShot(username,x,y) {


    var shot = {"playerShooting": username, "shotCoordinate": {"X":x,"Y":y} };

    $.ajax({
        contentType: "application/json",
        url: "/api/BattleshipGame/TakeShot",
        data: JSON.stringify(shot),
        type: "POST",
        success: function (result) {
            drawOpponentBoard();
        }
    });
}

function setPlayerName() {
    $.ajax({
        contentType: "application/json",
        url: "/api/BattleshipGame/JoinGame",
        data: JSON.stringify($("#input_playerName").val()),
        type: "POST",
        success: function (result) {
            document.cookie = "playername=" + $("#input_playerName").val();
            location.reload();
           // $("#lbl_PlayerName").text = $("#input_playerName").val();

        }
    });
}

document.addEventListener('keyup', function (e) {

    if (e.keyCode == 32) {
        rotateShip();
    }
});

function getCookie(name) {
    var match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    if (match) return match[2];
}

function initPieces() {
    if (username) {
        $.ajax({
            contentType: "application/json",
            url: "/api/BattleshipGame/GetNextShipToPlace",
            data: JSON.stringify(username),
            type: "POST",
            success: function (result) {
                selectedShip = result;
            }
        });
    }
}

function refreshBoard() {

    updateGameState();

    $.ajax({
        contentType: "application/json",
        url: "/api/BattleshipGame/GetMyBoard",
        data: JSON.stringify(username),
        type: "POST",
        success: function (result) {
            myBoard = result;
            drawBoard(myBoard);
            /*for (x = 0; x < myBoard.playerGrid.length; x++) {
                for (y = 0; y < myBoard.playerGrid[x].length; y++) {
                    if (myBoard.playerGrid[x][y].ship != null) {
                        if (myBoard.playerGrid[x][y].shot == "2") {
                            $("#P2" + myBoard.playerGrid[x][y].coordinates.X + myBoard.playerGrid[x][y].coordinates.Y).css('background-color', "red");
                        }
                        else {
                            $("#P2" + myBoard.playerGrid[x][y].coordinates.X + myBoard.playerGrid[x][y].coordinates.Y).css('background-color', "black");
                        }

                    }
                    else if (myBoard.playerGrid[x][y].shot == "0") {
                        $("#P2" + myBoard.playerGrid[x][y].coordinates.X + myBoard.playerGrid[x][y].coordinates.Y).css('background-color', "blue");
                    }
                    else if (myBoard.playerGrid[x][y].shot == "1"){
                        $("#P2" + myBoard.playerGrid[x][y].coordinates.X + myBoard.playerGrid[x][y].coordinates.Y).css('background-color', "white");
                    }

                }
            }*/
        }
    });

}

function drawBoard(myBoard) {
    for (x = 0; x < myBoard.playerGrid.length; x++) {
        for (y = 0; y < myBoard.playerGrid[x].length; y++) {
            if (myBoard.playerGrid[x][y].ship != null) {
                if (myBoard.playerGrid[x][y].shot == "2") {
                    $("#P2" + myBoard.playerGrid[x][y].coordinates.X + myBoard.playerGrid[x][y].coordinates.Y).css('background-color', "red");
                }
                else {
                    $("#P2" + myBoard.playerGrid[x][y].coordinates.X + myBoard.playerGrid[x][y].coordinates.Y).css('background-color', "black");
                }

            }
            else if (myBoard.playerGrid[x][y].shot == "0") {
                $("#P2" + myBoard.playerGrid[x][y].coordinates.X + myBoard.playerGrid[x][y].coordinates.Y).css('background-color', "blue");
            }
            else if (myBoard.playerGrid[x][y].shot == "1") {
                $("#P2" + myBoard.playerGrid[x][y].coordinates.X + myBoard.playerGrid[x][y].coordinates.Y).css('background-color', "white");
            }

        }
    }
}

function drawOpponentBoard() {

    updateGameState();

    $.ajax({
        contentType: "application/json",
        url: "/api/BattleshipGame/GetOpponentBoard",
        data: JSON.stringify(username),
        type: "POST",
        success: function (result) {
            opponentBoard = result;

            for (x = 0; x < opponentBoard.playerGrid.length; x++) {
                for (y = 0; y < opponentBoard.playerGrid[x].length; y++) {
                    if (opponentBoard.playerGrid[x][y].shot == "0") {
                        $("#P1" + opponentBoard.playerGrid[x][y].coordinates.X + opponentBoard.playerGrid[x][y].coordinates.Y).css('background-color', "blue");
                    }
                    else if (opponentBoard.playerGrid[x][y].shot == "1") {
                        $("#P1" + opponentBoard.playerGrid[x][y].coordinates.X + opponentBoard.playerGrid[x][y].coordinates.Y).css('background-color', "white");
                    }
                    else if (opponentBoard.playerGrid[x][y].shot == "2") {
                        $("#P1" + opponentBoard.playerGrid[x][y].coordinates.X + opponentBoard.playerGrid[x][y].coordinates.Y).css('background-color', "red");
                    }

                }
            }
        }
    });

}


function updateGameState() {

    $.ajax({
        contentType: "application/json",
        url: "/api/BattleshipGame/GameState",
        data: JSON.stringify(username),
        type: "GET",
        success: function (result) {
            gameState = result;

            if (fullGameState.winner != null) {

                var txt = $("#gameStatus").text();
                $("#gameStatus").text(gameState + " " + fullGameState.winner.Name + " WINS!!!");
            }
            else {
                $("#gameStatus").text(gameState);
            }
        }
    });
}

function updateGameObj() {

    $.ajax({
        contentType: "application/json",
        url: "/api/BattleshipGame/GameObj",
        type: "GET",
        success: function (result) {
            fullGameState = result;
            displaySunkShips();
        }
    });
}

function displaySunkShips() {

    var myPlayer;
    var myOpponent;

    $.ajax({
        contentType: "application/json",
        url: "/api/BattleshipGame/GetPlayer",
        data: JSON.stringify(username),
        type: "POST",
        success: function (result) {
            myPlayer = result;

            $.ajax({
                contentType: "application/json",
                url: "/api/BattleshipGame/GetOpponent",
                data: JSON.stringify(username),
                type: "POST",
                success: function (result) {
                    myOpponent = result;

                    mySunkShips = myPlayer.SunkShips;
                    opponentsSunkShips = myOpponent.SunkShips;

                    $('#opponentsSunk').empty();
                    for (var s in opponentsSunkShips) {

                        $('#opponentsSunk').append('<li>' + opponentsSunkShips[s].name + '</li>');
                    }

                    $('#mySunk').empty();
                    for (var s in mySunkShips) {

                        $('#mySunk').append('<li>' + mySunkShips[s].name + '</li>');
                    }

                }
            });

        }
    });

}
