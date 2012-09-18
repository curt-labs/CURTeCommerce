var createGroup, deleteGroup, addSetting, deleteSetting, settinghtml, generateSetting, removeMenu;
settinghtml = '<div class="admin_form"><input type="hidden" id="group_id" value="{{id}}" /><label for="settingName">Name<input type="text" id="settingName" placeholder="Enter the setting name" pattern="[a-zA-Z0-9-]+"/></label><input type="submit" id="submitText" value="Add Text Field" /><input type="button" id="submitImage" value="Add Image Field" /><a href="javascript:$.modal.close()" class="modalClose" title="Close">Close</a></div>';

$(function () {
    //127.0.0.1:81/Admin/FileManager/Folder?name=misc
    createGroup = (function () {
        group = prompt('Please enter the name of the setting group.');
        if (group !== undefined && group !== null && group.length > 0) {
            $.post('/Admin/Settings/CreateGroup', { 'name': trim(group) }, function (resp) {
                if (resp.name !== null) {
                    var tab = '<li><a href="#' + resp.settingGroupID + '" id="tab-' + resp.settingGroupID + '" data-id="' + resp.settingGroupID + '" title="' + resp.name + '">' + resp.name + '</a></li>';
                    var cont = '<div class="tab_content" id="' + resp.settingGroupID + '"><a class="addSetting" data-group="' + resp.settingGroupID + '" href="javascript:void(0);"><span>+</span> Add Setting</a></div>';
                    if ($('ul.tabs li').length == 0) {
                        $('#nogroups').remove();
                    }
                    $('ul.tabs').append(tab);
                    $('div.tab_container').append(cont);
                    $('#tab-' + resp.name.toLowerCase().replace(/ /, '')).trigger("click");
                } else {
                    showMessage(resp);
                }
            }, 'json');
        }
    });

    deleteGroup = (function (name, id) {
        if (confirm('Are you sure you want to remove the setting group ' + name + '?')) {
            $.post('/Admin/Settings/DeleteGroup', { 'id': id }, function (resp) {
                if (resp == null || resp.length == 0) {
                    $($('#tab-' + id)).fadeOut(200, function () {
                        $(this).remove();
                    });
                    $($('#' + id)).fadeOut(200, function () {
                        $(this).remove();
                    });
                } else {
                    showMessage(resp);
                }
            });
        }
    });

    addSetting = (function (groupid) {
        view = { id: groupid };
        $.modal(Mustache.to_html(settinghtml, view), {
            containerCss: {
                backgroundColor: '#ffffff',
                borderColor: '#ffffff',
                height: '140px',
                width: '550px',
                padding: '10px'
            }, overlayCss: {
                backgroundColor: 'black'
            }, onClose: function (dialog) {
                $('div.simplemodal-container').fadeOut(400, function () {
                    $('div.simplemodal-overlay').hide();
                    $('div.simplemodal-container').hide();
                });
                $.modal.close();
            }
        });
    });

    generateSetting = (function (sname, gid, isImg) {
        $.post('/Admin/Settings/AddSetting', { 'sname': sname, 'gid': gid, 'isImage': isImg }, function (resp) {
            $.modal.close();
            console.log(resp);
            if (resp.name !== null) {
                var setting = '<label for="' + resp.name + '"><span>' + resp.name + '</span><input type="text" name="' + resp.name + '" id="' + resp.name + '" ' + ((isImg) ? 'class="imagefield" ' : "") + 'value="" /></label>';
                $('#' + gid).find('.addSetting').before(setting);
                imageSelector();
            } else {
                showMessage(resp);
            }
        }, 'json');
    });

    deleteSetting = (function (id) {
        if (confirm('Are you sure you want to remove the setting ' + id + '?')) {
            $.post('/Admin/Settings/DeleteSetting', { 'name': id }, function (resp) {
                if (resp == null || resp.length == 0) {
                    $($('#' + id)).parent().fadeOut(200, function () {
                        $(this).remove();
                    });
                } else {
                    showMessage(resp);
                }
            });
        }
    });

    removeMenu = (function () {
        $("ul.tabs li ul.menu").remove();
        $(".tab_container .tab_content label span ul.menu").remove();
    });

    $(document).on('contextmenu', 'ul.tabs li a', function (e) {
        removeMenu();
        var tabid = $(this).data('id');
        if ($('#' + tabid + ' label').length == 0) {
            e.preventDefault();
            var posx = e.pageX - $(this).parent().offset().left;
            var posy = e.pageY - $(this).parent().offset().top;
            var menu = $(this).parent().find('ul.menu');
            if (menu.length > 0) {
                $(menu).css({ 'top': posy, 'left': posx });
            } else {
                $(this).parent().append('<ul class="menu" style="top:' + posy + 'px;left: ' + posx + 'px"><li><a href="javascript:void(0);" data-id="' + tabid + '" class="deleteGroup">Delete</a></li></ul>');
                $(this).parent().find('ul.menu').show();
            }
        }
    });

    $("html").click(function () {
        removeMenu();
    });

    $(document).on('contextmenu', '.tab_container .tab_content label span', function (e) {
        removeMenu();
        var setid = $(this).parent().find('input').attr('id');
        e.preventDefault();
        var posx = e.pageX - $(this).parent().offset().left;
        var posy = e.pageY - $(this).parent().offset().top;
        var menu = $(this).find('ul.menu');
        if (menu.length > 0) {
            $(menu).css({ 'top': posy, 'left': posx });
        } else {
            $(this).append('<ul class="menu" style="top:' + posy + 'px;left: ' + posx + 'px"><li><a href="javascript:void(0);" data-id="' + setid + '" class="deleteSetting">Delete</a></li></ul>');
            $(this).find('ul.menu').show();
        }
    });

    $(document).on('click', '.deleteGroup', function () {
        var id = $(this).data('id');
        var name = $('#tab-' + id).html();
        deleteGroup(name, id);
    });

    $(document).on('click', '.deleteSetting', function () {
        var id = $(this).data('id');
        deleteSetting(id);
    });

    $(document).on('click', '#AddGroup', function () {
        createGroup();
    });

    $(document).on('click', '.addSetting', function (event) {
        event.preventDefault();
        var groupid = $(this).data('group');
        addSetting(groupid);
    });

    $(document).on('click', '#submitText', function (event) {
        event.preventDefault();
        var sname = $('#settingName').val();
        var gid = $('#group_id').val();
        if (trim(sname) != "" && sname.indexOf(' ') == -1 && sname.indexOf('/') == -1) {
            generateSetting(sname, gid, false);
        } else {
            showMessage("No Spaces or Slashes are allowed in the setting name.")
        }
    });

    $(document).on('click', '#submitImage', function (event) {
        event.preventDefault();
        var sname = $('#settingName').val();
        var gid = $('#group_id').val();
        if (trim(sname) != "" && sname.indexOf(' ') == -1 && sname.indexOf('/') == -1) {
            generateSetting(sname, gid, true);
        } else {
            showMessage("No Spaces or Slashes are allowed in the setting name.")
        }
    });

    $(document).on('keypress', '#settingName', function (e) {
        if (!/[0-9a-zA-Z-]/.test(String.fromCharCode(e.which)))
            return false;
    });

});