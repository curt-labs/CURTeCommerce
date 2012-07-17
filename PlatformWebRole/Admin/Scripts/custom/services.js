var other, matched;
$(function () {
    other = $('#otherLocations').dataTable({ 'bJQueryUI': true });
    matched = $('#matchedLocations').dataTable({ 'bJQueryUI': true });
    $('#indexTable').dataTable({ 'bJQueryUI': true });

    $('.delete').live('click', function () {
        if (confirm('Are you sure you want to remove this service?')) {
            return true;
        }
        return false;
    });

    $('.addLocation').live('click', function () {
        var name, locationID, serviceID, row;
        row = $(this).parent().parent().get()[0];
        name = $(row).find('td:first').text();
        locationID = $(this).attr('data-locationID');
        serviceID = $(this).attr('data-serviceID');

        if (confirm('Are you sure you want to tie ' + name + ' to this service?')) {

            if (locationID > 0 && serviceID > 0) {
                $.post('/Admin/Services/AddLocation', { 'serviceID': serviceID, 'locationID': locationID }, function (resp) {
                    if (resp == null) {
                        var data = other.fnGetData($(row).get()[0]);
                        data[data.length - 1] = '<a href="javascript:void(0)" title="Remove Location" class="removeLocation" data-locationID="' + locationID + '" data-serviceID="' + serviceID + '">Remove</a>';
                        matched.fnAddData(data);
                        other.fnDeleteRow(row);
                    } else {
                        displayError(resp);
                    }
                }, 'json');
            }
        }
    });

    $('.removeLocation').live('click', function () {
        var name, locationID, serviceID, row;
        row = $(this).parent().parent().get()[0];
        name = $(row).find('td:first').text();
        locationID = $(this).attr('data-locationID');
        serviceID = $(this).attr('data-serviceID');

        if (confirm('Are you sure you want to remove ' + name + ' from this service?')) {

            if (locationID > 0 && serviceID > 0) {
                $.post('/Admin/Services/RemoveLocation', { 'serviceID': serviceID, 'locationID': locationID }, function (resp) {
                    if (resp == null) {
                        var data = matched.fnGetData($(row).get()[0]);
                        data[data.length - 1] = '<a href="javascript:void(0)" title="Add Location" class="addLocation" data-locationID="' + locationID + '" data-serviceID="' + serviceID + '">Add</a>';
                        other.fnAddData(data);
                        matched.fnDeleteRow(row);
                    } else {
                        displayError(resp);
                    }
                }, 'json');
            }
        }
    });
});