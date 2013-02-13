var gotoOrder;
$(function () {

    gotoOrder = function (id) {
        window.location.href = '/Account/Order/' + id;
    };

    $(document).on('click', 'table.orders tr.order', function () {
        var id = $(this).data('id');
        if (id !== undefined && id > 0) {
            gotoOrder(id);
        }
    });
    $('#add-address').hide();
    $(document).on('click', '#new-address', function (event) {
        event.preventDefault();
        $('#add-address').slideDown();
    });
    $(document).on('click', '#btnResetAddress', function (event) {
        $('#add-address').hide();
    });
    $(document).on('click', '.address a.delete', function (e) {
        event.preventDefault();
        var src = $(this).attr('href');
        if (confirm('Are you sure you want to delete this address?')) {
            window.location.href = src;
        }
    });
});