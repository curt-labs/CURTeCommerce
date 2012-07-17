/* Author: Alex Ninneman @ninnemana */

$(function () {

    $('.btnLogin').click(function (e) {
        var u, p;
        u = $('.username').val();
        p = $('.password').val();
        $('.username').parent().find('span').remove();
        $('.username').removeClass('err');
        $('.password').parent().find('span').remove();
        $('.password').removeClass('err');

        if (p.length === 0) {
            e.preventDefault();
            var warning = $('.password').attr('title');
            $('.password').after('<span>' + warning + '</span>');
            $('.password').addClass('err').focus();
        }

        if (u.length === 0) {
            e.preventDefault();
            var warning = $('.username').attr('title');
            $('.username').after('<span>' + warning + '</span>');
            $('.username').addClass('err').focus();
        }
    });

    $('.btnForgot').click(function (e) {
        var email = $('.email').val();
        $('.email').parent().find('span').remove();
        $('.email').removeClass('err');

        if (email.length === 0) {
            e.preventDefault();
            var warning = $('.email').attr('title');
            $('.email').after('<span>' + warning + '</span>');
            $('.email').addClass('err').focus();
        }
    });
});