var year, make, model, style = 0;
var mount = '';

function resetLookup() {
    mount = '';
    year = 0;
    make = 0;
    model = 0;
    style = 0;
    $('#mount').val('');
    $('#year').html('<option value="0">- Select Year -</option>').attr('disabled','disabled');
    $('#make').html('<option value="0">- Select Make -</option>').attr('disabled', 'disabled');
    $('#model').html('<option value="0">- Select Model -</option>').attr('disabled', 'disabled');
    $('#style').html('<option value="0">- Select Style -</option>').attr('disabled', 'disabled');
    $('#lookup label span').hide();
    $('#lookup label select').removeClass('err');
}

$(function () {

    $('#lookup').find('select').attr('disabled', 'disabled');
    $('#lookup').find('select:first').removeAttr('disabled');
    $('#reset_lookup').click(function () { resetLookup(); });

    $('#mount').change(function () {
        $('#lookup label img').css('display', 'none');
        $('#year').next().css('display', 'inline');
        year = make = model = null;
        $('#year', '#make', '#model', '#style').attr('disabled', 'disabled');
        $('#year').html('<option value="">- Select Year -</option>');
        $('#make').html('<option value="0">- Select Make -</option>');
        $('#model').html('<option value="0">- Select Model -</option>');
        $('#style').html('<option value="0">- Select Style -</option>');
        mount = $(this).val();
        if (mount.length !== 0 && mount !== null) {
            $(this).removeClass('err').prev().hide();
        }
        $.getJSON('https://api.curtmfg.com/v2/getyear?callback=?', { 'mount': mount, 'dataType': 'JSONP' }, function (years) {
            $.each(years, function (i, year) {
                var opt = document.createElement('option');
                $(opt).text(year);
                $('#year').append(opt);
            });
            $('#year').removeAttr('disabled').focus();
            $('#lookup label img').css('display', 'none');
        });
    });

    $('#year').change(function () {
        $('#lookup label img').css('display', 'none');
        $('#make').next().css('display', 'inline');
        make = model = null;
        $('#make', '#model', '#style').attr('disabled', 'disabled');
        $('#make').html('<option value="0">- Select Make -</option>');
        $('#model').html('<option value="0">- Select Model -</option>');
        $('#style').html('<option value="0">- Select Style -</option>');
        year = $(this).val();
        if (year !== 0 && year !== null) {
            $(this).removeClass('err').prev().hide();
        }
        $.getJSON('https://api.curtmfg.com/v2/getmake?callback=?', { 'mount': mount, 'year': year, 'dataType': 'JSONP' }, function (makes) {
            $.each(makes, function (i, make) {
                var opt = document.createElement('option');
                $(opt).text($.trim(make));
                $('#make').append(opt);
            });
            $('#make').removeAttr('disabled').focus();
            $('#lookup label img').css('display', 'none');
        });
    });

    $('#make').change(function () {
        $('#lookup label img').css('display', 'none');
        $('#model').next().css('display', 'inline');
        model = null;
        $('#model', '#style').attr('disabled', 'disabled');
        $('#model').html('<option value="0">- Select Model -</option>');
        $('#style').html('<option value="0">- Select Style -</option>');
        make = $(this).val();
        if (make !== 0 && make !== null) {
            $(this).removeClass('err').prev().hide();
        }
        $.getJSON('https://api.curtmfg.com/v2/getmodel?callback=?', { 'mount': mount, 'year': year, 'make': make, 'dataType': 'JSONP' }, function (models) {
            $.each(models, function (i, model) {
                var opt = document.createElement('option');
                $(opt).text($.trim(model));
                $('#model').append(opt);
            });
            $('#model').removeAttr('disabled').focus();
            $('#lookup label img').css('display', 'none');
        });
    });

    $('#model').change(function () {
        $('#lookup label img').css('display', 'none');
        $('#style').next().css('display', 'inline');
        $('#style').attr('disabled', 'disabled');
        $('#style').html('<option value="0">- Select Style -</option>');
        model = $(this).val();
        if (model !== 0 && model !== null) {
            $(this).removeClass('err').prev().hide();
        }
        $.getJSON('https://api.curtmfg.com/v2/getstyle?callback=?', { 'mount': mount, 'year': year, 'make': make, 'model': model, 'dataType': 'JSONP' }, function (styles) {
            $.each(styles, function (i, style) {
                var opt = document.createElement('option');
                $(opt).text($.trim(style));
                $('#style').append(opt);
            });
            $('#style').removeAttr('disabled').focus();
            $('#lookup label img').css('display', 'none');
        });
    });

    $('#style').change(function () {
        $('#lookup input[type=submit]').removeAttr('disabled').focus();
        style = $(this).val();
        if (style !== 0 && style !== null) {
            $(this).removeClass('err').prev().hide();
        }
    });

    $('#lookup input[type=submit]').click(function (event) {
        event.preventDefault();
        style = $('#style').val();
        var passed = true;

        // Validate mount
        if (mount.length === '' || mount === null) {
            passed = false;
            $('#mount').addClass('err').prev().css('display', 'block');
        } else {
            $('#mount').removeClass('err').prev().hide();
        }

        // Validate year
        if (year === 0 || year === null) {
            passed = false;
            $('#year').addClass('err').prev().css('display', 'block');
        } else {
            $('#year').removeClass('err').prev().hide();
        }

        // Validate make
        if (make === 0 || make === null) {
            passed = false;
            $('#make').addClass('err').prev().css('display', 'block');
        } else {
            $('#make').removeClass('err').prev().hide();
        }

        // Validate model
        if (model === 0 || model === null) {
            passed = false;
            $('#model').addClass('err').prev().css('display', 'block');
        } else {
            $('#model').removeClass('err').prev().hide();
        }

        // Validate style
        if (style === 0 || style === null) {
            passed = false;
            $('#style').addClass('err').prev().css('display', 'block');
        } else {
            $('#style').removeClass('err').prev().hide();
        }

        // Redirect the page to the results
        if (passed) {
            window.location.href = window.location.protocol + '//' + window.location.host + '/Lookup/' + encodeURIComponent(year) + '/' + encodeURIComponent(make) + '/' + encodeURIComponent(model) + '/' + encodeURIComponent(style.replace(/\//g, '!'));
        }
        return false;
    });

    if ($('#year').val() > 0) {
        $('#year').change();
    }

    $('#curt_product').cycle('fade');

    if ($.browser.msie && $.browser.version < 9) {
        var style_width = $('#style').width() + 5;
        $('#style').focus(function () {
            $(this).css('width', 'auto');
        }).blur(function () {
            $(this).css('width', style_width + 'px');
        });
    }

});