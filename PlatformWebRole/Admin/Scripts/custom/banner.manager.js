$(function () {
    var table = $('table').dataTable({ 'bJQueryUI': true });

    $(document).on('click', '.delete', function (e) {
        e.preventDefault();
        var href = $(this).attr('href');
        var row = $(this).closest('tr').get()[0];
        if (confirm('Are you sure you want to remove this banner?')) {
            $.get(href, function (resp) {
                if (resp.length === 0) {
                    table.fnDeleteRow(row);
                } else {
                    showMessage(resp);
                }
            });
        }
    });
});