/*jslint browser: true */
var ids = [];
var view_more_trigger = 0;
var page = 1;
var perpage = 20;
var endless_load = true;
var generateHTML;
var getMore;
var loadInstallSheet;
var parts;

$(function () {
    view_more_trigger = $('#part_container').height() - 1000;
    $(window).scroll(function () {
        var current_pos = $(window).scrollTop();
        if (current_pos > view_more_trigger && endless_load) {
            page = page + 1;
            getMore(page, perpage);
        }
    });

    $('.view_more').live('click', function (e) {
        e.preventDefault();
        page = page + 1;
        getMore(page, perpage);
    });

    getMore = function (current_page, per_page) {
        var term, getObjs;
        term = $('#searchTerm').val();
        getObjs = { 'page': current_page, 'perpage': per_page, 'search': term };
        $.get('/Search/GetMore', getObjs, function (data) {
            if (data.length > 0) {
                $.each(data, function (i, part) {
                    if (ids.indexOf(part.partID) === -1) {
                        ids.push(part.partID);
                        var html = generateHTML(part);
                        $('.part_groups').height($('.part_groups').height() + 330);
                        $('.part:last').after(html);
                        view_more_trigger = $('#part_container').height() - 1000;
                    }
                });
            } else {
                endless_load = false;
                $('.view_more').remove();
            }
        }, 'json');
    };

    generateHTML = function (part) {
        var item_count, html;
        item_count = $('.part').get().length;
        html = '';
        html += '<div class="part" id="' + part.partID + '">';
        html += '<h4>' + part.shortDesc + '</h4>';
        html += '<div class="left_col">';
        html += '<p>Part #' + part.partID + '</p>';
        html += '<div class="image_array">';
        html += '<a class="main" href="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_1024x768_a.jpg" rel="Shadowbox[parts' + item_count + ']" target="_blank" title="Click to enlarge">';
        html += '<img src="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_300x225_a.jpg" onerror="$(this).parent().remove()" alt="Click to enlarge" />';
        html += '</a>';
        html += '<a class="sub" href="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_1024x768_b.jpg" rel="Shadowbox[parts' + item_count + ']" title="Click to enlarge">';
        html += '<img src="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_100x75_b.jpg" onerror="$(this).parent().remove()" alt="Click to enlarge" />';
        html += '</a>';
        html += '<a class="sub" href="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_1024x768_c.jpg"  rel="Shadowbox[parts' + item_count + ']" title="Click to enlarge">';
        html += '<img src="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_100x75_c.jpg" onerror="$(this).parent().remove()" alt="Click to enlarge" />';
        html += '</a>';
        html += '<a class="sub" href="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_1024x768_d.jpg"  rel="Shadowbox[parts' + item_count + ']" title="Click to enlarge">';
        html += '<img src="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_100x75_d.jpg" onerror="$(this).parent().remove()" alt="Click to enlarge" />';
        html += '</a>';
        html += '<a class="sub" href="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_1024x768_e.jpg"  rel="Shadowbox[parts' + item_count + ']" title="Click to enlarge">';
        html += '<img src="https://www.curtmfg.com/CURTLibrary/' + part.partID + '/images/' + part.partID + '_100x75_e.jpg" onerror="$(this).parent().remove()" alt="Click to enlarge" />';
        html += '</a>';
        html += '</div>';
        html += '</div>';
        html += '<div class="mid_col">';
        html += '<ul>';
        $.each(part.content, function (i, con) {
            if (con.key.toUpperCase() !== 'INSTALLATIONSHEET' && con.key.toUpperCase() !== 'INSTALLVIDEO') {
                html += '<li>' + con.value + '</li>';
            }
        });
        html += '</ul>';
        html += '</div>';
        html += '<div class="right_col">';
        html += '<span class="price">' + part.listPrice + '</span>';
        html += '<a href="/Part/' + part.partID + '" class="details" title="View Details">View Details</a>';
        html += '<a href="/Cart/Add/' + part.partID + '" class="add_cart" data-id="' + part.partID + '" title="Add to Cart">';
        html += '<img src="/Content/img/cart.png" alt="Add to Cart" />';
        html += '<span>Add to Cart</span>';
        html += '</a>';
        html += loadInstallSheet(part);
        html += '</div>';
        html += '<div class="clearfix"></div>';
        html += '</div></div>';
        return html;
    };

    loadInstallSheet = function (part) {
        var html;
        html = '';
        $.each(part.content, function (i, content) {
            if (content.key.toUpperCase() === 'INSTALLATIONSHEET' && html.length === 0) {
                html += '<a href="' + content.value + '" title="Download Installation Instructions" class="install">';
                html += '<img src="/Content/img/pdf.png" alt="Download Installation Instructions" />Installation Instructions</a>';
            }
        });

        return html;
    };

    // We need to populate our array full of the displayed partID's
    parts = $('.part').get();
    $.each(parts, function (i, part) {
        var id = parseInt($(this).attr('id'));
        ids.push(id);
    });
});