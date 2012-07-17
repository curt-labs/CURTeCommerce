$(function () {
    $(document).on("click",'li.year a.yearlink, li.year span', function () { expandArchive($(this).parent()) });
    $('li.year').each(function () {
        if ($(this).find('ul li')) {
            var yearlink = $(this).find('a.yearlink');
            $('<span class="arrow-right"></span>').insertBefore(yearlink);
            $(this).find('ul').hide();
        }
    });
});

function expandArchive(obj) {
    if ($(obj).find('ul li')) {
        // check if child ul is already visible
        if ($(obj).find('ul').is(':visible')) {
            $(obj).find('span.arrow-down').removeClass('arrow-down').addClass('arrow-right');
            $(obj).find('ul').slideUp();
        } else {
            $(obj).find('span.arrow-right').removeClass('arrow-right').addClass('arrow-down');
            $(obj).find('ul').slideDown();
        }
    }
}