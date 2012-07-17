var table;
$(function () {
    table = $('table').dataTable({ "bJQueryUI": true });

    $('.remove').live('click', function (e) {
        e.preventDefault();
        var id, row;
        id = $(this).data('id');
        row = $(this).parent().parent().get()[0];
        if (id > 0 && confirm('Are you sure you want remove this subscription?')) {
            $.post('/Admin/Newsletter/Delete', { 'id': id }, function (resp) {
                if (resp.length === 0) {
                    table.fnDeleteRow(row);
                } else {
                    showMessage(resp);
                }
            });
        }
        return false;
    });
});