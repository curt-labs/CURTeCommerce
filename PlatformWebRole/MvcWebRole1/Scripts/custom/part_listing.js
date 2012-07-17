/*global Shadowbox */
var max_content_height = 0;
var previous_html = "";
$(function () {
    $('.part_content').css('position', 'absolute').css('top', '0px').css('left', '0px');

    Shadowbox.init();

    $('.part_tab_container').show();
    $('#part_container .part_groups').find('.part_content').hide();
    $('#part_container .part_groups').find('.part_content:first').show();

    $('.tab a').click(function () {
        $('.tab').removeClass('active');
        $(this).parent().addClass('active');

        var tab = $(this).attr('id').split(':')[1];
        $('#part_container .part_groups').find('.part_content').fadeOut(600);
        $('#part_container .part_groups').find('#' + tab).fadeIn(600);
    });

    $.each($('.part_content'), function (i, content) {
        var parts = $(content).find('.part');
        if ((parts.length * 450) > max_content_height) {
            max_content_height = (parts.length * 430);
        }
    });
    $('.part_groups').css('height', max_content_height + 'px');
});