var orderTable;

$(function () {
    orderTable = $('.orderTable').dataTable({
        "bJQueryUI": true,
        "aoColumns": [
            null,
            { "sType": "date" },
            null,
            null,
            { "sType": "date" },
            null,
            { "sType": "date" },
            null,
            null,
            null,
            null
        ],
        "aaSorting": [[1, "desc"]]
    });

});