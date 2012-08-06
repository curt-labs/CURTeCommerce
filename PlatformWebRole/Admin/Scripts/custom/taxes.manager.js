$(function () {
    $(document).on('click', '.setStateRate', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var rate = $('#state-' + id).attr('value');
        $.post('/Admin/Taxes/SaveRate', { 'stateID': id, 'rate': rate }, function (resp) {
            if (resp != null && resp == "") {
                $('#state-' + id).effect("highlight", {}, 1000);
                showMessage("Tax Rate Saved Successfully");
            } else {
                showMessage(resp);
            }
        });
    });
});