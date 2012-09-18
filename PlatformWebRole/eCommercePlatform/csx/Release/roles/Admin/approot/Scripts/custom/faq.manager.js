var viewQuestions;
$(function () {

    viewQuestions = (function (article) {
        $(article).find('div.mustache').slideDown(400);
    });

    $(document).on('click', '.viewQuestions', function () {
        var article = $(this).closest('article').get()[0];
        viewQuestions(article);
    });

    $(document).on('click', '.deleteTopic, .deleteQuestion', function (e) {
        e.preventDefault();
        var href, confirmMsg, article;
        href = $(this).attr('href');
        article = $(this).closest('article');
        if ($(this).attr('class') === 'deleteQuestion') {
            confirmMsg = 'Are you sure you want to remove this question?';
        } else {
            confirmMsg = 'Are you sure you want to remove this topic?\r\nAll questions will no longer be tied to a topic';
        }
        if (confirm(confirmMsg)) {
            $.get(href, function (resp) {
                if (resp.length === 0) {
                    $(article).remove();
                } else {
                    showMessage(resp);
                }
            }).error(function (req, status, error) {
                showMessage(error);
            });
        }
    });

});