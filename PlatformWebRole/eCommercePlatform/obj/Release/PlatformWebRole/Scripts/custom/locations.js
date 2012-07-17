var map, markers, locations, infoWindow, geocoder, directionsDisplay,directionsService;
directionsService = new google.maps.DirectionsService();
$(function () {

    var displayLoader = function () {
        var width = $('#mid_col').width();
        var height = $('#mid_col').height();
        $('#mid_col').css('position', 'relative');
        var loader = document.createElement('div');
        $(loader).addClass('loader').css('background', 'white').css('z-index', '2000').css('position', 'absolute').css('top', '0').css('left', '0').css('height', height).css('width', width).css('opacity', '0.8').css('text-align', 'center');
        var img = document.createElement('img');
        $(img).attr('src', '/Content/img/159.gif').css('width', '175px').css('display', 'block').css('margin', '60px auto 10px auto');
        $(loader).append(img);
        var text = document.createElement('span');
        $(text).css('font-size', '22px').text('Calculating your location...');
        $(loader).append(text);
        $('#mid_col').append(loader);
    }

    var hideLoader = function () {
        $('div.loader').fadeOut(400, function () {
            $(this).remove();
        });
    }

    var loadMap = function (lat, long, zoom) {
        var opts = {
            center: new google.maps.LatLng(lat, long),
            zoom: zoom,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('map'), opts);
        $('#map').show();

        directionsDisplay.setMap(map);
        loadMarkers()
    };

    var loadMarkers = function () {
        markers = new Array();
        infoWindows = new Array();
        for (i = 0; i < locations.length; i++) {
            var marker = new google.maps.Marker({
                position: new google.maps.LatLng(locations[i].latitude, locations[i].longitude),
                map: map,
                title: locations[i].name,
                animation: google.maps.Animation.DROP
            });
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
    };

    // Create the content for the InfoWindow
    var generateInfoWindow = function (index) {
        var location = locations[index]; // Get the location we're going to pull the content from
        if (location != undefined) {

            var contentString = '<div class="infoWindow">';
            contentString += '<span class="title">' + location.name + '</span>';
            contentString += '<span class="directions_easy" data-address="' + location.address + ' ' + location.city + ',' + location.state + '"  data-lat="' + location.latitude + '" data-long="' + location.longitude + '">';
            contentString += '<img src="/Content/img/directions.jpg" alt="Get Directions" />';
            contentString += 'Get Directions</span>';
            if (location.phone != undefined) {
                contentString += '<span>';
                contentString += '<img src="/Content/img/phone-icon.png" alt="Phone: ' + location.phone + '">';
                contentString += 'Phone: ' + location.phone;
                contentString += '</span>';
            }
            if (location.fax != undefined) {
                contentString += '<span>';
                contentString += '<img src="/Content/img/Fax.png" alt="Fax: ' + location.fax + '">';
                contentString += 'Fax: ' + location.fax;
                contentString += '</span>';
            }
            if (location.email != undefined) {
                contentString += '<span class="send_email" data-email="' + location.email + '">';
                contentString += '<img src="/Content/img/footer/mail.png" alt="E-Mail: ' + location.email + '">';
                contentString += 'E-Mail: ' + location.email;
                contentString += '</span>';
            }

            contentString += '</div>';

        }
        return contentString;
    };

    var handleGeolocator = function (pos) {
        displayLoader()

        $.post('/Locations/GetNearest', { 'lat': pos.coords.latitude, 'lon': pos.coords.longitude }, function (resp) {
            if (resp.length == 0) {
                loadMap(locations[0].latitude, locations[0].longitude, 4)
            } else {
                $.each(resp, function (i, location) {
                    var html = $('#' + location.locationID).clone().wrap('<div>').parent().html();
                    $('#' + location.locationID).remove();
                    if (i === 0) {
                        map.panTo(new google.maps.LatLng(location.latitude, location.longitude));
                        hideLoader();
                        map.setZoom(9);
                    }
                    $('.location').last().after(html.toString()).show();
                });
            }
        }, 'json');
    };

    var promptForAddress = function (loc) {

        $.getJSON('/GlobalFunctions/GetCountries', function (countries) {
            if (countries !== undefined && countries.length > 0) {
                var html;
                html = '<link href="/Content/view_sass/share-email.css" media="all" rel="stylesheet" />';
                html += '<span class="share-heading">Enter your address for a start location.</span>';
                html += '<form class="share-form">';
                html += '<input type="hidden" id="destination" value="' + loc + '" />';
                html += '<label for="addr">Address';
                html += '<input type="text" id="directAddr" placeholder="Enter your address..." /></label>';
                html += '<label for="city">City';
                html += '<input type="text" id="city" placeholder="Enter your city..." /></label>';
                html += '<label for="state">State';
                html += '<select id="state">';
                $.each(countries, function (i, country) {
                    html += '<optgroup label="' + country.name + '">';
                    $.each(country.states, function (i, state) {
                        html += '<option value="' + state.abbr + '">' + state.state + '</option>';
                    });
                    html += '</optgroup>';
                });
                html += '</select>';
                html += '</label>';
                html += '<div class="clearfix"></div>';
                html += '<input type="submit" id="getDirections" value="Get Directions" style="display:inline-block" />';
                html += '<a href="javascript:$.modal.close()" title="Close" style="margin-left:10px;vertical-align:bottom;">Close</a>';
                html += '</form>';
                $.modal(html, {
                    containerCss: {
                        backgroundColor: '#ffffff',
                        borderColor: '#ffffff',
                        height: '500px',
                        width: '400px',
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
        var lat, long, id, locationIndex;

        // Get the attributes of this location
        lat = $(this).data('lat');
        long = $(this).data('long');
        id = $(this).data('id');

        // Pan the map to it's location
        map.panTo(new google.maps.LatLng(lat, long));

        // Get the location that matches this locationID
        for (i = 0; i < locations.length; i++) {
            if (locations[i].locationID === id) {
                locationIndex = $.inArray(locations[i], locations);
                break;
            }
        }

        // If our infoWindow has been popualated, close that shit :)
        if (infoWindow) { infoWindow.close() }

        // Open the info windows for the marker we are panning to
        infoWindow = new google.maps.InfoWindow({
            content: generateInfoWindow(locationIndex)
        });
        infoWindow.open(map, markers[locationIndex]);
    });

    $('.map_tough').live('click', function () {
        var addr, zip, latLng;
        addr = $(this).data('address');
        geocoder.geocode({ 'address': addr }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                map.panTo(results[0].geometry.location);
            } else {
                displayError('There was an error while processing your request.');
            }
        });
    });

    $('.directions_easy').live('click', function () {
        var loc = $(this).data('address');
        promptForAddress(loc);
    });

    $('#getDirections').live('click', function () {
        $.modal.close();
        var destination, from_addr, from_city, from_state, request;
        destination = $('#destination').val();
        from_addr = $('#directAddr').val();
        from_city = $('#city').val();
        from_state = $('#state').val();
        request = {
            origin: from_addr + ' ' + from_city + ',' + from_state,
            destination: destination,
            travelMode: google.maps.TravelMode.DRIVING
        };
        directionsService.route(request, function (response, status) {
            if (status === google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(response);
            }
        });
        return false;
    });

    $('span.send_email').live('click', function (e) {
        e.preventDefault(); // Stop link from firing
        var email = $(this).attr('data-email');
        var html = $('#content-container').html();
        //console.log(html);
        $.modal(html, {
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

        /*$('div.simplemodal-container form.simple-form input[type=submit]').live('click', function (e) {
            $('#simplemodal-container').find('span.error').remove();
            e.preventDefault();
            var action, opts;
            opts = new Object();
            action = $('div.simplemodal-container form.simple-form').attr('action');
            opts.name = $('div.simplemodal-container form.simple-form #name').val();
            opts.phone = $('div.simplemodal-container form.simple-form #phone').val();
            opts.email = $('div.simplemodal-container form.simple-form #email').val();
            opts.to = $('div.simplemodal-container form.simple-form #to').val();
            opts.message = $('div.simplemodal-container form.simple-form #message').val();
            opts.response_field = $('div.simplemodal-container form.simple-form #recaptcha_response_field').val();
            opts.challenge_field = $('div.simplemodal-container form.simple-form #recaptcha_challenge_field').val();

            $.post(action, opts, function (resp) {
                if (resp.length > 0) {
                    $('div.simplemodal-container form.simple-form span.title').after('<span class="error" style="color:red;font-size:13px;font-weight:bold">' + resp + '</span>');
                } else {
                    $.modal.close();
                    alert('Thank you for the contact inqury, someone will respond to your request soon.');
                }
            });
            return false;
        });*/
        return false;
    });

    locations = $.parseJSON($('#json').val());

    if (Modernizr.geolocation) {
        // browser supports HTML5 geolocation
        navigator.geolocation.getCurrentPosition(handleGeolocator);
    }

    // Initiate our directions object
    directionsDisplay = new google.maps.DirectionsRenderer();

    // No geo support
    // Load the center of US
    // Latitude 38
    // Longitude -97
    loadMap('38', '-97', 4)

    // Initiate our gecoder in case we need to for geoed locations
    geocoder = new google.maps.Geocoder();


});