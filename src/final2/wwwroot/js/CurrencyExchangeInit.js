$(function () {
    $('.currencies span').text('Loading...');

    $.get('http://api.fixer.io/latest?base=ILS&symbols=USD,GBP,EUR', function (result) {
        for (var key in result.rates) {
            if (result.rates.hasOwnProperty(key)) {
                $('#currency-' + key).text(result.rates[key]);
            }
        }
    })
})
