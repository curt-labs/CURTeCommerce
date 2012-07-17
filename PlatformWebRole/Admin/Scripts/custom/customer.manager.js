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

    customerTable = $('.customerTable').dataTable({ 'bJQueryUI': true });
    //cartItemTable = $('#cart table').dataTable({ 'bJQueryUI': true });

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