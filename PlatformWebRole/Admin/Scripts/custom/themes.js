var fileWindow, deleteFile, initSortable, destroySortable, updateSort, fileList, previewTheme, stopPreviewTheme, loaderwindow, doUpload, upload;
$(function () {
    $(document).on('click', '.deletetheme', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        if (confirm('Are you sure you want to delete this theme?')) {
            var modalwindow = showLoader("Deleting...")
            $.post('/Admin/Themes/Delete', { id: id }, function (data) {
                modalwindow.close();
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
            var modalwindow = showLoader("Activating...")
            $.post('/Admin/Themes/Activate', { id: id }, function (data) {
                if (data.success) {
                    modalwindow.close();
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

    $(document).on('click', '#addexternalfile', function (e) {
        e.preventDefault();
        var areaID = $(this).data('areaid');
        var typeID = $(this).data('typeid');
        var themeID = $(this).data('themeid');
        var filename = prompt("Enter the file full path", "");
        if (filename != null) {
            $.post('/Admin/Themes/SaveFile', { themeID: themeID, areaID: areaID, typeID: typeID, content: "", name: filename, fileID: 0, externalFile: true }, function (data) {
                $.getJSON('/Admin/Themes/AreaFiles', { themeID: themeID, areaID: areaID, typeID: typeID }, function (response) {
                    $('#thememanagerfiles').empty();
                    var html = fileList(response);
                    $('#thememanagerfiles').append(html);
                });
            });
        }
    });

    $(document).on('click', '.deletethemefile a', deleteFile);
    $(document).on('click', '#preview', previewTheme);
    $(document).on('click', '#endpreview', stopPreviewTheme);

    $(document).on('click', '.duplicatetheme', function (e) {
        e.preventDefault();
        var href = $(this).attr('href');
        if (confirm('Duplicate this theme?')) {
            var modalwindow = showLoader("Duplicating Theme... This takes a while.")
            $.post(href, function (data) {
                var themediv = '<div class="theme" id="theme-' + data.ID + '">';
                themediv += '<a href="/Admin/Themes/Files/' + data.ID + '" class="themename" title="Manage Theme Files">' + data.name + '</a>';
                themediv += '<span class="themeimg">';
                themediv += '<a href="/Admin/Themes/Activate/' + data.ID + '" data-id="' + data.ID + '" title="Not Active Theme, click to Activate" class="activate"><span class=inactive></span></a>';
                themediv += '<a href="/Admin/Themes/Delete/' + data.ID + '" data-id="' + data.ID + '" class="deletetheme" title="Delete Theme">&times;</a>';
                themediv += '<a href="/Admin/Themes/Edit/' + data.ID + '" title="Edit Theme Basic Information" class="edittheme"><span class="pencil"></span></a>';
                themediv += '<a href="/Admin/Themes/Duplicate/' + data.ID + '" title="Duplicate Theme" class="duplicatetheme"><span class="copy"></span></a>';
                themediv += '<a href="/Admin/Themes/Files/' + data.ID + '" class="themefiles" title="Manage Theme Files">';
                themediv += '<img src="' + ((data.screenshot != null && data.screenshot != '') ? data.screenshot : '/Admin/Content/img/noimage.jpg' ) + '" alt="' + data.name + '" />';
                themediv += '</a></span></div>';
                $('.theme:last').after(themediv);
                modalwindow.close();
            }, 'json');
        }
    });



});

fileWindow = function (data) {
    var html = '<div id="thememanager"><a href="javascript:$.modal.close()" title="Close" class="modalClose">&times;</a>'
    html += '<h3>' + data.area.name + ': ' + data.type.name + ' Files</h3>';
    html += '<p>Files are shown in their render order. Drag and Drop to re-order them.</p>';
    html += '<ul id="thememanagerfiles">';
    html += fileList(data);
    html += '</ul>';
    html += '<div id="dropzone" data-themeid="' + data.themeID + '" data-areaid="' + data.area.ID + '" data-typeid="' + data.type.ID + '"><span id="droptext">Drop Here</span><progress id="loader" value="0" max="100" style="display: none;"></progress></div>';
    html += '<button class="btn btn-inverse" id="addfile" data-themeid="' + data.themeID + '" data-areaid="' + data.area.ID + '" data-typeid="' + data.type.ID + '" type="button"><i class="icon-plus icon-white"></i> Add File</button>';
    html += '<button class="btn btn-inverse" id="addexternalfile" data-themeid="' + data.themeID + '" data-areaid="' + data.area.ID + '" data-typeid="' + data.type.ID + '" type="button"><i class="icon-plus icon-white"></i> Add External File</button></div>';
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
            killDnD();
            $.modal.close();
        }
    });
    initSortable();
    initDnD();
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

fileList = function (data) {
    var html = "";
    if (data.files.length > 0) {
        $(data.files).each(function (i, obj) {
            html += '<li id="file_' + obj.ID + '" data-themeid="' + obj.themeID + '" data-areaid="' + obj.ThemeAreaID + '" data-typeid="' + obj.ThemeFileTypeID + '">';
            html += '<span class="handle">&bull;</span> ' + ((obj.externalFile) ? '<span class="filename">' + obj.filePath + '</span>' : '<a href="/Admin/Themes/EditFile/' + obj.ID + '" title="Edit File">' + obj.filePath + '</a>');
            html += '<span class="deletethemefile"><a href="/Admin/Themes/DeleteFile/' + obj.ID + '">&times;</a></span>';
            html += '</li>';
        });
    } else {
        html += '<li>No files</li>';
    }
    return html;
}

previewTheme = function (e) {
    e.preventDefault();
    var themeID = $(this).data('themeid');
    if (confirm('Are you sure you want to preview this theme?')) {
        var modalwindow = showLoader("Processing...")
        $.post('/Admin/Themes/Preview/', { id: themeID }, function (data) {
            if (data.success) {
                showMessage('Theme is now in preview mode. It will be visible to you for the next five minutes.');
                $('.theme').addClass('preview');
                modalwindow.close();
            } else {
                showMessage('Preview failed. Please try again.');
            }
        }, 'json')
    }
}

stopPreviewTheme = function (e) {
    e.preventDefault();
    var themeID = $(this).data('themeid');
    if (confirm('Are you sure you want to stop previewing this theme?')) {
        $.post('/Admin/Themes/EndPreview/', { id: themeID }, function (data) {
            if (data.success) {
                showMessage('Preview stopped.');
                $('.theme').removeClass('preview');
            } else {
                showMessage('Stopping Preview failed. Please try again.');
            }
        }, 'json')
    }
}

showLoader = function (msg) {
    var html = '<div style="width:100%;text-align:center;margin-top:100px;">';
    html += '<img src="/Content/img/large-loader.gif" alt="Loading..." style="width:140px" />';
    html += '<span style="font-size: 24px;display:block;margin-top:25px">' + msg + '</span>';
    html += '</div>';
    var modalwindow;
    modalwindow = $.modal(html, {
        containerId: 'simplemodal-loader',
        containerCss: {
            backgroundColor: '#ffffff',
            borderColor: '#393939',
            height: '400px',
            width: '800px',
            padding: '10px'
        }, overlayCss: {
            backgroundColor: 'black'
        }, onClose: function (dialog) {
            dialog.data.fadeOut('400', function () {
                dialog.container.hide('400', function () {
                    dialog.overlay.slideUp('fase', function () {
                        $.modal.close();
                    });
                });
            });
        }
    });
    return modalwindow;
};

upload = function (event) {
    event.stopPropagation();
    event.preventDefault();
    $('#dropzone').css("background-color", "transparent");
    var files = event.dataTransfer.files;
    var themeid = $('#dropzone').data('themeid');
    var areaid = $('#dropzone').data('areaid');
    var typeid = $('#dropzone').data('typeid');
    for (var i = 0; i < files.length; i++) {
        doUpload(files[i], themeid, areaid, typeid);
    }
}

doUpload = function (file, themeID, areaID, typeID) {
    // Uploading - for Firefox, Google Chrome and Safari
    xhr = new XMLHttpRequest();
    // Add progress event listener
    xhr.upload.addEventListener("progress", function (evt) {
        if (evt.lengthComputable) {
            $('#droptext').hide();
            $('#loader').show();
            var loaded_pct = (evt.loaded / evt.total) * 100;
            $('#loader').attr('value', loaded_pct);
        } else {
            $('#loader').hide();
            $('#droptext').show();
        }
    }, false);

    // Add event listener for the completed loading
    xhr.addEventListener("load", function (resp) {
        if (resp != undefined && resp.currentTarget !== undefined && resp.currentTarget.response !== undefined) {
            $.getJSON('/Admin/Themes/AreaFiles', { themeID: themeID, areaID: areaID, typeID: typeID }, function (response) {
                $('#thememanagerfiles').empty();
                var html = fileList(response);
                $('#thememanagerfiles').append(html);
            });
        } else {
            showMessage('Error: Invalid file data.');
        }
        $('#loader').attr('value', '0');
        $('#loader').hide();
        $('#droptext').show();
    }, false);

    xhr.open('post', '/Admin/Themes/Upload?themeID=' + themeID +'&areaID=' + areaID + '&typeID=' + typeID, true);

    // Set appropriate headers
    xhr.setRequestHeader("Content-Type", "multiplart/form-data");
    xhr.setRequestHeader("X-File-Name", file.name);
    xhr.setRequestHeader("X-File-Size", file.size);
    xhr.setRequestHeader("X-File-Type", file.type);

    xhr.send(file);

}

initDnD = function () {
    if ($.browser.msie || !Modernizr.draganddrop) {
        $('#dropzone').remove();
        //$('#form_container').after('<p style="color:red">You\'re browser does not support drag and drop file uploading. Please upgrade to a different browser <br />(We recommend <a href="http://www.google.com/chrome" target="_blank">Google Chrome</a>.)</p>');
    } else {
        if ($('#dropzone').length > 0) {
            $('#dropzone').get(0).addEventListener('drop', upload, false);
            $('#dropzone').get(0).addEventListener('dragover', function (event) {
                event.preventDefault();
                $('#dropzone').css("background-color", "#ffc");
            }, false);
            $('#dropzone').get(0).addEventListener('dragleave', function (event) {
                event.preventDefault();
                $('#dropzone').css("background-color", "transparent");
            }, false);
        }
    }
}

killDnD = function () {
    $('#dropzone').get(0).removeEventListener('drop', this, false);
    $('#dropzone').get(0).removeEventListener('dragover', this, false);
    $('#dropzone').get(0).removeEventListener('dragleave', this, false);
}
