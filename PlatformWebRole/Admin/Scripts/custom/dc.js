var lat, lon, map, name;
$(function () {
    $('table').dataTable({ 'bJQueryUI': true });

    lat = $('#Latitude').val();
    lon = $('#Longitude').val();
    if (lat != undefined && lon != undefined) {
        var latLng = new google.maps.LatLng(lat, lon);
        var opts = {
            zoom: 8,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('map'), opts);
        $('#map').css('height', '300px').css('width', '300px').css('margin-bottom', '20px');

        marker = new google.maps.Marker({
            position: latLng,
            map: map,
            title: $('#Name').val(),
            animation: google.maps.Animation.DROP
        });
    } else { // We're on the index page
        var latLng = new google.maps.LatLng('38', '-97');
        var opts = {
            zoom: 4,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('map'), opts);

        $('#map').css('height', '600px').css('width', '900px').css('margin', '40px auto 20px auto');
        $.each($('table tbody tr'), function (i, row) {
            lat = $(this).data('lat');
            lon = $(this).data('lon');
            name = $(this).data('name');
            var latLng = new google.maps.LatLng(lat, lon);
            marker = new google.maps.Marker({
                position: latLng,
                map: map,
                title: name,
                animation: google.maps.Animation.DROP
            });
        });

    }

    $('a.delete').live('click', function () {
        if (confirm('Are you sure you want to remove this distribution center?')) {
            return true;
        }
        return false;
    });
});