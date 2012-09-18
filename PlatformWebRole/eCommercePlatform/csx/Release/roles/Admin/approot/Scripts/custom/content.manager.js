var table,tab_height, container_height, loadPageList, pageArray, parentTable, subTable;
$(function () {

    loadPageList = (function (pageType, listType, pageID) {
        $.getJSON('/Admin/ContentManager/GetPages', { 'id': pageID }, function (pages) {
            var tmpl, html, view;
            tmpl = $('div.mustache div.mustache-page_list').html();
            view = {
                pages: pages,
                pageType: pageType,
                listType: listType,
                button: (pageType == 'parent') ? "Make Parents" : "Make Subpages"
            };
            html = Mustache.to_html(tmpl, view);
            $.modal(html, {
                containerCss: {
                    backgroundColor: '#ffffff',
                    borderColor: '#ffffff',
                    height: '500px',
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
    });

    try {
        parentTable = $('#parentTable').dataTable({ 'bJQueryUI': true });
        subTable = $('#subTable').dataTable({ 'bJQueryUI': true });
        if (parentTable.length === 0 && subTable.length === 0) {
            $('table').dataTable({ 'bJQueryUI': true });
        }
        CKEDITOR.replace('content', {
            height: '550px',
            filebrowserBrowseUrl: '/Admin/FileManager/FileBrowser',
            filebrowserUploadUrl: '/Admin/FileManager/CKUpload'
        });
    } catch (err) { }

    $('ul.file_tabs li:first').addClass('active');
    $('div.file_tab_container div.tab_content:first').addClass('active');

    container_height = $('div.file_tab_container').height();
    tab_height = $('ul.file_tabs').height();
    if (tab_height > container_height) {
        $('div.file_tab_container').css('min-height', (tab_height + 50) + 'px');
    }

    $('ul.file_tabs li').click(function () {
        var tab = $(this).find('a').attr('href');
        $('ul.file_tabs li').removeClass('active');
        $(this).addClass('active');
        $('div.tab_content').removeClass('active');
        $(tab).addClass('active');
        $('div.file_tab_container').css('height', ($(tab).height() + 50) + 'px');
    });

    $(document).on('click', '.addParent', function () {
        var count = $(this).data("count");
        console.log(count);
        if (count == 0) {
            loadPageList('parent', 'single', 0);
        } else {
            showMessage("Only 1 parent page is allowed.");
        }
    });

    $(document).on('click', '.addSub', function () {
        loadPageList('sub', 'multiple', 0);
    });

    $(document).on('click', 'div.page_list article', function () {
        if ($(this).find('input[type=checkbox]').is(':checked')) {
            $(this).find('input[type=checkbox]').prop('checked', false);
            $(this).find('span.check').hide();
        } else {
            if ($('#simplemodal-data input.listType').val() == "single") {
                $('#simplemodal-data div.page_list article input.page_select').prop('checked', false);
                $('#simplemodal-data div.page_list article span.check').hide();
            }
            $(this).find('input[type=checkbox]').prop('checked', true);
            $(this).find('span.check').show();
        }
    });

    $(document).on('click', '#parent', function () {
        var pageID = $(this).attr('data-pageID');
        var parentID;
        $.each($('div.simplemodal-container div.page_list .page_select').get(), function (i, page) {
            if ($(page).is(':checked')) {
                parentID = $(page).val();
            }
        });
        $.getJSON('/Admin/ContentManager/AddParent', { 'id': pageID, 'parentID': parentID }, function (response) {
            if (response[0] !== undefined && response[0].Title !== undefined) {
                parentTable.fnClearTable();
                $.each(response, function (i, page) {
                    parentTable.fnAddData([
                        page.Title,
                        Mustache.to_html($('div.mustache-new_parent_col2').html(), page),
                        Mustache.to_html($('div.mustache-new_parent_col3').html(), page)
                    ]);
                });
                $.modal.close();
                $('a.addParent').data("count", 1);
            } else {
                $.modal.close();
                showMessage(response);
            }
        }).error(function (req, status, error) {
            $.modal.close();
            showMessage('There was an error while processing your request: ' + error);
        }); ;
    });

    $(document).on('click', '#sub', function () {
        var pageID = $(this).attr('data-pageID');
        pageArray = new Array();
        $.each($('div.simplemodal-container div.page_list .page_select').get(), function (i, page) {
            if ($(page).is(':checked')) {
                pageArray.push($(page).val());
            }
        });
        $.getJSON('/Admin/ContentManager/AddSubs', { 'id': pageID, 'subs': pageArray.toString() }, function (response) {
            if (response[0] !== undefined && response[0].Title !== undefined) {
                subTable.fnClearTable();
                $.each(response, function (i, page) {
                    subTable.fnAddData([
                        page.Title,
                        Mustache.to_html($('div.mustache-new_parent_col2').html(), page),
                        Mustache.to_html($('div.mustache-new_sub_col3').html(), page)
                    ]);
                });
                $.modal.close();
            } else {
                $.modal.close();
                showMessage(response);
            }
        }).error(function (req, status, error) {
            $.modal.close();
            showMessage(error);
        });
    });

    $(document).on('click', '.removeParent', function (e) {
        e.preventDefault();
        if (confirm('Are you sure want to remove the relationship between the two pages?')) {
            var row, path, tableId;
            row = $(this).parent().parent().get()[0];
            tableId = $(this).parent().parent().parent().parent().attr('id');
            path = $(this).attr('href');
            $.get(path, function (resp) {
                if (resp.length === 0) {
                    if (tableId === 'subTable') {
                        subTable.fnDeleteRow(row);
                    } else {
                        parentTable.fnDeleteRow(row);
                        $('a.addParent').data("count", 0);
                    }
                } else {
                    showMessage(resp);
                }
            }).error(function () {
                showMessage('There was an error while processing your request.');
            });
        }
    });

    $(document).on('click', '.deletePage', function () {
        if (confirm('Are you sure you want to delete this page?\r\nThis cannot be undone')) {
            return true;
        }
        return false;
    });
});