var orderTable;

$(function () {
    $(document).on('change', '#tableperpage', function () {
        $('#formperpage').submit();
    });
    $(document).on('change', '.orderStatus', function () {
        var selbox = $(this);
        var prevStatus = $(this).data('status');
        var orderID = $(this).data('id');
        var newID = $(this).val();
        var status = $(this).find("option:selected").text();
        if (confirm("Change order #" + orderID + " status to " + status + "?")) {
            $.getJSON('/Admin/Orders/ChangeStatus', { id: orderID, statusID: newID }, function (data) {
                $(selbox).effect("highlight", {}, 1000);
            });
        } else {
            $(this).val(prevStatus);
        }
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