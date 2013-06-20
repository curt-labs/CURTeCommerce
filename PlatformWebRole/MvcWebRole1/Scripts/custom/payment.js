$(function () {
    $(document).on('submit', '#ccform', function () {
        $('#paymentSubmit').val('Submitting...');
        $('#paymentSubmit').attr('disabled', 'disabled');
    });
});