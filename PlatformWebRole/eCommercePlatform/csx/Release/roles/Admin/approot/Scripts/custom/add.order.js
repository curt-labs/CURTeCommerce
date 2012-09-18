var addPart,updateTotal, emptyCart, removePart, newCustomer, toTitleCase, searchCustomers;
$(function () {
    $("#customersearch").autocomplete({
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
            $('#customerID').attr('value', ui.item.id);
            $('#OrderStep2').submit();
        },
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        }
    });

    $(document).on('click', '#same', function () {
        if ($(this).is(':checked')) {
            $.each($('fieldset.billing').find('input,select'), function (i, input) {
                var id, val;
                id = $(this).attr('id').substring(1);
                if ($(this).is(':checkbox')) {
                    if ($(this).is(':checked')) {
                        $('#s' + id).attr('checked', 'checked');
                    } else {
                        $('#s' + id).removeAttr('checked');
                    }
                } else {
                    val = $(this).val();
                    $('#s' + id).val(val);
                }
            });
        } else {
            $.each($('fieldset.shipping').find('input'), function (i, input) {
                $(this).val('');
            });
        }
    });

    $('#addPart').click(addPart);
    $('#emptyCart').click(emptyCart);
    $('#newCustomer').click(newCustomer);
    $(document).on('click', 'a.remove', removePart);

    $('#add-billing-address, #add-shipping-address').hide();
    $(document).on('click', '#new-billing-address', function (event) {
        event.preventDefault();
        $('#current-billing-address').hide();
        $('#add-billing-address').slideDown();
    });
    $(document).on('click', '#new-shipping-address', function (event) {
        event.preventDefault();
        $('#current-shipping-address').hide();
        $('#add-shipping-address').slideDown();
    });

    $(document).on('click', '#btnResetBilling', function (event) {
        $('#current-billing-address').slideDown();
        $('#add-billing-address').hide();
    });
    $(document).on('click', '#btnResetShipping', function (event) {
        $('#current-shipping-address').slideDown();
        $('#add-shipping-address').hide();
    });

    $(document).on('change', '#shipping_types', function (event) {
        var type = $(this).val();
        var customerID = $('#customerID').val();
        $('#btnChooseShippingType').attr('disabled', 'disabled');
        if (type !== 0) {
            if (confirm('Are you sure you want to upgrade to ' + toTitleCase(type.replace(/_/g, ' ')) + '?')) {
                window.location.href = '/Admin/Orders/Step3/' + customerID + '?type=' + type;
            } else {
                $('#shipping_types').val(0);
            }
        }
        $('#btnChooseShippingType').removeAttr('disabled');
    });

    $('label[for=shipping_types]').show();
});

searchCustomers = function (data) {
    console.log(data);
}

addPart = (function (event) {
    event.preventDefault();
    var partid = $.trim($('#partid').val());
    var cartid = $('#cart').data('cartid');
    if (partid != "" && cartid != "") {
        $.getJSON('/Admin/Orders/AddPartToCart', { 'partID': partid, 'cartID': cartid }, function (response) {
            console.log(response);
            if (response != null) {
                if ($('#part_' + partid).length > 0) {
                    var part = $('#part_' + partid);
                    var quantity = Number($(part).find('td.quantity span').html()) + 1;
                    $(part).find('td.quantity span').html(quantity);
                    var price = response.price * quantity;
                    $(part).find('td.price').html('$' + price.toFixed(2));
                } else {
                    var tr = '<tr style="display: none;" id="part_' + response.partID + '"><td>' + response.partID + '</td><td>' + response.shortDesc + '</td><td class="quantity"><span>1</span><a href="javascript:void(0)" data-id="' + response.partID + '" class="remove" title="Remove Item From Cart">&times;</a></td><td class="price">$' + response.price.toFixed(2) + '</td></tr>';
                    $('#cart tbody').append(tr);
                    $('#part_' + response.partID).fadeIn();
                }
                updateTotal();
                $('#partid').select();
            }
        });
    }
});

removePart = (function (event) {
    event.preventDefault();
    var partid = $(this).data('id');
    var cartid = $('#cart').data('cartid');
    $.post('/Admin/Orders/RemovePartFromCart', { 'cartID': cartid, 'partID': partid });
    $('#part_' + partid).fadeOut('fast', function () {
        $(this).remove();
        updateTotal();
    });
});

emptyCart = (function (event) {
    event.preventDefault();
    if (confirm("Are you sure you want to empty this cart?")) {
        var cartid = $('#cart').data('cartid');
        $.post('/Admin/Orders/EmptyCart', { 'cartID': cartid });
        $('#cart tbody tr').fadeOut('fast', function () {
            $(this).remove();
            updateTotal();
            $('#partid').val('');
        });
    }
});

updateTotal = (function () {
    var total = 0;
    $('table#cart tbody tr td.price').each(function (i, obj) {
        var pr = Number($(obj).html().replace('$', ''));
        total += pr;
    });
    $('table#cart tfoot tr td.price strong').html('$' + total.toFixed(2));
});

newCustomer = (function () {
    $('#newCustomerForm').slideToggle();
    $('#sfirst').parent().before('<label for="same"><input type="checkbox" id="same" value="1" />Same as billing address</label>');
});

toTitleCase = (function (str) {
    return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
});