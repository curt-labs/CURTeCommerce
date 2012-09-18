/* Author: Alex Ninneman @ninnemana */

var formErrorObject, showMessage, hideMessage, validateForm, changeTab, closeError, displayError, loadFiles, unloadImageSelector, imageSelector, checkProtocol;
var hover_messagebox = false;
$(function () {

    validateForm = (function (formobj) {
        formErrorObject = new Object();
        $.each($(formobj).find('input[required],textarea[required],select[required]'), function () {
            var id = $(this).attr('id');
            if ($(this).val() == 0 || $(this).val().length == 0) {
                $(this).addClass('err');
                formErrorObject[id] = $(this).attr('title');
            } else {
                $(this).removeClass('err');
                if (formErrorObject[id] != undefined) {
                    delete formErrorObject[id];
                }
            }
        });
        if (Object.keys(formErrorObject).length > 0) {
            var html = '';
            html += '<ul class="error_box">';
            for (var e in formErrorObject) {
                html += '<li>' + formErrorObject[e] + '</li>';
            }
            html += '</ul>';
            $('.error_box').remove();
            $(formobj).prepend(html);
            return false;
        } else {
            $('.error_box').remove();
        }
        return true;
    });

    showMessage = (function () {
        var msg = arguments[0];
        $('div.message_box span').html(msg);
        $('div.message_box').slideDown();
        setTimeout('hideMessage()', 8000);
    });

    hideMessage = (function () {
        if (!hover_messagebox) {
            $('div.message_box').slideUp('350', function () {
                $('div.message_box span').html('');
            });
        } else {
            setTimeout('hideMessage()', 3000);
        }
    });

    changeTab = (function (href) {
        if (href.length > 0) {
            var element;
            element = $('#' + href).get()[0];
            if (element != undefined && element != null) {
                var oldelement, oldhref;
                oldelement = $('a[href=#' + href + ']').parent().parent().find('a.active');
                oldhref = (oldelement.length > 0) ? $(oldelement).attr('href') : "";
                $(oldelement).removeClass('active')
                $('a[href=#' + href + ']').addClass('active');
                $(element).parent().find('div.tab_content' + ((oldhref != "") ? oldhref : "")).fadeOut(400, function () {
                    $(this).removeClass('active');
                    $(element).parent().css('height', $(element).height() + 20 + 'px');
                    $(element).fadeIn(400, function () {
                        $(element).addClass('active');
                    });
                });
            }
        } else {
            href = $('ul.tabs li a:first').attr('href');
            if (href !== undefined) {
                changeTab(href.replace('#', ''));
            }
        }
    });

    imageSelector = (function () {
        $('.imagefield').each(function () {
            if ($(this).parent().find('.imagefieldvalue').length == 0) {
                $(this).hide();
                var img = $.trim($(this).val());
                if (img == "") {
                    img = "/Admin/Content/img/noimage.jpg";
                }
                var imgdisplay = '<img class="imagefieldvalue" src="' + img + '" alt="Image" />';
                var choosebutton = '<input type="button" class="chooseimagefield" value="Choose" />';
                var clearbutton = '<input type="button" class="clearimagefield" value="Clear" />';
                var radiobuttons = '<input type="radio" class="chooseimageprotocol protocolnone" value="none" /><input type="radio" class="chooseimageprotocol protocolhttps" value="https" /><input type="radio" class="chooseimageprotocol protocolhttp" value="http" />';
                $(this).before(imgdisplay);
                $(this).after(radiobuttons);
                $(this).after(clearbutton);
                $(this).after(choosebutton);
                checkProtocol($(this).parent());
            }
        });
    });

    checkProtocol = function (obj) {
        var img = $(obj).find('.imagefieldvalue').attr('src');
        var protocol = 'protocolnone';
        if (img == '' || img == '/Admin/Content/img/noimage.jpg') {
            protocol = 'protocolhttp';
        } else {
            if (img.indexOf("https://") != -1) {
                protocol = 'protocolhttps';
            }
            if (img.indexOf("http://") != -1) {
                protocol = 'protocolhttp';
            }
        }
        $(obj).find('.chooseimageprotocol').each(function () { $(this).removeAttr('checked'); });
        $(obj).find('input.' + protocol).attr("checked", "checked");
    }

    loadFiles = (function (name) {
        $.getJSON('/Admin/FileManager/AjaxFileBrowser', { 'name': name }, function (response) {
            $("#loader").hide();
            $('#filemanagerfiles').empty();
            if (name != "") {
                var parent = name.split('/')[0];
                if (name.indexOf('/') == -1) {
                    parent = "";
                }
                var folder = '<div class="folder" data-path="' + parent + '"><span class="foldericon">&uarr;</span><span class="foldername">Back</span></div>';
            }
            $('#filemanagerfiles').append(folder);
            $(response.SubContainers).each(function (i, cont) {
                var path = cont.uri.replace('http://', '');
                path = path.substring(path.indexOf('/') + 1);
                if (path.indexOf('/') > -1) {
                    path = path.substring(0, path.length - 1);
                    var fname = path.split("/")[path.split("/").length - 1];
                } else {
                    var fname = path;
                }
                var folder = '<div class="folder" data-path="' + path + '"><span class="foldericon"></span><span class="foldername">' + fname + '</span></div>';
                $('#filemanagerfiles').append(folder);
            });
            var img_types = ["jpg", "png", "jpeg", "bmp", "gif"];
            $(response.files).each(function (i, file) {
                var filename = file.uri.split('/')[file.uri.split('/').length - 1];
                var isimg = false;
                $(img_types).each(function (x, itype) {
                    if (filename.split('.')[filename.split('.').length - 1].toLowerCase() == itype) {
                        isimg = true;
                    }
                });
                var folder = '<div class="file" data-path="' + file.uri + '"><span class="fileicon"><img src="' + ((isimg) ? file.uri : "/Admin/Content/img/FileNew.png") + '" alt="' + filename + '"></span><span class="filename">' + filename + '</span></div>';
                $('#filemanagerfiles').append(folder);
            });
        });
    });

    $(document).on('click', '#filemanagerfiles .folder', function () {
        var path = $(this).data('path');
        loadFiles(path);
    });

    $(document).on('click', '#filemanagerfiles .file', function () {
        var path = $(this).data('path').replace('http:', '').replace('https:', '');
        $('input.modalchoosing').attr('value', path);
        $('input.modalchoosing').parent().find('img.imagefieldvalue').attr('src', path);
        checkProtocol($('input.modalchoosing').parent());
        $.modal.close();
    });

    // Start firing off page load tasks!!

    if (formErrorObject == undefined) {
        formErrorObject = new Object();
    }
    $('header').css('position', 'fixed');
    $('header').css('top', '0');

    $('nav').css('height', $('div[role=main]').find('section').height() + 'px').css('padding-top', '70px');
    $('div[role=main] section').css('padding-top', '70px');

    $(window).resize(function () {
        $('nav').css('height', $('div[role=main]').find('section').height() + 'px')
        if ($(window).width() < 1020) {
            $('header').css('position', 'relative');
            $('nav').css('padding-top', '30px');
            $('div[role=main] section').css('padding-top', '10px');
        } else {
            $('header').css('position', 'fixed');
            $('nav').css('padding-top', '70px');
            $('div[role=main] section').css('padding-top', '70px');
        }
    });

    $('input[type=submit]').live('click', function () {
        var formobj = $(this).closest("form");
        return validateForm(formobj);
    });
    $('input[required]').live('keyup', function () {
        var formobj = $(this).closest("form");
        validateForm(formobj);
    });

    imageSelector();

    $(document).on('click', '.chooseimagefield', function (e) {
        e.preventDefault();
        var originalObj = $(this);
        $(originalObj).parent().find('.imagefield').addClass('modalchoosing');
        var html = '<div id="filemanager"><a href="javascript:$.modal.close()" title="Close" class="modalClose">close</a><img src="/Admin/Content/img/ajax_loader.gif" alt="loading..." id="loader"/><div id="filemanagerfiles"></div></div>'
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
                $.modal.close();
            }
        });
        loadFiles('');
    });

    $(document).on('click', '.clearimagefield', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to clear this image?')) {
            $(this).parent().find('.imagefield').attr('value', '');
            $(this).parent().find('.imagefieldvalue').attr('src', '/Admin/Content/img/noimage.jpg');
        }
    });

    $(document).on('click', '.chooseimageprotocol', function (e) {
        var protocol = $(this).attr('class').replace('chooseimageprotocol', '').replace(' ', '');
        $(this).parent().find('.chooseimageprotocol').each(function () { $(this).removeAttr('checked'); });
        $(this).attr('checked', 'checked');
        var path = $(this).parent().find('input.imagefield').val().replace('http:', '').replace('https:', '');
        switch (protocol) {
            case 'protocolhttps':
                path = 'https:' + path;
                break;
            case 'protocolhttp':
                path = 'http:' + path;
                break;
        }
        $(this).parent().find('input.imagefield').attr('value', path);
        $(this).parent().find('img.imagefieldvalue').attr('src', path);
    });

    $('div.message_box').mouseenter(function () {
        hover_messagebox = true;
    }).mouseleave(function () {
        hover_messagebox = false;
    });

    $('div.message_box a').live('click', function () {
        hover_messagebox = false;
        hideMessage();
    });

    $('nav ul li ul').hide();

    $.each($('nav ul li').get(), function () {
        var link = $(this).find('a');
        if ($(link).attr('class') == 'active') {
            $(this).find('ul').slideDown();
        }
    });

    $('nav ul li').hoverIntent(function () {
        var link = $(this).find('a');
        if ($(link).attr('class') != 'active') {
            $(this).find('ul').slideDown();
        }
    }, function () {
        var link = $(this).find('a');
        if ($(link).attr('class') != 'active') {
            $(this).find('ul').slideUp();
        }
    });

    $('.admin_form ul li a:first').addClass('active');
    $('.admin_form div.tab_container div.tab_content:first').addClass('active');

    /**** 
    *  USE JQUERY-BBQ to handle hashing and back button nav 
    *******/

    // Bind an event to window.onhashchange that, when the history state changes,
    // gets the url from the hash and displays either our cached content or fetches
    // new content to be displayed.
    $(window).bind('hashchange', function (e) {

        // Get the hash (fragment) as a string, with any leading # removed. Note that
        var url = $.param.fragment();

        // Fire click on the tab that is being referenced
        changeTab(url);

    });

    // Since the event is only triggered when the hash changes, we need to trigger
    // the event now, to handle the hash the page may have loaded with.
    $(window).trigger('hashchange');

    /**** 
    *  END JQUERY-BBQ
    *******/

    // Configure parameters for jsErrLog
    //jsErrLog.debugMode = true; // Turn off when we go live

    if ($.browser.msie) {
        window.location.href = 'http://www.google.com/chrome';
    }

});
