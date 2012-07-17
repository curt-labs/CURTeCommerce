var testimonialTable;
$(function () {

    $(document).on('click', '.modalClose', function () {
        $.modal.close();
        return false;
    });

    $(document).on('click', '.delete', function (event) {
        event.preventDefault();
        var id, row;
        id = $(this).data('id');
        row = $(this).parent().parent().get()[0];
        if (confirm('Are you sure you want to remove this testimonial?')) {
            $.get('/Admin/Testimonials/Delete/' + id, function (resp) {
                if (resp.length == 0) {
                    testimonialTable.fnDeleteRow(row);
                } else {
                    showMessage('There was an error while processing your request: ' + resp);
                }
            }).error(function (req, status, error) {
                showMessage('There was an error while processing your request: ' + error);
            });
        }
    });

    $(document).on("click",'.isApproved', function () {
        var testimonialID = $(this).data('id');
        var row = $(this).parent().parent().get()[0]
        if (testimonialID > 0) {
            $.getJSON('/Admin/Testimonials/Approve', { 'id': testimonialID }, function (response) {
                if (response.error == null) {
                    if (response == 1) {
                        showMessage("Testimonial Approved.");
                    } else {
                        showMessage("Testimonial Unapproved.");
                    }
                    testimonialTable.fnDeleteRow(row);
                } else {
                    showMessage('There was an error while processing your request: ' + response.error);
                }
            });
        }
    });

    testimonialTable = $('table.testimonialTable').dataTable({
        "bJQueryUI": true,
        "aoColumns": [
            null,
            null,
            null,
            { "sType": "date" },
            null,
            null
        ],
        "aaSorting": [[3, "asc"]]
    });

});