var customerTable, cartItemTable, toggleSuspended;

$(function () {

    toggleSuspended = function (id) {
        $.post('/Admin/Customers/Suspension', { 'id': id, 'ajax': true }, function (data) {
            if (data.length === 0) {
                return true;
            } else {
                return false;
            }
        }).error(function (req, status, error) {
            showMessage('There was an error while processing your request.');
        });
    };

    $(document).on('change', '#tableperpage', function () {
        $('#formperpage').submit();
    });

    $("#tablesearch").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Admin/Customers/Search",
                dataType: "json",
                data: {
                    searchtext: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.fname + " " + item.lname + " (" + item.email + ")",
                            value: item.fname + " " + item.lname + " (" + item.email + ")",
                            id: item.ID
                        }
                    }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            window.location.href = "/Admin/Customers/Info/" + ui.item.id
        },
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        }
    });
    $(document).on('click', 'a.suspension', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        if (confirm("Are you sure you want to toggle the suspension of this account?")) {
            if (!toggleSuspended(id)) {
                if ($(this).text() == 'Suspend') {
                    $(this).text('Reinstate');
                    showMessage('Customer account has been suspended.');
                } else {
                    $(this).text('Suspend');
                    showMessage('Customer account has been reinstated.');
                }
            } else {
                showMessage('There was an error while processing your request.');
            }
        }
    });
});