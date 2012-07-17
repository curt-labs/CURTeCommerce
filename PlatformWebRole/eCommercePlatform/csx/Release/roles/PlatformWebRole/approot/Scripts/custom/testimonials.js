$(function () {
    /*restoreStarRating();

    $('#rate .star').click(function () {
        rating = $(this).attr('id').split('_')[1];
        $('#rating').val(rating);
        restoreStarRating();
    });

    $('#rate .star').hover(function () {
        var currate = Number($(this).attr('id').split('_')[1])
        for (var i = 1; i <= 5; i++) {
            if (i <= currate) {
                $('#star_' + i).attr('class', 'star hovered');
            } else {
                $('#star_' + i).attr('class', 'star');
            }
        }
    }, function () { restoreStarRating(); });*/
});

function restoreStarRating() {
    $('#rate span').each(function () { $(this).attr('class', 'star'); });
    rating = Number($('#rating').val());
    for (var i = 1; i <= rating; i++) {
        $('#star_' + i).addClass('selected');
    }
}