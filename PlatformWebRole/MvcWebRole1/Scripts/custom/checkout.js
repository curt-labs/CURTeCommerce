var fillShipping;
$(function () {
    $(document).on('click', '#same', function () {
        if ($(this).is(':checked')) {
            $('#show_shipping').hide();
            fillShipping();
        } else {
            $('#show_shipping').slideDown('fast');
            $.each($('fieldset.shipping').find('input'), function (i, input) {
                $(this).val('');
            });
        }
    });
    $(document).on('change', '#shipping_types', function (event) {
        var type = $(this).val();
        $('#btnChooseShippingType').attr('disabled', 'disabled');
        if (type !== 0) {
            if (confirm('Are you sure you want to upgrade to ' + toTitleCase(type.replace(/_/g, ' ')) + '?')) {
                window.location.href = '/Cart/UpgradeShipping?type=' + type;
            } else {
                $('#shipping_types').val(0);
            }
        }
        $('#btnChooseShippingType').removeAttr('disabled');
    });

    $(document).on('click', '#btnSignup', function () {
        if ($('#same').is(':checked')) {
            fillShipping();
        }
        $('#customer_form').submit();
    });


    $('label[for=shipping_types]').show();
});

fillShipping = function () {
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
}