$(function () {
    $('#product-image').val('');
    $('#product-image').change(function () {
        var data = new FormData();
        var files = $("#product-image").get(0).files;
        if (files.length <= 0) {
            throw "No files chosen!";
        }

        data.append("productImage", files[0]);

        $.ajax({
            url: '/Toys/UploadImage',
            type: "POST",
            processData: false,
            contentType: false,
            data: data,
            success: function (response) {
                $('#product-image-url-field').val(response);
                $('#product-image-preview').attr('src', response);
                $('#product-image-preview').show();
            },
            error: function (er) {
                $('#product-image-url-field').val('');
                alert('Something happened');
                console.error("Failed uploading image", er);
            }
        });
    });
});