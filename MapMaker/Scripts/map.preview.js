function mapPreview(previewString) {
    var canvas = document.getElementById("canvasMap");
    var ctx = canvas.getContext("2d");
    var preview = previewString;
    var lineheight = 18;
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
};