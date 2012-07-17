$(function () {
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

    $('label[for=shipping_types]').show();
});