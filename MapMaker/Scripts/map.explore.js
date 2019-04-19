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

var test = document.getElementById("test-zone");

function getModelDetails(previewString, playerIcon, initialPosX, initialPosY, mSizeX, mSizeY, occupiedBlocks) {
    preview = previewString;
    icon = playerIcon;
    posX = initialPosX;
    posY = initialPosY;
    sizeX = mSizeX;
    sizeY = mSizeY;
    blocks = occupiedBlocks;
}

function movement(event) {
    const keyPressed = event.keyCode;
    const W_KEY = 87;
    const A_KEY = 65;
    const S_KEY = 83;
    const D_KEY = 68;
    if (keyPressed === W_KEY) {
        posY--;
        if (checkForWalls()) {
            posY++;
        }       
        clearCanvas();
        mapExploreView(preview);
        test.textContent = (posX + "," + posY);
    }
    if (keyPressed === A_KEY) {
        posX--;
        if (checkForWalls()) {
            posX++;
        }     
        clearCanvas();
        mapExploreView(preview);
        test.textContent = (posX + "," + posY);
    }
    if (keyPressed === D_KEY) {
        posX++;
        if (checkForWalls()) {
            posX--;
        }     
        clearCanvas();
        mapExploreView(preview);
        test.textContent = (posX + "," + posY);
    }
    if (keyPressed === S_KEY) {
        posY++;
        if (checkForWalls()) {
            posY--;
        }     
        clearCanvas();
        mapExploreView(preview);
        test.textContent = (posX + "," + posY);
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
    // checks if an element is equaly to position "x,y"
    var blockAtPos = function (element) { return element == currentPos; };
    // checks each string in the blocks array against the position "x,y" and returns true if any of them match
    return blocks.some(blockAtPos);
}

function clearCanvas() {
    ctx.fillStyle = "#000";
    ctx.fillRect(0, 0, canvas.width, canvas.height);
}

function mapExploreView() {
    if (/Android|BlackBerry/i.test(navigator.userAgent)) {
        preview = preview.replace(/(\u2588)/g, "X");
        preview = preview.replace(/(\u2580)/g, "X");
        preview = preview.replace(/(\u2584)/g, "X");
        preview = preview.replace(/(\u2551)/g, "|");
        preview = preview.replace(/(\u2550)/g, "-");
    }
    var lines = preview.split('\n');
    ctx.font = "18px monospace";
    ctx.fillStyle = "#72bb53";
    if (/Android|BlackBerry/i.test(navigator.userAgent)) {
        ctx.fillText("*Android devices do not support monospaced unicode characters.", 10, 15)
        ctx.fillText(" Please load on another device to see a more accurate map preview.", 10, 30)
    for (var i = 0; i < lines.length; i++)
        ctx.fillText(lines[i], 10, 60 + (i * lineheight));
    }
    else {
        for (var i = 0; i < lines.length; i++)
            ctx.fillText(lines[i], 10, 15 + (i * lineheight));
    }
    ctx.fillText(icon, 53 + (posX * 10.8), 70 + (posY * lineheight));

}