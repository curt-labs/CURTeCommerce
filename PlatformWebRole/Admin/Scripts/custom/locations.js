/* Author: Alex Ninneman @ninnemana */
var map,marker;
$(function () {
    var loc_table = $('table').dataTable({ "bJQueryUI": true });

    $('#google_places').live('click', function () {
        if ($(this).is(':checked')) {
            // Display the available place types
            var html = '<div class="place_types">';
            html += '<span>Select Relevant Google Place Types</span>';
            html += '<div class="left">';
            html += '<label><input type="checkbox" name="place_types" id="car_dealer" value="car_dealer" /><span>Car Dealer</span></label>';
            html += '<label><input type="checkbox" name="place_types" id="car_rental" value="car_rental" /><span>Car Rental</span></label>';
            html += '<label><input type="checkbox" name="place_types" id="car_repair" value="car_repair" /><span>Car Repair</span></label>';
            html += '<label><input type="checkbox" name="place_types" id="car_wash" value="car_wash" /><span>Car Wash</span></label>';
            html += '</div>';
            html += '<div class="left">';
            html += '<label><input type="checkbox" name="place_types" id="gas_station" value="gas_station" /><span>Gas Station</span></label>';
            html += '<label><input type="checkbox" name="place_types" id="convenience_store" value="convenience_store" /><span>Convenience Store</span></label>';
            html += '<label><input type="checkbox" name="place_types" id="department_store" value="department_store" /><span>Department Store</span></label>';
            html += '<label><input type="checkbox" name="place_types" id="store" value="store" /><span>Store</span></label>';
            html += '</div>';
            html += '<div class="clearfix"></div>';
            html += '</div>';
            $(this).parent().after(html);
            $('.place_types').slideDown();
        } else {
            $('.place_types').slideUp('400', function () {
                $('.place_types').remove();
            });
        }
    });

    $('.view-map').live('click', function (e) {
        e.preventDefault();

        // Retrieve the latitude/longitude for this object
        var lat = $(this).data('latitude');
        var lng = $(this).data('longitude');
        $(this).remove();
        var latLng = new google.maps.LatLng(lat, lng);
        var opts = {
            zoom: 8,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('map'), opts);
        $('#map').css('height', '400px').css('width', '400px').css('margin-bottom', '20px');

        marker = new google.maps.Marker({
            position: latLng,
            map: map,
            title: $('#name').val(),
            animation: google.maps.Animation.DROP
        });
    });

    $('.view-place-listing').live('click', function (e) {
        e.preventDefault();
        var ref, key, places;

        ref = $(this).data('reference');
        $.post('/Admin/Locations/ViewGoogPlace', { 'reference': ref }, function (resp) {
            displayDetails(resp);
        }, 'json');
    });

    var displayDetails = function (details) {
        if (details.status == "NOT_FOUND") {
            if (confirm('The reference to this places listing appears to be broken.\r\n Should we remove the reference in the database?')) {
                var segments = $('form.admin_form').attr('action').split('/');
                locID = segments[segments.length - 1];
                $.post('/Admin/Locations/DeletePlaceReference', { 'locationID': locID }, function (resp) {
                    if (resp.length > 0) {
                        displayError('Unable to remove places reference: ' + resp);
                    }
                });
            }
        } else {
            console.log(details);
        }
    };

});