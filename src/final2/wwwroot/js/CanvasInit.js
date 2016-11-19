$(function () {
    var canvas = document.getElementById('logo-canvas');
    if (canvas.getContext) {
        var ctx = canvas.getContext("2d");
        var w = 140;
        var x = 80;
        var y = 80;
        ctx.beginPath();
        ctx.arc(x, y, w / 2, 0, 2 * Math.PI, false);
        ctx.fill();

        ctx = canvas.getContext("2d");
        ctx.font = '20pt Calibri';
        ctx.fillStyle = 'white';
        ctx.textAlign = 'center';
        ctx.fillText('Shoppy', x, y + 7);
    }
});