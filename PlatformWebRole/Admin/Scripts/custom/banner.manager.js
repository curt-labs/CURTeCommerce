$(function () {
    createSortable();
    $(document).on('click', '.delete', function (e) {
        var id = $(this).data('id');
        e.preventDefault();
        var href = $(this).attr('href');
        var row = $('#banner_' + id);
        if (confirm('Are you sure you want to remove this banner?')) {
            $.get(href, function (resp) {
                if (resp.length === 0) {
                    $(row).fadeOut(function () {
                        $(row).remove();
                    });
                } else {
                    showMessage(resp);
                }
            });
        }
    });
});


function updateSort() {
    var x = $('ul.banners').sortable("serialize");
    $.post("/Admin/Banner/updateSort?" + x);
}

function createSortable() {
    $("ul.banners").sortable({
        handle: 'img',
        update: function (event, ui) { updateSort(); }
    }).disableSelection();
}