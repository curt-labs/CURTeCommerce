var map, markers, locations, infoWindow, geocoder, directionsService, mapbounds, removeDirections, loadDirections, directionsmap, directionsmarker, loadDirectionsMap;
var loadScript, initialize;
$(function () {

    loadScript = function () {
        var script = document.createElement("script");
        script.type = "text/javascript";
        script.src = "http://maps.googleapis.com/maps/api/js?sensor=true&callback=initialize";
        document.body.appendChild(script);
    };

    initialize = function () {
        loadMap('38', '-97', 4);
    };

    var displayLoader = function () {
        var width = $('#mid_col').width();
        var height = $('#mid_col').height();
        $('#mid_col').css('position', 'relative');
        var loader = document.createElement('div');
        $('#mid_col').append(loader);
        $(loader).attr("id", "loader").addClass('loader').css('background', 'white').css('z-index', '2000').css('position', 'absolute').css('top', '0').css('left', '0').css('height', height).css('width', width).css('opacity', '0.8').css('text-align', 'center');
        var img = document.createElement('img');
        $(img).attr('src', '/Content/img/159.gif').css('width', '175px').css('display', 'block').css('margin', '60px auto 10px auto');
        $(loader).append(img);
        var text = document.createElement('span');
        $(text).css('font-size', '22px').text('Calculating your location...');
        $(loader).append(text);
    };

    var hideLoader = function () {
        $('div#loader').fadeOut(400, function () {
            //$(this).remove();
        });
    };

    var removeDirections = function () {
        $('#site_overlay').remove();
        $('#site_overlay_container').remove();
        $('#directions').remove();
        $('body').off('keyup');
        $(window).off('resize');
    };


    var loadDirections = function () {
        var destination, from_addr, from_city, from_state, request;
        destination = $('#dir_destination').val();
        from_addr = $('#directAddr').val();
        from_city = $('#city').val();
        from_state = $('#state').val();
        var directionsService = new google.maps.DirectionsService();
        var directionsRenderer = new google.maps.DirectionsRenderer();
        directionsRenderer.setMap(directionsmap);
        directionsRenderer.setPanel(document.getElementById('directionsPanel'));
        request = {
            origin: from_addr + ' ' + from_city + ',' + from_state,
            destination: destination,
            travelMode: google.maps.TravelMode.DRIVING,
            provideRouteAlternatives: true
        };
        directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsRenderer.setMap(null);
                directionsRenderer.setMap(directionsmap);
                directionsmarker.setMap(null);
                $('#directionsPanel').empty();
                $('#directionsformcont').slideUp('fast', function () {
                    $('#changeDirections').fadeIn();
                    $('#directionsPanel').show();
                    directionsRenderer.setDirections(response);
                });
            } else {
                alert('Error: ' + status);
            }
        });
    };
    $(document).on('click', '#changeDirections', function (event) { $('#directionsformcont').slideDown(); $('#changeDirections').hide(); $('#directionsPanel').hide(); });

    var loadMap = function (lat, longitude, zoom) {
        var options = {
            center: new google.maps.LatLng(lat, longitude),
            mapTypeId: google.maps.MapTypeId.TERRAIN,
            fillOpacity: 0,
            strokeOpacity: 0,
            zoom: zoom
        };
        $('#map').show();
        map = new google.maps.Map(document.getElementById('map'), options);

        loadMarkers();
    };

    var loadDirectionsMap = function () {
        var endlocation = new google.maps.LatLng($('#dir_lat').val(), $('#dir_long').val());
        var directionsoptions = {
            center: endlocation,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            zoom: 12
        };
        directionsmap = new google.maps.Map(document.getElementById('directions_map'), directionsoptions);
        directionsmarker = new google.maps.Marker({
            position: endlocation,
            map: directionsmap,
            title: $('#dir_name').val(),
            animation: google.maps.Animation.DROP
        });
        dirbounds = new google.maps.LatLngBounds();
        dirbounds.extend(endlocation);
        directionsmap.fitBounds(dirbounds);
        return true;
    };

    var loadMarkers = function () {
        markers = new Array();
        var mapbounds = new google.maps.LatLngBounds();
        infoWindows = new Array();
        for (i = 0; i < locations.length; i++) {
            var glatlong = new google.maps.LatLng(locations[i].latitude, locations[i].longitude);
            var marker = new google.maps.Marker({
                position: glatlong,
                map: map,
                title: locations[i].name,
                animation: google.maps.Animation.DROP
            });
            mapbounds.extend(glatlong);
            markers[i] = marker;
            google.maps.event.addListener(marker, 'click', function () {
                if (infoWindow) { // Check if the infoWindow has been populated
                    // Close that shit!
                    infoWindow.close();
                }

                // Get the index of this array, we'll use it to fetch the content for the location
                var index = $.inArray(this, markers);

                // Create out InfoWindow
                infoWindow = new google.maps.InfoWindow({
                    content: generateInfoWindow(index)
                });
                // Open the InfoWindow
                infoWindow.open(map, markers[index]);
            });

        }
        map.fitBounds(mapbounds);
    };

    // Create the content for the InfoWindow
    var generateInfoWindow = function (index) {
        var location = locations[index]; // Get the location we're going to pull the content from
        var contentString = "";
        if (location !== undefined) {

            contentString = '<div class="infoWindow">';
            contentString += '<span class="title">' + location.name + '</span>';
            contentString += '<span class="directions_easy" data-address="' + location.address + ' ' + location.city + ',' + location.state + '"  data-lat="' + location.latitude + '" data-long="' + location.longitude + '">';
            contentString += '<img src="/Content/img/directions.jpg" alt="Get Directions" />';
            contentString += 'Get Directions</span>';
            if (location.phone !== undefined) {
                contentString += '<span>';
                contentString += '<img src="/Content/img/phone-icon.png" alt="Phone: ' + location.phone + '">';
                contentString += 'Phone: ' + location.phone;
                contentString += '</span>';
            }
            if (location.fax !== undefined) {
                contentString += '<span>';
                contentString += '<img src="/Content/img/Fax.png" alt="Fax: ' + location.fax + '">';
                contentString += 'Fax: ' + location.fax;
                contentString += '</span>';
            }
            if (location.email !== undefined) {
                contentString += '<span class="send_email" data-email="' + location.email + '">';
                contentString += '<img src="/Content/img/footer/mail.png" alt="E-Mail: ' + location.email + '">';
                contentString += 'E-Mail: ' + location.email;
                contentString += '</span>';
            }

            contentString += '</div>';

        }
        return contentString;
    };

    var promptForAddress = function () {
        $('#directions_modal').modal({
            containerCss: {
                backgroundColor: '#ffffff',
                borderColor: '#ffffff',
                height: '500px',
                width: '800px',
                padding: '10px'
            }, onClose: function (dialog) {
                dialog.data.fadeOut('400', function () {
                    dialog.container.hide('400', function () {
                        dialog.overlay.slideUp('fase', function () {
                            $.modal.close();
                        });
                    });
                });
            }
        });
    };


    $('.delete').click(function () {
        if (confirm('Are you sure you want to delete this location?')) {
            return true;
        }
        return false;
    });

    $('.map_easy').live('click', function () {
        var lat, longitude, id, locationIndex;

        // Get the attributes of this location
        lat = $(this).data('lat');
        longitude = $(this).data('long');
        id = $(this).data('id');

        // Pan the map to it's location
        map.panTo(new google.maps.LatLng(lat, longitude));

        // Get the location that matches this locationID
        for (i = 0; i < locations.length; i++) {
            if (locations[i].locationID === id) {
                locationIndex = $.inArray(locations[i], locations);
                break;
            }
        }

        // If our infoWindow has been popualated, close that shit :)
        if (infoWindow) { infoWindow.close(); }

        // Open the info windows for the marker we are panning to
        infoWindow = new google.maps.InfoWindow({
            content: generateInfoWindow(locationIndex)
        });
        infoWindow.open(map, markers[locationIndex]);
    });

    $(document).on('click', '.map_tough', function () {
        var addr, zip, latLng;
        // Initiate our gecoder in case we need to for geoed locations
        geocoder = new google.maps.Geocoder();
        addr = $(this).data('address');
        geocoder.geocode({ 'address': addr }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                map.panTo(results[0].geometry.location);
            } else {
                displayError('There was an error while processing your request.');
            }
        });
    });

    $(document).on('click', '.directions_easy', function () {
        var location = $(this);
        $('#dir_destination').val($(location).data('address'));
        $('#dir_name').val($(location).parent().parent().find('.name').html());
        $('#dir_lat').val($(location).data('lat'));
        $('#dir_long').val($(location).data('long'));
        promptForAddress();
    });

    $(document).on('click', '#getDirections', function () {
        loadDirectionsMap();
        loadDirections();
        return false;
    });

    $(document).on('click', 'span.send_email', function (e) {
        e.preventDefault(); // Stop link from firing
        var email = $(this).attr('data-email');
        $('#content-container').modal({
            containerCss: {
                backgroundColor: '#ffffff',
                borderColor: '#ffffff',
                height: '800px',
                width: '550px',
                padding: '10px'
            }, onClose: function (dialog) {
                if (modelClosingAction === 'showThanks') {
                    $('#simplemodal-overlay').remove();
                    $('#simplemodal-container').remove();
                    showThanks();
                } else {
                    dialog.data.fadeOut('400', function () {
                        dialog.container.hide('400', function () {
                            dialog.overlay.slideUp('fase', function () {
                                $.modal.close();
                            });
                        });
                    });
                }
            }
        });
        $('div.simplemodal-container form.simple-form input#to').val(email);
        $('div.simplemodal-container form.simple-form input[type=submit]').css('display', 'inline-block').after('<a href="javascript:$.modal.close()" title="Close" style="margin-left:10px;vertical-align:bottom;">Close</a>');

    });

    locations = $.parseJSON($('#json').val());
    loadScript();
});