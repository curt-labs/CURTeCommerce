$(function () {
    $(document).on('click', '.deletetheme', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        if (confirm('Are you sure you want to delete this theme?')) {
            $.post('/Admin/Themes/Delete', { id: id }, function (data) {
                if (data.success) {
                    $('#theme-' + id).fadeOut('fast', function () {
                        $('#theme-' + id).remove();
                    });
                    showMessage("Theme removed successfully.")
                } else {
                    showMessage("There was a problem removing the theme.")
                }
            }, 'json');
        }
    });

    $(document).on('click', '.activate', function (e) {
        e.preventDefault();
        var alink = $(this);
        var id = $(this).data('id');
        if (confirm('Make this theme the active theme?')) {
            $.post('/Admin/Themes/Activate', { id: id }, function (data) {
                if (data.success) {
                    // set all other active themes to inactive
                    $('.theme.active').each(function (i, obj) { $(this).removeClass('active'); });
                    $('.activate.active').each(function (i, obj) { $(this).attr('title','Not Active Theme, click to Activate').removeClass('active'); });
                    $('.activate span.active').each(function (i, obj) { $(this).removeClass('active').addClass('inactive'); });
                    // set this theme to active;
                    $('#theme-' + id).addClass('active');
                    $(alink).addClass('active').attr('title','Active Theme').find('span').removeClass('inactive').addClass('active');
                    showMessage("Theme Activated.")
                } else {
                    showMessage("There was a problem activating the theme.")
                }
            }, 'json');
        }
    });
});