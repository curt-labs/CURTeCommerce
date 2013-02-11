var customerTable, cartItemTable, toggleSuspended, clearAddressForm;

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

    $(document).on('click', '#addaddressbtn', function (e) {
        e.preventDefault();
        $('#addAddress').slideDown('fast');
    });

    $(document).on('click', '.edit', function (e) {
        e.preventDefault();
        var idstr = $(this).data('id');
        $.getJSON('/Admin/Customers/GetAddress/' + idstr, function (address) {
            if (address != null && address.ID > 0) {
                $('#addressID').val(address.ID);
                $('#first').val(address.first);
                $('#last').val(address.last);
                $('#street1').val(address.street1);
                $('#street2').val(address.street2);
                $('#city').val(address.city);
                $('#state').val(address.state);
                $('#postalcode').val(address.postal_code);
                $('#residential').attr('checked', false);
                if (address.residential) {
                    $('#residential').attr('checked', true);
                }
            }
            $('#addAddress').slideDown('fast');
        });
    });

    $(document).on('click', '.delete', function (e) {
        e.preventDefault();
        var idstr = $(this).data('id');
        if (confirm("Are you sure you want to delete this address?")) {
            $.post('/Admin/Customers/DeleteAddress/' + idstr, function (data) {
                if (data.success) {
                    $('#address_' + idstr).fadeOut('fast', function () {
                        $(this).remove();
                    });
                } else {
                    showMessage("There was a problem removing the address.");
                }
            }, 'json');
        }
    });

    $(document).on('click', '.setdefault', function (e) {
        e.preventDefault();
        var href = $(this).attr('href');
        if (confirm("Set this address as default?")) {
            window.location = href;
        }
    });

    $(document).on('click', '#btnResetAddress', function () {
        clearForm('#addressform');
        $('#addAddress').slideUp('fast');
    });

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

clearForm = function (idstr) {
    var fields = $(idstr).find("input:not([type=submit]):not([type=reset])");
    $(fields).each(function (i, obj) {
        if ($(obj).attr('type') == 'checkbox') {
            $(obj).attr('checked', $(obj).data('default') == 'checked')
        } else {
            $(obj).attr('value', $(obj).data('default'));
        }
    });
}