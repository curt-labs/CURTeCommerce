$(function () {
    $(document).on('click', '.setStateRate', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var rate = $('#staterate-' + id).attr('value');
        var hide = $('#statehidden-' + id).is(':checked');
        $.post('/Admin/Regions/SaveRate', { 'stateID': id, 'rate': rate, 'hide': hide }, function (resp) {
            if (resp != null && resp == "") {
                $('#state-' + id).effect("highlight", {}, 1000);
                showMessage("State / Province Saved Successfully");
            } else {
                showMessage(resp);
            }
        });
    });
});