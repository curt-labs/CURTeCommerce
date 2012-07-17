/* Author: Alex Ninneman @ninnemana */

$(function () {

    if (formErrorObject == undefined) {
        formErrorObject = new Object();
    }

    var form_title = $('span.title').text();
    if (form_title.toUpperCase().indexOf('ADD') == -1) {
        var html = '<a href="javascript:void(0)" class="change_pass" title="Change Password">Change Password</a><span class="change_pass_note">This will override your existing password</span>';
        $('label[for=p1]').before(html);
    } else {
        $('.admin_form label[for=p1],.admin_form label[for=p2]').show();
    }

    $('.change_pass').live('click', function (e) {
        e.preventDefault();
        $('a.change_pass,span.change_pass_note').remove();
        $('label[for=p1],label[for=p2]').slideDown();
        validatePasswords();
    });

    $('#p1,#p2').live('keyup', function () {
        validatePasswords();
    });

    // Compare password fields for equality
    var validatePasswords = (function () {
        var p1 = $.trim($('#p1').val());
        var p2 = $.trim($('#p2').val());
        if (p1 != p2) {
            $('#p1,#p2').addClass('err');
            if ($('.err_msg').length == 0) {
                $('#p1').before('<span class="err_msg">Passwords do not match</span>');
            }
            formErrorObject.passwords = 'Passwords do not match';
        } else {
            $('#p1,#p2').removeClass('err');
            if (formErrorObject.passwords != undefined) {
                delete formErrorObject.passwords;
            }
        }
    });

    $('.profile_image').live('click', function () {
        if (confirm('Are you sure you want to remove your profile image?')) {
            $.get('/Admin/Profile/DeleteProfileImage', function (resp) {
                if (resp.length == 0) {
                    $('.profile_image').remove();
                } else {
                    showMessage('There was an error while deleting the profile image.');
                }
            });
        }
    });

    $('#deleteProfile').live('click', function () {
        if (confirm('Are you sure you want to remove this profile?\r\n\r\nThis cannot be undone.')) {
            return true;
        }
        return false;
    });
});