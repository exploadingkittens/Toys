$(document).ready(function () {
    $.ajaxSetup({ cache: true });
    $.getScript('//connect.facebook.net/en_US/sdk.js', function () {
        FB.init({
            appId: '714757761996451',
            version: 'v2.5' // or v2.0, v2.1, v2.2, v2.3
        });

        $('#facebook-share-button').removeAttr('disabled');

        /*
        FB.ui({
            method: 'share_open_graph',
            action_type: 'og.likes',
            action_properties: JSON.stringify({
                object: 'https://developers.facebook.com/docs/',
            })
        }, function (response) {
            // Debug response (optional)
            console.log(response);
        });
        */
        /*
        FB.ui(
        {
        method: 'share',
        href: 'https://developers.facebook.com/docs/'
    }, function(response){});
    */
    });

    $('#facebook-share-button').click(function () {
        FB.ui({
            method: 'share_open_graph',
            action_type: 'og.likes',
            action_properties: JSON.stringify({
                object: 'https://developers.facebook.com/docs/',
            })
        }, function (response) {
            // Debug response (optional)
            console.log(response);
        });
    });
});