/*global displayError, confirm */
function validateReview() {
    var err, sub, rev;
    err = '';
    sub = $('#rsubject').val();
    rev = $('textarea[name=review]').val();

    if (rev.length === 0) {
        $('textarea[name=review]').addClass('error').focus();
        err = 'You must enter a review.';
    } else {
        $('textarea[name=review]').removeClass('error');
    }

    if (sub.length === 0) {
        $('#rsubject').addClass('error').focus();
        err = 'You must enter a subject.';
    } else {
        $('#rsubject').removeClass('error');
    }
    return err;
}

$(function () {
    var ra_button = document.createElement("a");
    $(ra_button).addClass("ra_button").attr('href', '#').attr('title', 'Submit Anonymous').html('Submit Anonymous');
    $('input[name=btnReview]').after(ra_button);


    $('#btnReview').click(function () {
        var err = validateReview();
        if (err.length > 0) {
            displayError(err);
            return false;
        }
    });

    $('.ra_button').live('click', function (e) {
        var err, rev, rating;
        e.preventDefault();
        if (confirm('Are you sure you want to submit this review anonymously?')) {
            err = validateReview();
            if (err.length > 0) {
                displayError(err);
            } else {
                rev = $('textarea[name=review]').val();
                rating = $('#rating').val();
                window.location = window.location.protocol + '//' + window.location.host + $('.new_review form').attr('action') + '?review=' + rev + '&rating=' + rating;
            }
        }
    });

});