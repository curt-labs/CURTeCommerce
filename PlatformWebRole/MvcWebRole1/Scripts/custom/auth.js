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

    $(document).on('click', '#btnSignup', function () {
        if ($('#same').is(':checked')) {
            fillShipping();
        }
        $('#registration_form').submit();
    });

    $(document).on('click', '#btnLogin', function () {
        showLoader('Logging you in...');
    });

    $(document).on('click', '#btnForgot', function () {
        showLoader('Sending Password Email...');
    });

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