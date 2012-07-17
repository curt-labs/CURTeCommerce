var gotoOrder;
$(function () {

    gotoOrder = function (id) {
        window.location.href = '/Account/Order/' + id;
    };

    $(document).on('click', 'article.order', function () {
        var id = $(this).data('id');
        if (id !== undefined && id > 0) {
            gotoOrder(id);
        }
    });
});