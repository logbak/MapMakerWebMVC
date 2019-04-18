var canvas = document.getElementById("canvasMap");
var ctx = canvas.getContext("2d");
var preview = "";
var posX = 1;
var posY = 1;
var sizeX = 99;
var sixeY =99;
var icon = "";
var lineheight = 18;
var blocks;

function getModelDetails(previewString, playerIcon, initialPosX, initialPosY, mSizeX, mSizeY, occupiedBlocks) {
    preview = previewString;
    icon = playerIcon;
    posX = initialPosX;
    posY = initialPosY;
    sixeX = mSizeX;
    sixeY = mSizeY;
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
        if (checkForBlocks()) {
            posY++;
        }       
        clearCanvas();
        mapExploreView(preview);
    }
    if (keyPressed === A_KEY) {
        posX--;
        if (checkForBlocks()) {
            posX++;
        }     
        clearCanvas();
        mapExploreView(preview);
    }
    if (keyPressed === D_KEY) {
        posX++;
        if (checkForBlocks()) {
            posX--;
        }     
        clearCanvas();
        mapExploreView(preview);
    }
    if (keyPressed === S_KEY) {
        posY++;
        if (checkForBlocks()) {
            posY--;
        }     
        clearCanvas();
        mapExploreView(preview);
    }
}

function checkForBlocks() {

    if (posX < 1) {
        return true;
    }
    //else if (posX > sizeX) {
    //    return true;
    //}
    else if (posY < 1) {
        return true;
    }
    //if (posY > sixeY) {
    //    return true;
    //}
    blocks.forEach(function (element) {
        var block = element.split(",");
        if (block[0] === posX && block[1] === posY) {
            return true;
        }
        return false;
    });
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