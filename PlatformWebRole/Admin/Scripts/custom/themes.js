var fileWindow, deleteFile, initSortable, destroySortable, updateSort;
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

    $(document).on('click', '.themearea a', function (e) {
        e.preventDefault();
        var areaID = $(this).data('areaid');
        var typeID = $(this).data('typeid');
        var themeID = $('#themeID').val();
        $.getJSON('/Admin/Themes/AreaFiles', { themeID: themeID, areaID: areaID, typeID: typeID }, fileWindow);
    });

    $(document).on('click', '#addfile', function (e) {
        e.preventDefault();
        var areaID = $(this).data('areaid');
        var typeID = $(this).data('typeid');
        var themeID = $(this).data('themeid');
        window.location = '/Admin/Themes/AddFile?themeID=' + themeID + '&typeID=' + typeID + '&areaID=' + areaID;
    });

    $(document).on('click', '.deletethemefile a', deleteFile);
});

fileWindow = function (data) {
    var html = '<div id="thememanager"><a href="javascript:$.modal.close()" title="Close" class="modalClose">&times;</a>'
    html += '<h3>' + data.area.name + ': ' + data.type.name + ' Files</h3>';
    html += '<p>Files are shown in their render order. Drag and Drop to re-order them.</p>';
    html += '<ul id="thememanagerfiles">';
    if (data.files.length > 0) {
        $(data.files).each(function (i, obj) {
            var filepath = obj.filePath.split('/');
            html += '<li id="file_' + obj.ID + '" data-themeid="' + obj.themeID + '" data-areaid="' + obj.ThemeAreaID + '" data-typeid="' + obj.ThemeFileTypeID + '">';
            html += '<span class="handle">&bull;</span> <a href="/Admin/Themes/EditFile/' + obj.ID + '" title="Edit File">' + filepath[filepath.length - 1] + '</a>';
            html += '<span class="deletethemefile"><a href="/Admin/Themes/DeleteFile/' + obj.ID + '">&times;</a></span>';
            html += '</li>';
        });
    } else {
        html += '<li>No files</li>';
    }
    html += '</ul>';
    html += '<button class="btn btn-inverse" id="addfile" data-themeid="' + data.theme.ID + '" data-areaid="' + data.area.ID + '" data-typeid="' + data.type.ID + '" type="button"><i class="icon-plus icon-white"></i> Add File</button></div>';
    $.modal(html, {
        containerCss: {
            backgroundColor: '#ffffff',
            borderColor: '#ffffff',
            height: '350px',
            width: '550px',
            padding: '10px'
        }, overlayCss: {
            backgroundColor: 'black'
        }, onClose: function (dialog) {
            $('div.simplemodal-container').fadeOut(400, function () {
                $('div.simplemodal-overlay').hide();
                $('div.simplemodal-container').hide();
            });
            $('input.modalchoosing').removeClass('modalchoosing');
            destroySortable();
            $.modal.close();
        }
    });
    initSortable();
};

deleteFile = function (e) {
    e.preventDefault();
    var aobj = $(this);
    var href = $(this).attr('href');
    if (confirm('Are you sure you want to delete this file?')) {
        $.post(href, function (data) {
            if (data.success) {
                var target = $(aobj).parent().parent();
                $(target).fadeOut('fast', function () { $(target).remove(); });
                showMessage("File Deleted.");
                initSortable();
            } else {
                showMessage("There was a problem deleting the file.")
            }
        }, 'json')
    }
}

updateSort = function() {
    var x = $('#thememanagerfiles').sortable("serialize");
    console.log(x);
    $.post('/Admin/Themes/updateFileSort?' + x);
}

initSortable = function () {
    $('#thememanagerfiles').sortable("destroy");
    $('#thememanagerfiles').sortable({ axis: "y", handle: 'span.handle', update: function (event, ui) { updateSort(event, ui) } }).disableSelection();
}

destroySortable = function () {
    $('#thememanagerfiles').sortable("destroy");
}