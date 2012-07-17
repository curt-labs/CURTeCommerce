var orderTable;

$(function () {
    orderTable = $('.orderTable').dataTable({
        "bJQueryUI": true,
        "aoColumns": [
            null,
            null,
            null,
            { "sType": "date" },
            null,
            null,
            null,
            null
        ],
        "aaSorting": [[3, "desc"]]
    });

});