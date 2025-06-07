$(document).ready(function () {
    $("#menu-toggle, .sidebar-overlay").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });
});