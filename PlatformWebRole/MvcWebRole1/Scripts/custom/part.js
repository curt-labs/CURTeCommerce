/*global Shadowbox */
var content_height = 0;
var max_height = 0;
var changeHashNoScroll;

function changePartTab(id) {
    if (id.length > 0) {
        var atab = $('.part_tab_container ul li a.active');
        $('.part_tab_container ul li a').removeClass('active');

        $('.part_content').fadeOut();
        if (id.indexOf('#') === -1) {
            id = '#' + id;
        }
        $('a[href=' + id + ']').addClass('active');
        $(id).fadeIn();
        $('.part_container').css('height', $(id).height() + 30);
    } else {
        $('.part_tab_container ul li a').removeClass('active');
        $('.part_tab_container ul li a:first').addClass('active');
        if ($('.part_content:first').css('display') !== 'block') {
            $('.part_content').fadeOut();
            $('.part_content:first').fadeIn();
            $('.part_container').css('height', $('.part_content:first').height() + 30);
        }
    }
}

$(function () {
    Shadowbox.init();

    $('#vehicles table').dataTable({
        'bFilter': false,
        'bPaginate': false
    });

    $('.part_tab_container').show();
    $('.part_content').hide();
    $('.part_content:first').show();
    $('.part_container').css('min-height', $('.part_content:first').height() + 30);
    $('.part_content').css('position', 'absolute').css('left', '0').css('top', '0');

    $('#vehicles table tr').mouseover(function () {
        $(this).css('cursor', 'pointer');
    });

    $('#vehicles table tbody tr').live('click', function () {
        var year, make, model, style = 0;
        year = $(this).find('td:nth-child(1)').html();
        make = $(this).find('td:nth-child(2)').html();
        model = $(this).find('td:nth-child(3)').html();
        style = $(this).find('td:nth-child(4)').html();

        window.location.href = window.location.protocol + '//' + window.location.host + '/Lookup/' + encodeURIComponent(year) + '/' + encodeURIComponent(make) + '/' + encodeURIComponent(model) + '/' + encodeURIComponent(style.replace(/\//g, '!'));
    });

    $(document).on('click', '#readmore', function (e) {
        e.preventDefault();
        var infotab = $('#info');
        changeHashNoScroll("info");
        $('html, body').animate({
            scrollTop: $("#info").offset().top - Math.floor($(window).height() / 2.5) + 'px'
        }, 'fast');
    });



    /**** 
    *  USE JQUERY-BBQ to handle hashing and back button nav 
    *******/

    // Keep a mapping of url-to-container for caching purposes.
    /*var cache = {
        // If url is '' (no fragment), display this div's content.
        '': $('.bbq-default')
    };*/

    // Bind an event to window.onhashchange that, when the history state changes,
    // gets the url from the hash and displays either our cached content or fetches
    // new content to be displayed.
    $(window).bind('hashchange', function (e) {

        // Get the hash (fragment) as a string, with any leading # removed. Note that
        var url = $.param.fragment();

        // Fire click on the tab that is being referenced
        changePartTab(url);

    });

    // Since the event is only triggered when the hash changes, we need to trigger
    // the event now, to handle the hash the page may have loaded with.
    $(window).trigger('hashchange');

    /**** 
    *  END JQUERY-BBQ
    *******/

});

changeHashNoScroll = function (hashval) {
    window.location.hash = "info";
    return false;
};