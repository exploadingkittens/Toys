$(function () {
    var notifications = $('.user-notification');

    notifications.append('<button class="notification-close-button"><span>&#10006;</span></button>');

    $('.notification-close-button').click(function () {
        $(this).parent().hide(500);
    });
});