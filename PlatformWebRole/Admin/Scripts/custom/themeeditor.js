var tab, matchIndent, fillGutter, filecontent, saveFile;
$(function () {
    filecontent = $('#preeditor').html();
    //SyntaxHighlighter.all()
    $('.editor pre').on('keydown', function (e) {
        var keyCode = e.keyCode || e.which;
        switch (keyCode) {
            case 9:
                e.preventDefault();
                tab();
                break;
            case 13:
                e.preventDefault();
                matchIndent();
                break;
            case 83:
                if (e.ctrlKey) {
                    e.preventDefault();
                    saveFile(e);
                    break;
                }
        }
        fillGutter()
    });
    $('.editor pre').on('keyup', fillGutter);
    fillGutter();
    $('.save').on('click', saveFile);

    $('.cancel').on('click', function (e) {
        var href = $(this).attr('href');
        if (filecontent != $('#preeditor').html()) {
            e.preventDefault();
            if (confirm('Your changes are not saved. Discard your changes?')) {
                window.location = href;
            }
        }
    });
});

tab = function () {
    var sel = window.getSelection();
    var range = sel.getRangeAt(0);
    var newTextNode = document.createTextNode('    ');
    range.insertNode(newTextNode);
    range.setStartAfter(newTextNode);
    sel.removeAllRanges();
    sel.addRange(range);
}

matchIndent = function () {
    var sel = window.getSelection();
    var range = sel.getRangeAt(0);
    /*var cursorpos = range.startOffset;
    var text = range.startContainer.nodeValue;
    var revpos = text.length - cursorpos;
    var revtext = text.split("").reverse().join("");
    var spacepos = revtext.indexOf("  ", revpos);
    var leadingspaces = 0;
    if (spacepos > -1) {
        var lbpos = revtext.indexOf("\n", spacepos);
        leadingspaces = lbpos - spacepos;
    }
    var extraspace = "";
    for (var i = 0; i < leadingspaces; i++) {
        extraspace += ' ';
    }*/
    var newTextNode = document.createTextNode(String.fromCharCode(10,13));
    range.insertNode(newTextNode);
    range.setStartAfter(newTextNode);
    sel.removeAllRanges();
    sel.addRange(range);
}

fillGutter = function () {
    var divHeight = $('#preeditor').height();
    var lineHeight = Number($('#preeditor').css('line-height').replace('px',''));
    var count = divHeight / lineHeight;
    var spans = $('.gutter span.line').length;
    var loopcount = count;
    if (count > spans) {
        loopcount = count - spans;
    } else if (count == spans) {
        loopcount = 0;
    } else {
        $('.gutter').empty();
    }
    for (var i = 0; i < loopcount; i++) {
        $('.gutter').append('<span class="line"></span>');
    }
};

saveFile = function (e) {
    $('.ajax-loader').show();
    e.preventDefault();
    var fileID = $('.save').data('fileid');
    var themeID = $('.save').data('themeid');
    var areaID = $('.save').data('areaid');
    var typeID = $('.save').data('typeid');
    var name = $('#filename').val();
    var content = $('#preeditor').html();
    if ($.trim(name) == "") {
        showMessage('File Name is required');
        $('#filename').focus();
        return;
    }
    $.post('/Admin/Themes/SaveFile', { fileID: fileID, themeID: themeID, areaID: areaID, typeID: typeID, content: content, name: name }, function (data) {
        $('.ajax-loader').hide();
        filecontent = content;
        if (fileID == "" || fileID == 0) {
            if (data.ID > 0) {
                window.location = "/Admin/Themes/EditFile/" + data.ID;
            } else {
                showMessage('There was a problem saving this file. It may be a duplicate file name. Please try again.');
            }
        } else {
            showMessage('File saved successfully');
        }
    }, 'json');
}