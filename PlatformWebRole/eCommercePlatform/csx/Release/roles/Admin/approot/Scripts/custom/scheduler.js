var deleteTask, runTask;

deleteTask = function (name, id, href) {
    if (confirm('Are you sure you want to delete task "' + name + '"?')) {
        $.post('/Admin/Scheduler/DeleteTask', { 'id': id }, function () {
            $('#task_' + id).fadeOut('fast',function () { $('#task_' + id).remove(); });
        });
    }
};

runTask = function (id) {
    $('#task_' + id + ' img.running').show();
    $.post('/Admin/Scheduler/RunTask', { 'id': id }, function () {
        $('#task_' + id + ' img.running').hide();
    });
};

$(function () {
    $('#runtime').timepicker({ ampm: true });
    $(document).on('click', '.delete', function (e) {
        event.preventDefault();
        var id, name, href;
        id = $(this).data('id');
        name = $(this).data('name');
        href = $(this).attr('href');
        deleteTask(name, id, href);
    });

    $(document).on('click', '.runtask', function (e) {
        event.preventDefault();
        var id;
        id = $(this).data('id');
        runTask(id);
    });
});