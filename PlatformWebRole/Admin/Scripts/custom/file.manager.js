var createFolder, deleteFolder, renameFolder, folder, folderTemplate, selectContainer, uploadFile, doUpload, deleteBlob, makeDroppable, hideElement;
folderTemplate = '<li class="ligallery" id="folder_{{Name}}"><a class="gallery" href="{{Path}}?name={{Name}}{{Qstring}}"><span class="folder contextmenu"></span><span class="galleryname"><strong>{{Name}}</strong> 0 subfolders; {{BlobCount}} files</span></a>{{#ShowDelete}}<ul class="menu"><li><a href="/Admin/FileManager/DeleteContainer?name={{Name}}" data-name="{{Name}}" class="deleteContainer">Delete</a></li></ul>{{/ShowDelete}}</li>';

$(function () {
    //127.0.0.1:81/Admin/FileManager/Folder?name=misc
    createFolder = (function (parent, qstring) {
        var pathstr = "/Admin/FileManager";
        if (qstring != "") {
            qstring = "&" + qstring;
            pathstr = "/Admin/FileManager/FileBrowser";
        }
        folder = prompt('Please enter the name of the folder.');
        if (folder !== undefined && folder !== null && folder.length > 0) {
            if (folder.indexOf('/') != -1) {
                alert('The folder name cannot contain any slashes.');
                createFolder();
                return false;
            }
            $.post('/Admin/FileManager/CreateContainer', { 'name': folder, 'parent': parent }, function (resp) {
                if (resp.Container !== null && resp.Container.Name !== null) {
                    if (parent != "") {
                        var view = { Path: pathstr, Name: folder, Container: resp.Container.Name, Qstring: qstring, BlobCount: resp.BlobCount, ShowDelete: false, ShowFiles: true };
                        var droppoint = ($('#galleries li.ligallery:last').length > 0) ? $('#galleries li.ligallery:last') : $('#dropzone');
                        $(droppoint).after(Mustache.to_html(folderTemplate, view));
                    } else {
                        var view = { Path: pathstr, Name: resp.Container.Name, Qstring: qstring, BlobCount: resp.BlobCount, ShowDelete: true };
                        var droppoint = ($('#galleries li.ligallery:last').length > 0) ? $('#galleries li.ligallery:last') : $('#dropzone');
                        $(droppoint).after(Mustache.to_html(folderTemplate, view));
                    }
                } else {
                    displayError(resp);
                }
            }, 'json');
        }
    });

    upload = function (event) {
        event.stopPropagation();
        event.preventDefault();
        $('#dropzone').css("background-color", "transparent");
        var files = event.dataTransfer.files;
        var container = $('#dropzone').data('uri');
        for (var i = 0; i < files.length; i++) {
            doUpload(files[i], container);
        }
    }

    doUpload = function (file, container) {
        // Uploading - for Firefox, Google Chrome and Safari
        xhr = new XMLHttpRequest();


        // Add progress event listener
        xhr.upload.addEventListener("progress", function (evt) {
            if (evt.lengthComputable) {
                $('#loader').show();
                var loaded_pct = (evt.loaded / evt.total) * 100;
                $('#loader').attr('value', loaded_pct);
            } else {
                $('#loader').hide();
            }
        }, false);

        // Add event listener for the completed loading
        xhr.addEventListener("load", function (resp) {
            if (resp != undefined && resp.currentTarget !== undefined && resp.currentTarget.response !== undefined) {
                var file_count = $('li.lifile').length;
                var gal_count = $('li.ligallery').length;
                var img_types = ["jpg", "png", "jpeg", "bmp", "gif"];
                var filepath = resp.currentTarget.response;
                var filesplit = filepath.split("/");
                var filename = filesplit[filesplit.length - 1];
                var isimg = false;
                for (var x = 0; x < img_types.length; x++) {
                    if (filename.split(".")[filename.split(".").length - 1].toLowerCase() == img_types[x]) {
                        isimg = true;
                    }
                }

                var html = '<li class="lifile"><span class="filebox">';
                if (isimg) {
                    html += '<img src="' + filepath + '" class="contextmenu" alt="' + filename + '" />';
                } else {
                    html += '<img src="/Admin/Content/img/FileNew.png" class="contextmenu" alt="' + filename + '" />';
                }
                html += '</span><span class="filename"><strong>' + filename + '</strong> path: <a href="' + filepath + '">link</a><br /></span>';
                html += '<ul class="menu"><li><a href="javascript:void(0)" title="Delete File" class="deleteBlob" data-uri="' + filepath + '">Delete File</a></li></ul></li>';

                if (file_count > 0) {
                    $('li.lifile:first').before(html);
                } else if (gal_count > 0) {
                    $('li.ligallery:last').after(html);
                } else {
                    $('#dropzone').after(html);
                }
            } else {
                showMessage('Error: Invalid file data.');
            }
            $('#loader').attr('value', '0');
            $('#loader').hide();
        }, false);

        xhr.open('post', '/Admin/FileManager/Upload?container=' + container, true);

        // Set appropriate headers
        xhr.setRequestHeader("Content-Type", "multiplart/form-data");
        xhr.setRequestHeader("X-File-Name", file.name);
        xhr.setRequestHeader("X-File-Size", file.size);
        xhr.setRequestHeader("X-File-Type", file.type);

        xhr.send(file);

    }

    hideElement = (function (id) {
        $(id).fadeOut(300, function () {
            $(this).remove();
        });
    });

    deleteFolder = (function (name) {
        var idstr = 'folder_' + name;
        if (confirm('Are you sure you want to remove ' + name + '?')) {
            $.post('/Admin/FileManager/DeleteContainer', { 'name': name }, function (resp) {
                if (resp == null || resp.length == 0) {
                    $($('#' + idstr)).fadeOut(200, function () {
                        $(this).remove();
                    });
                } else {
                    displayError(resp);
                }
            });
        }
    });

    deleteBlob = (function (uri, listing) {
        if (uri.length > 0 && confirm('Are you sure you want to remove the file at: \r\n' + uri)) {
            $.post('/Admin/FileManager/DeleteBlob', { 'uri': uri }, function (resp) {
                if (resp == null || resp.length == 0) {
                    $(listing).fadeOut(200, function () {
                        $(this).remove();
                    });
                } else {
                    showMessage(resp);
                }
            });
        }
    });

    $(document).on('click', '.createFolder', function () {
        var parent = $(this).data('uri');
        var qstring = $(this).data('qstring');
        createFolder(parent, qstring);
    });

    $(document).on('click', '.deleteContainer', function () {
        var name;
        name = $(this).data('name');
        console.log(name);
        deleteFolder(name);
        return false;
    });

    $(document).on('click', 'a.deleteBlob', function (e) {
        e.preventDefault();
        var uri, listing, fid;
        uri = $(this).data('uri');
        listing = $(this).parent().parent().parent();
        deleteBlob(uri, listing);
        return false;
    });

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

    $("#galleries li .contextmenu").live("contextmenu", function (event) {
        event.preventDefault();
        $("#galleries li ul.menu").hide();
        var posx = event.pageX - $(this).parent().parent().offset().left;
        var posy = event.pageY - $(this).parent().parent().offset().top;
        var menu = $(this).parent().parent().find('ul.menu');
        if (menu == null) $(this).parent().parent().find('ul.menu');
        menu.css('top', posy + 'px');
        menu.css('left', posx + 'px');
        menu.show();
    });

    $("html").click(function () { $("#galleries li ul.menu").hide(); });

    $('#loader').hide();

    /*// Stop our page from loading dropped files
    window.addEventListener("dragover", function (e) {
    e = e || event;
    e.preventDefault();
    }, false);
    window.addEventListener("drop", function (e) {
    e = e || event;
    e.preventDefault();
    }, false);*/
});