$('#AddNewProductForm').submit(function (event) {
    var picUrl = $('#product-image-url-field').val();

    if (!picUrl || picUrl == '') {
        alert('An image must be chosen');
        event.preventDefault();
    }
});