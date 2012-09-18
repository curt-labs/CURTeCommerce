var typeTable, inquiryTable, addType;
$(function () {

    addType = (function (label, email) {
        $.getJSON('/Admin/ContactManager/AddType', { 'label': label, 'email': email }, function (resp) {
            typeTable.fnAddData([
                resp.label,
                resp.email,
                '<a href="/Admin/ContactManager/DeleteType/' + resp.ID + '" title="Delete ' + resp.label + '" class="deleteType">Remove</a>'
            ]);
            $.modal.close();
        }).error(function (req, status, error) {
            $.modal.close();
            showMessage(error);
        });
    });

    $(document).on('click', '.addType', function () {
        var html = $('.mustache-add_type').html();
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
                $.modal.close();
            }
        });
    });

    $(document).on('click', '#btnAddType', function () {
        var label, email;
        label = $('div.simplemodal-container #label').val();
        email = $('div.simplemodal-container #email').val();
        addType(label, email);
    });

    $(document).on('click', '.deleteType', function (e) {
        e.preventDefault();
        var row, path;
        if (confirm('Are you sure you want to remove this contact type?')) {
            path = $(this).attr('href');
            row = $(this).parent().parent().get()[0];
            $.post(path, function (resp) {
                typeTable.fnDeleteRow(row);
            }).error(function (req, status, error) {
                showMessage('There was an error while processing your request: ' + error);
            });
        }
    });

    $(document).on('click', '.viewMessage', function () {
        var id, view;
        id = $(this).data('id');
        $.getJSON('/Admin/ContactManager/Get/' + id, function (inq) {
            view = {
                name: inq.name,
                phone: inq.phone,
                email: inq.email,
                dateAdded: inq.dateAdded,
                message: inq.message,
                ContactType: inq.type
            };
            if (inq.ContactType !== undefined && inq.ContactType.label !== undefined) {
                view.ContactType = inq.ContactType.label;
            }
            $.modal(Mustache.to_html($('.mustache-view_message').html(), view), {
                containerCss: {
                    backgroundColor: '#ffffff',
                    borderColor: '#ffffff',
                    height: '500px',
                    width: '600px',
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
        }).error(function (req, status, error) {
            showMessage(error);
        });
    });

    $(document).on('click', '.modalClose', function () {
        $.modal.close();
        return false;
    });

    $(document).on('click', '.removeMessage', function () {
        var id, row;
        id = $(this).data('id');
        row = $(this).parent().parent().get()[0];
        if (confirm('Are you sure you want to remove this inquiry?')) {
            $.get('/Admin/ContactManager/RemoveInquiry/' + id, function (resp) {
                if (resp.length == 0) {
                    inquiryTable.fnDeleteRow(row);
                } else {
                    showMessage('There was an error while processing your request: ' + resp);
                }
            }).error(function (req, status, error) {
                showMessage('There was an error while processing your request: ' + error);
            });
        }
    });

    $(document).on('click', '.responded', function () {
        var id, element;
        id = $(this).data('id');
        element = $(this);
        if (id > 0) {
            $.get('/Admin/ContactManager/MarkResponded/' + id, function (resp) {
                if (resp.length === 0) {
                    $(element).parent('td').prev().find('img').attr('src', '/Admin/Content/img/Checkmark.svg');
                    $(element).prev('span').remove();
                    $(element).remove();
                } else {
                    showMessage('There was an error while processing your request: ' + resp);
                }
            }).error(function (req, status, error) {
                showMessage('There was an error while processing your request: ' + error);
            });
        } else {
            showMessage('There was an error while processing your request.');
        }
    });

    typeTable = $('.typeTable').dataTable({ 'bJQueryUI': true });
    inquiryTable = $('.inquiryTable').dataTable({ 'bJQueryUI': true });
});