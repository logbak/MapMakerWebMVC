var canvas = document.getElementById("canvasMap");
var ctx = canvas.getContext("2d");
var preview = "";
var posX = 1;
var posY = 1;
var sizeX = 1;
var sizeY = 1;
var icon = "";
var lineheight = 18;
var blocks;
var mapName;
var descriptions;
var events;
var exits;

var mapText = document.getElementById("map-info");
var otherText = document.getElementById("other-info");

//sets all the details from the exploration model
function getModelDetails(previewString, playerIcon, initialPosX, initialPosY, mSizeX, mSizeY, occupiedBlocks, allDescriptions, allEvents, exitsInfo) {
    preview = previewString;
    icon = playerIcon;
    posX = initialPosX;
    posY = initialPosY;
    sizeX = mSizeX;
    sizeY = mSizeY;
    blocks = occupiedBlocks;
    descriptions = allDescriptions;
    events = allEvents;
    exits = exitsInfo;
    mapName = descriptions[0].split(",");
}

function movement(event) {
    const keyPressed = event.keyCode;
    const W_KEY = 87;
    const A_KEY = 65;
    const S_KEY = 83;
    const D_KEY = 68;
    const E_KEY = 69;
    const V_KEY = 86;
    if (keyPressed === W_KEY) {
        posY--;
        if (checkExits("N")) {
            posY++;
        }
    }
    if (keyPressed === A_KEY) {
        posX--;
        if (checkExits("W")) {
            posX++;
        }
    }
    if (keyPressed === D_KEY) {
        posX++;
        if (checkExits("E")) {
            posX--;
        }
    }
    if (keyPressed === S_KEY) {
        posY++;
        if (checkExits("S")) {
            posY--;
        }     
    }

    clearCanvas();
    mapExploreView(preview);
    mapText.textContent = ("Map: " + mapName[0] + "| Player position: " + posX + "," + posY);
    otherText.textContent = "";

    if (keyPressed === V_KEY) {

        otherText.textContent = ("Map Description: " + mapName[1]);
        for (i = 1; i <= descriptions.length; i++) {
            var description = descriptions[i].split(",");
            otherText.textContent += ("\r\n" + "At X:" + description[0] + " Y:" + description[1] + " | " + description[2] + " - " + description[3]);
        };

    }

    if (keyPressed === E_KEY) {
        checkForEvents();
    }
}

function checkExits(direction) {
    //get original position before movement attempt
    var oPosX = posX;
    var oPosY = posY;
    if (direction == "N") oPosY = (posY + 1);
    else if (direction == "S") oPosY = (posY - 1);
    else if (direction == "E") oPosX = (posX - 1);
    else if (direction == "W") oPosX = (posX + 1);

    var exitToID = 0;
    exits.forEach(function (element) {
        //splits an individual exit element into an array of [posX, posY, exitDirection(as single character), exitToID]
        var exitInfo = element.split(",");
        //checks if the original location matches that of an exit and an attempt was made to move in that direction
        if (exitInfo[0] == oPosX && exitInfo[1] == oPosY && exitInfo[2] == direction) exitToID = exitInfo[3];
    });

    if (exitToID > 0) {
        //sets the MapID dropdown menu to the value of the exitToID
        setSelectedValue("MapID", exitToID);
        //submits the form on the page with the new mapID selected
        otherText.textContent = ("Exit to:" + exitToID);
        document.getElementById('exit').submit();
    }
    else return checkForWalls();
}

function setSelectedValue(selectObj, valueToSet) {
    var selector = document.getElementById(selectObj);
    for (var i = 0; i < selector.options.length; i++) {
        if (selector.options[i].text == valueToSet) {
            selector.options[i].selected = true;
            return;
        }
    }
}

function checkForWalls() {
    if (posX < 1 || posX > sizeX || posY < 1 || posY > sizeY) return true;
    else return checkForBlocks();
}

function checkForBlocks() {

    // converts positions x and y to an array
    var posArray = [posX.toString(), posY.toString()];
    // turns that array into a string "x,y"
    var currentPos = posArray.join(",");
    // checks if an element is equal to position "x,y"
    var blockAtPos = function (element) { return element == currentPos; };
    // checks each string in the blocks array against the position "x,y" and returns true if any of them match
    return blocks.some(blockAtPos);
}

function checkForEvents() {

    var plusOrMinus = function (value1, value2) {
        if (value1 == (value2 + 1) || value1 == (value2 - 1) || value1 == value2) return true;
        else return false;
    }

    for (i = 0; i <= events.length; i++) {
        var event = events[i].split(",");
        if (plusOrMinus(event[0], posX) && plusOrMinus(event[1], posY)) {
            if (numberOfEvents > 0) otherText.textContent += ("\r\n");
            otherText.textContent += ("You interact with " + event[2] + " and " + event[3] + ". " + event[4]);
        }
    };
}

function clearCanvas() {
    ctx.fillStyle = "#000";
    ctx.fillRect(0, 0, canvas.width, canvas.height);
}

function mapExploreView() {

    mapText.textContent = ("Map: " + mapName[0]);

    var lines = preview.split('\n');
    ctx.font = "18px monospace";
    ctx.fillStyle = "#72bb53";

    if (/Android|BlackBerry/i.test(navigator.userAgent)) {
        preview = preview.replace(/(\u2588)/g, "X");
        preview = preview.replace(/(\u2580)/g, "X");
        preview = preview.replace(/(\u2584)/g, "X");
        preview = preview.replace(/(\u2551)/g, "|");
        preview = preview.replace(/(\u2550)/g, "-");
        ctx.fillText("*Android devices do not support monospaced unicode characters.", 10, 15)
        ctx.fillText(" Please load on another device to see a more accurate map preview.", 10, 30)
    for (var i = 0; i < lines.length; i++)
        ctx.fillText(lines[i], 10, 60 + (i * lineheight));
        ctx.fillText(icon, 53 + (posX * 10.8), 115 + (posY * lineheight));
    }

    else {
        for (var i = 0; i < lines.length; i++)
            ctx.fillText(lines[i], 10, 15 + (i * lineheight));
        ctx.fillText(icon, 53 + (posX * 10.8), 70 + (posY * lineheight));
    }

}