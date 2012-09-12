var orderTable;

$(function () {
    $(document).on('change', '#tableperpage', function () {
        $('#formperpage').submit();
    });

    $("#tablesearch").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Admin/Orders/Search",
                dataType: "json",
                data: {
                    searchtext: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: "Order # " + item.orderID,
                            value: "Order # " + item.orderID,
                            id: item.orderID
                        }
                    }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            window.location.href = "/Admin/Orders/Items/" + ui.item.id
        },
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        }
    });
});