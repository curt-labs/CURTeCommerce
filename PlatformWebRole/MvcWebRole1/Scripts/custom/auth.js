$(function () {

    $('#sfirst').parent().before('<label for="same"><input type="checkbox" id="same" value="1" />Same as billing address</label>');

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

    $(document).on('click', '#btnSignup', function () {

    });

    $(document).on('click', '#btnLogin', function () {
        showLoader('Logging you in...');
    });

    $(document).on('click', '#btnForgot', function () {
        showLoader('Sending Password Email...');
    });

    $(window).scroll(function () {
        if ($('div.signup').css('display') != 'block') {
            $('div.right article.scroller').css('margin-top', $(this).scrollTop());
        }
    });
});