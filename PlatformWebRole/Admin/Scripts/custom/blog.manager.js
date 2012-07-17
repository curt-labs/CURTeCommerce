var postTable,categoryTable,commentTable;
$(function () {

    postTable = $('table#postTable').dataTable({
        "bJQueryUI": true,
        "aoColumns": [
            null,
            { "sType": "date" },
            null,
            null,
            null,
            null
        ],
        "aaSorting": [[1, "asc"]]
    });

    categoryTable = $('table#categoryTable').dataTable({ "bJQueryUI": true });

    commentTable = $('table#commentTable').dataTable({
        "bJQueryUI": true,
        "aoColumns": [
            null,
            null,
            { "sType": "date" },
            null,
            null
        ],
        "aaSorting": [[2, "asc"]]
    });

    $(document).on("click", '.delete_category', deleteCategory);
    $('#btnSubmitPublish').click(function () {
        var act = $('form.admin_form').attr('action');
        $('form.admin_form').attr('action', act + '?publish=true');
        $('form.admin_form').submit();
    });

    $('.delete_comment').live('click', function () {
        var comment_id = $(this).attr('data-id');
        var table_row = $(this).parent().parent().get()[0];
        if (comment_id > 0 && confirm("Are you sure you want to remove this Comment?")) {
            $.getJSON('/Admin/Blog/DeleteComment/' + comment_id, function (resp) {
                if (resp.error == undefined) {
                    commentTable.fnDeleteRow(table_row);
                    showMessage('Comment removed.');
                } else {
                    showMessage(resp.error);
                }
            });
        }
        return false;
    });

});

function deleteCategory(event) {
    event.preventDefault();
    var id, row;
    id = $(this).data("id");
    row = $(this).parent().parent().get()[0];
    if (confirm('Are you sure you want to remove this category?')) {
        $.post('/Admin/Blog/DeleteCategory', { 'id': id }, function (data) {
            if (data == null || data == "") {
                categoryTable.fnDeleteRow(row);
                showMessage("Category Deleted");
            } else {
                showMessage(data);
            }
        }, "text");
    }
}