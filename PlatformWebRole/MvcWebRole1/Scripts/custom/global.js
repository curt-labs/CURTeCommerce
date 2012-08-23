/*jshint nomen: true */
var master_mount, master_year, master_make, master_model, master_style = 0;
var year, make, model, style = 0;
var animate_logo = true;
var urlAddress = "https://www.hitchdepotusa.com/";
var pageName = "Trailer Hitches";
var modelClosingAction = null;
var cartRendering, cartHovered = false;
var cartItems = [];
var validateForm, closeError, resetLookup, displayError, displayMessage, addFavorite, showThanks, changeTab, populateCart, showCart, updateCart, hideCart, cartTimer, startCartTimer, prepareCart, toTitleCase, showLoader, CartItem, getTimestamp;

$(function () {
    // Adds hovering cart
    $('body').append('<div id="cartbox"></div>');
    $('#cartbox').css({ "left": $('#cartlink').offset().left + "px", "top": $('#cartlink').parent().parent().position().top + $('#cartlink').outerHeight() + 'px' });

    CartItem = (function (partID, qty, price, shortDesc, loaded) {
        this.partID = partID;
        this.shortDesc = shortDesc;
        this.qty = qty;
        this.price = price;
        this.loaded = loaded;
    });

    validateForm = (function (form) {
        var errCount = 0;
        $.each($(form).find('input[required],textarea[required],select[required]'), function () {
            var id = $(this).attr('id');
            if ($(this).val() == 0 || $(this).val().length == 0) {
                $(this).addClass('err');
                if ($(this).closest('label').find('span.required-helper').length === 0) {
                    $(this).before('<span class="required-helper">' + $(this).attr('title') + '</span>');
                }
                //formErrorObject[id] = $(this).attr('title');
                errCount++;
            } else {
                $(this).removeClass('err');
                if (this.nodeName.toLowerCase() === 'select') {
                    $(this).parent('.shadow').removeClass('err-shadow');
                }
                $(this).closest('label').find('span.required-helper').remove();
            }
        });
        if (errCount > 0) {
            $.modal.close();
            return false;
        }
        return true;
    });

    closeError = (function () {
        $('.error_display').hide();
    });

    resetLookup = (function () {
        $('#master_lookup label[for=year]').hide();
        $('#master_lookup label[for=make]').hide();
        $('#master_lookup label[for=model]').hide();
        $('#master_lookup label[for=style]').hide();
        $('#master_lookup label[for=mount]').show();

        $('#master_lookup #mount').val(0).removeAttr('disabled');
        $('#master_lookup #year').html('<option value="0">- Select Year -</option>').attr('disabled', 'disabled');
        $('#master_lookup #make').html('<option value="0">- Select Make -</option>').attr('disabled', 'disabled');
        $('#master_lookup #model').html('<option value="0">- Select Model -</option>').attr('disabled', 'disabled');
        $('#master_lookup #style').html('<option value="0">- Select Style -</option>').attr('disabled', 'disabled');
        $('#master_lookup label span').hide();
        $('#master_lookup label select').removeClass('err');
    });

    displayError = (function (err) {
        $('.error_display').find('.err').html(err);
        $('.error_display').fadeIn();
        setTimeout(closeError(), 5000);
    });

    displayMessage = (function (msg) {
        displayError(msg);
    });

    addFavorite = (function () {
        // Add Bookmark Us functionality
        if (window.sidebar) {
            // Mozilla Firefox Bookmark
            window.sidebar.addPanel(pageName, urlAddress, "");
        } else if (window.external && $.browser.webkit != true) {
            // IE Favorite
            window.external.AddFavorite(urlAddress, pageName);
        } else if (window.opera && window.print) {
            // Opera Hotlist
            var obj = document.createElement('a');
            $(obj).attr('href', urlAddress);
            $(obj).setAttribute('title', pageName);
            $(obj).setAttribute('rel', 'sidebar');
            $(obj).click();
            return false;
        } else {
            displayMessage('Don\'t forget to bookmark us! (Ctrl+D)');
        }
    });

    showThanks = (function () {
        $.modal('<span class="modalThanks">Thank you for sharing!</span>', {
            containerCss: {
                backgroundColor: '#ffffff',
                borderColor: '#ffffff',
                height: '100px',
                width: '400px',
                padding: '10px'
            }
        });
        setTimeout('$.modal.close()', 5000);
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
                    //$(element).parent().css('height', $(element).height() + 20 + 'px');
                    $(element).fadeIn(400, function () {
                        $(element).addClass('active');
                    });
                });
            } else {
                $('div.tab_container div.tab_content:first').show();
                $('ul.tabs').not('ul.nojs').find('li:first a').addClass('active');
                if ($('div.part_container').get() !== undefined && $('div.part_container').get().length > 0) {
                    $('div.part_tab_container li:first a').addClass('active');
                    $('div.part_container div.part_content:first').show();
                }
            }
        } else {
            href = $('ul.tabs li a:first').attr('href');
            if (href !== undefined && href.indexOf('#') > -1) {
                changeTab(href.replace('#', ''));
            } else {
                $('div.tab_container div.tab_content:first').show();
            }
        }
    });

    prepareCart = (function () {
        if (!cartRendering) {
            $('#cartbox').empty();
            $('#cartbox').append('<img id="cartloader" src="/Content/img/ajax-loader.gif" alt="loading..." />');
        }
    });

    // Makes AJAX call to get items in the cart and repopulate cart div
    populateCart = (function () {
        $.getJSON('/Cart/getCart', { 'ts': getTimestamp() }, function (data) {
            cartItems = [];
            $(data.CartItems).each(function (i, part) {
                var price = (part.quantity * Number(part.price));
                cartItems.push(new CartItem(part.partID, part.quantity, price, part.shortDesc, true));
            });
            updateCart();
        });
    });

    // Displays the cart div
    showCart = (function () {
        $('#cartbox').css({ "left": $('#cartlink').offset().left + "px", "top": $('#cartlink').parent().parent().position().top + $('#cartlink').outerHeight() + 'px' });
        clearTimeout(cartTimer);
        $('#cartbox').stop(true).fadeTo('fast', 1);
    });

    updateCart = (function () {
        $('#cartbox').empty();
        if (cartItems.length > 0) {
            // cart has items
            var cartstr = "<table><thead><tr><th>Part</th><th>Qty</th><th>Price</th></tr></thead><tbody>";
            var total = 0;
            var count = 0;
            $(cartItems).each(function (i, part) {
                total += part.price;
                count += part.qty;
                cartstr += '<tr><td class="' + ((part.loaded) ? 'loaded' : 'notloaded') + '"><a href="/Part/' + part.partID + '">' + part.shortDesc + '</a></td><td class="qty">' + part.qty + '</td><td class="price">$' + part.price.toFixed(2) + '</td>';
                if (part.loaded) {
                    cartstr += '<td><a href="/Cart/Remove/' + part.partID + '" data-id="' + part.partID + '" class="remove">&times;</a></td></tr>';
                } else {
                    cartstr += '<td><img src="/Content/img/ajax-loader-small.gif" alt="adding to cart..." /></td></tr>';
                }
            });
            cartstr += '</tbody><tfoot><td colspan="2">Subtotal:</td><td class="price">$' + total.toFixed(2) + '</td><td></td></tfoot></table>';
            cartstr += '<a href="/Cart/Checkout" class="button">Checkout</a>';
            $('#cartCount').html(count);
            $('#cartbox').append(cartstr);
        } else {
            // cart is empty
            $('#cartCount').html(0);
            $('#cartbox').append('<p>There are no items in your cart. You should add something.</p>');
        }
    });

    // Begins the timer for hiding the cart
    startCartTimer = (function (duration) {
        if (duration == undefined) {
            duration = 3000;
        }
        clearTimeout(cartTimer);
        cartTimer = setTimeout(hideCart, duration);
    });

    // Hides the cart div
    hideCart = (function () {
        if (!cartHovered) {
            // If you're hovering over the cart elements, it won't hide the cart
            $('#cartbox').fadeOut(1500);
        }
    });

    toTitleCase = (function (str) {
        return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
    });

    showLoader = function (msg) {
        var html = '<div style="width:100%;text-align:center;margin-top:100px;">';
        html += '<img src="/Content/img/large-loader.gif" alt="Loading..." style="width:140px" />';
        html += '<span style="font-size: 24px;display:block;margin-top:25px">' + msg + '</span>';
        html += '</div>';
        $.modal(html, {
            containerId: 'simplemodal-loader',
            containerCss: {
                backgroundColor: '#ffffff',
                height: '400px',
                width: '800px',
                padding: '10px'
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
    };

    if ($.browser.msie) {
        if ($.browser.version < 9 || document.documentMode < 9) {

            $('#master_lookup #style').focus(function () {
                $(this).data('origWidth', $(this).width()).css('width', 'auto');
            }).blur(function () {
                $(this).css('width', $(this).data('origWidth'));
            });
        }
        if ($.browser.version < 8 || document.documentMode < 8) {

            $('header').css('height', '130px').css('margin-bottom', '0px').css('margin-top', '90px');
            $('header').find('.sub_bar').css('z-index', '15000').find('.logo').show();
            $('#top_bar').css('position', 'absolute');
            $('#scroll_logo').css('visibility', 'hidden');
            $('#cartbox').css('position', 'absolute');
            animate_logo = false;

            var el;
            $("select").each(function () {
                el = $(this);
                el.data("origWidth", el.outerWidth()) // IE 8 can haz padding
            }).mouseenter(function () {
                $(this).css("width", "auto");
                $(this).parent('label').css('overflow', 'hidden');
            }).bind("blur change", function () {
                el = $(this);
                el.css("width", el.data("origWidth"));
            });
        }
    }

    if (animate_logo) {
        $('header').css('height', '130px').css('margin-bottom', '0px').css('margin-top', '90px');
        $('header').find('.logo').show();
        if ($(window).width() < 1020) {
            $('#top_bar').css('position', 'absolute');
        } else {
            $('#top_bar').css('position', 'fixed');
        }

        $('#scroll_logo').css('visibility', 'hidden');
        $(window).scroll(function () {
            var top = $(this).scrollTop();
            if (top > 120) {
                $('#scroll_logo').css('visibility', 'visible');
                $('header').find('.logo').hide();
            } else {
                $('#scroll_logo').css('visibility', 'hidden');
                $('header').find('.logo').show();
            }
        });

        $(window).resize(function () {
            if ($(window).width() < 1020) {
                $('#top_bar').css('position', 'absolute');
            } else {
                $('#top_bar').css('position', 'fixed');
            }
        });
    }

    // check placeholder browser support
    if (!Modernizr.input.placeholder) {

        // set placeholder values
        $(this).find('[placeholder]').each(function () {
            if ($(this).val() == '') // if field is empty
            {
                $(this).val($(this).attr('placeholder'));
            }
        });

        // focus and blur of placeholders
        $('[placeholder]').focus(function () {
            if ($(this).val() == $(this).attr('placeholder')) {
                $(this).val('');
                $(this).removeClass('placeholder');
            }
        }).blur(function () {
            if ($(this).val() == '' || $(this).val() == $(this).attr('placeholder')) {
                $(this).val($(this).attr('placeholder'));
                $(this).addClass('placeholder');
            }
        });

        // remove placeholders on submit
        $('[placeholder]').closest('form').submit(function () {
            $(this).find('[placeholder]').each(function () {
                if ($(this).val() == $(this).attr('placeholder')) {
                    $(this).val('');
                }
            })
        });

    }

    $('#master_lookup #reset_lookup').click(function () { resetLookup(); });
    $('#master_lookup select').attr('disabled', 'disabled');
    $('#master_lookup select:first').removeAttr('disabled');

    $('#master_lookup #mount').change(function () {
        $('#master_lookup label img').css('display', 'none');
        //$('#master_lookup #mount').next().css('display', 'inline');
        master_year = master_make = master_model = null;
        $('#master_lookup #year, #master_lookup #make', '#master_lookup #model', '#master_lookup #style').attr('disabled', 'disabled');
        $('#master_lookup #year').html('<option value="0">- Select Year -</option>');
        $('#master_lookup #make').html('<option value="0">- Select Make -</option>');
        $('#master_lookup #model').html('<option value="0">- Select Model -</option>');
        $('#master_lookup #style').html('<option value="0">- Select Style -</option>');
        master_mount = $(this).val();
        if (master_mount !== 0 && master_mount !== null) {
            $('#master_lookup #mount').removeClass('err');
        }
        $.getJSON('https://api.curtmfg.com/v2/getyear?callback=?', { 'mount': master_mount, 'dataType': 'JSONP' }, function (years) {
            $.each(years, function (i, year) {
                var opt = document.createElement('option');
                $(opt).text(year);
                $('#master_lookup #year').append(opt);
            });
            $('#master_lookup label[for=mount]').hide();
            $('#master_lookup label[for=year]').show();
            $('#master_lookup #year').removeAttr('disabled').focus();
            $('#master_lookup label img').css('display', 'none');
        });
    });

    $('#master_lookup #year').change(function () {
        $('#master_lookup label img').css('display', 'none');
        //$('#master_lookup #make').next().css('display', 'inline');
        master_make = master_model = null;
        $('#master_lookup #make', '#master_lookup #model', '#master_lookup #style').attr('disabled', 'disabled');
        $('#master_lookup #make').html('<option value="0">- Select Make -</option>');
        $('#master_lookup #model').html('<option value="0">- Select Model -</option>');
        $('#master_lookup #style').html('<option value="0">- Select Style -</option>');
        master_year = $(this).val();
        if (master_year !== 0 && master_year !== null) {
            $('#master_lookup #year').removeClass('err').next().next().hide();
        }
        $.getJSON('https://api.curtmfg.com/v2/getmake?callback=?', { 'mount': master_mount, 'year': master_year, 'dataType': 'JSONP' }, function (makes) {
            $.each(makes, function (i, make) {
                var opt = document.createElement('option');
                $(opt).text(make);
                $('#master_lookup #make').append(opt);
            });
            $('#master_lookup label[for=year]').hide();
            $('#master_lookup label[for=make]').show();
            $('#master_lookup #make').removeAttr('disabled').focus();
            $('#master_lookup label img').css('display', 'none');
        });
    });

    $('#master_lookup #make').change(function () {
        $('#master_lookup label img').css('display', 'none');
        //$('#master_lookup #model').next().css('display', 'inline');
        master_model = null;
        $('#master_lookup #model', '#master_lookup #style').attr('disabled', 'disabled');
        $('#master_lookup #model').html('<option value="0">- Select Model -</option>');
        $('#master_lookup #style').html('<option value="0">- Select Style -</option>');
        master_make = $(this).val();
        if (master_make !== 0 && master_make !== null) {
            $('#master_lookup #make').removeClass('err').next().next().hide();
        }
        $.getJSON('https://api.curtmfg.com/v2/getmodel?callback=?', { 'mount': master_mount, 'year': master_year, 'make': master_make, 'dataType': 'JSONP' }, function (models) {
            $.each(models, function (i, model) {
                var opt = document.createElement('option');
                $(opt).text(model);
                $('#master_lookup #model').append(opt);
            });
            $('#master_lookup label[for=make]').hide();
            $('#master_lookup label[for=model]').show();
            $('#master_lookup #model').removeAttr('disabled').focus();
            $('#master_lookup label img').css('display', 'none');
        });
    });

    $('#master_lookup #model').change(function () {
        $('#master_lookup label img').css('display', 'none');
        //$('#master_lookup #style').next().css('display', 'inline');
        $('#master_lookup #style').attr('disabled', 'disabled');
        $('#master_lookup #style').html('<option value="0">- Select Style -</option>');
        master_model = $(this).val();
        if (master_model !== 0 && master_model !== null) {
            $('#master_lookup #model').removeClass('err').next().next().hide();
        }
        $.getJSON('https://api.curtmfg.com/v2/getstyle?callback=?', { 'mount': master_mount, 'year': master_year, 'make': master_make, 'model': master_model, 'dataType': 'JSONP' }, function (styles) {
            $.each(styles, function (i, style) {
                var opt = document.createElement('option');
                $(opt).text(style);
                $('#master_lookup #style').append(opt);
            });
            $('#master_lookup label[for=model]').hide();
            $('#master_lookup label[for=style]').show();
            $('#master_lookup #style').removeAttr('disabled').focus();
            $('#master_lookup label img').css('display', 'none');
        });
    });

    $('#master_lookup #style').change(function () {
        $('#master_lookup input[type=submit]').removeAttr('disabled').focus();
        master_style = $(this).val();
        if (master_style !== 0 && master_style !== null) {
            $('#master_lookup #style').removeClass('err').next().next().hide();
        }
    });

    $('#master_lookup input[type=submit]').click(function (e) {
        e.preventDefault();
        master_style = $('#master_lookup #style').val();
        var passed = true;

        // Validate year
        if (master_year === 0 || master_year === null) {
            passed = false;
            $('#master_lookup #year').addClass('err').next().next().css('display', 'block');
        } else {
            $('#master_lookup #year').removeClass('err').next().next().hide();
        }

        // Validate make
        if (master_make === 0 || master_make === null) {
            passed = false;
            $('#master_lookup #make').addClass('err').next().next().css('display', 'block');
        } else {
            $('#master_lookup #make').removeClass('err').next().next().hide();
        }

        // Validate model
        if (master_model === 0 || master_model === null) {
            passed = false;
            $('#master_lookup #model').addClass('err').next().next().css('display', 'block');
        } else {
            $('#master_lookup #model').removeClass('err').next().hide();
        }

        // Validate style
        if (master_style === 0 || master_style === null) {
            passed = false;
            $('#master_lookup #style').addClass('err').next().next().css('display', 'block');
        } else {
            $('#master_lookup #style').removeClass('err').next().hide();
        }

        // Redirect the page to the results
        if (passed) {
            window.location.href = window.location.protocol + '//' + window.location.host + '/Lookup/' + encodeURIComponent(master_year) + '/' + encodeURIComponent(master_make) + '/' + encodeURIComponent(master_model) + '/' + encodeURIComponent(master_style.replace(/\//g, '!'));
        } else {
            displayError('There was an error while processing your request');
        }
    });

    if ($('#master_lookup #year').val() > 0) {
        $('#master_lookup #year').change();
    }

    $('.error_display .close').live('click', function () {
        closeError();
    });

    // Google +1 Button Functionality
    var po, s;
    po = document.createElement('script');
    po.type = 'text/javascript';
    po.async = true;
    po.src = 'https://apis.google.com/js/plusone.js';
    s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(po, s);


    // Add Bookmark Us functionality
    if (window.sidebar || (window.external && $.browser.webkit != true) || (window.opera && window.print)) {
        var bkhtml = '<a href="javascript:void(0)" title="Bookmark Us" class="bookmark">Bookmark Us</a>';
        $('div.sub_menu_container span.phone').before(bkhtml);
        $('.bookmark').live('click', function () {
            addFavorite();
        });
    }

    $.extend($.modal.defaults, {
        closeClass: "modalClose",
        closeHTML: "<a href='javascript:void(0)'>Close<span>x</span></a>",
        opacity: 80,
        onOpen: function (dialog) {
            dialog.overlay.fadeIn('400', function () {
                dialog.data.hide();
                dialog.container.fadeIn('400', function () {
                    dialog.data.slideDown('fast');
                    modelClosingAction = null;
                });
            });
        },
        onClose: function (dialog) {
            dialog.data.fadeOut('400', function () {
                dialog.container.hide('400', function () {
                    dialog.overlay.slideUp('fast', function () {
                        $.modal.close();
                    });
                });
            });
        },
        overlayClose: true
    });

    // Handle Share via Email
    $('a.email-share').click(function (e) {
        e.preventDefault(); // Stop link from firing
        var el = $(this);
        $.get($(el).attr('href') + '&layout=false', function (html) {
            if ($(el).attr('href').indexOf('part') !== -1) {
                $.modal(html, {
                    containerCss: {
                        backgroundColor: '#ffffff',
                        borderColor: '#ffffff',
                        height: '700px',
                        width: '800px',
                        padding: '10px'
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
            } else {
                $.modal(html, {
                    containerCss: {
                        backgroundColor: '#ffffff',
                        borderColor: '#ffffff',
                        height: '700px',
                        width: '500px',
                        padding: '10px'
                    }, onClose: function (dialog) {
                        if (modelClosingAction === 'showThanks') {
                            $('#simplemodal-overlay').remove();
                            $('#simplemodal-container').remove();
                            showThanks();
                        } else {
                            dialog.data.fadeOut('400', function () {
                                dialog.container.hide('400', function () {
                                    dialog.overlay.slideUp('fase', function () {
                                        $.modal.close();
                                    });
                                });
                            });
                        }
                    }
                });
            }
            $('form.share-form input[type=submit]').css('display', 'inline-block').after('<a href="javascript:$.modal.close()" title="Close" style="margin-left:10px;vertical-align:bottom;">Close</a>');

            $('form.share-form input[type=submit]').live('click', function (e) {
                $('#simplemodal-container').find('span.error').remove();
                e.preventDefault();
                var action, opts;
                opts = new Object();
                action = $('form.share-form').attr('action');
                opts.recipient = $('#recipient').val();
                opts.sender = $('#sender').val();
                opts.subj = $('#subj').val();
                opts.msg = $('#msg').val();
                opts.ajax = true;

                $.post(action, opts, function (resp) {
                    if (resp.length > 0) {
                        $('span.share-heading').after('<span class="share-heading error" style="color:red;font-size:13px;font-weight:bold">' + resp + '</span>');
                    } else {
                        modelClosingAction = 'showThanks';
                        $.modal.close();
                    }
                });
            });

        });
        return false;
    });


    $(document).on('click', 'form.simple-form input[type=submit]', function (e) {
        return validateForm($(this).closest('.simple-form'));
    });

    $(document).on('change keyup', 'form.simple-form input,form.simple-form textarea,form.simple-form select', function () {
        validateForm($(this).closest('.simple-form'));
    });

    $('ul.tabs').not('ul.nojs').find('li a:first').addClass('active');
    $('div.tab_container div.tab_content').hide();
    $('div.tab_container div.tab_content:first').addClass('active');

    // adds an item to the cart and calls populate method
    $(document).on('click', 'a.add_cart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var shortDesc = $('#' + id).find('h4').html();
        var price = Number($('#' + id).find('span.price').html().replace('$', ''));
        var added = false;
        $(cartItems).each(function (i, obj) {
            if (obj.partID == id) {
                obj.qty += 1;
                obj.price = obj.qty * price;
                added = true;
            }
        });
        if (!added) {
            cartItems.push(new CartItem(id, 1, price, shortDesc, false));
        }
        updateCart()
        showCart();
        startCartTimer();
        $.getJSON('/Cart/AddAjax', { 'id': id, 'qty': 1, 'ts': getTimestamp() }, function () {
            populateCart();
        });
    });

    // removes an item from the cart and fires populate method
    $(document).on('click', '#cartbox table a.remove', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var row = $(this).parent().parent().get(0);
        $(cartItems).each(function (i, obj) {
            if (obj.partID == id) {
                cartItems.splice(i, 1);
            }
        });
        if (cartItems.length > 0) {
            $(row).fadeOut('fast', function () {
                $.getJSON('/Cart/RemoveAjax', { 'id': id, 'ts': getTimestamp() }, populateCart);
            });
        } else {
            $('#cartbox').empty();
            $('#cartbox').append('<p style="display: none;">There are no items in your cart. You should add something.</p>');
            $('#cartbox p').fadeIn('fast', function () {
                $.getJSON('/Cart/RemoveAjax', { 'id': id, 'ts': getTimestamp() }, populateCart);
            });

        }
    });

    // adds an item to the cart and calls populate method
    $('#partAddToCart').submit(function (event) {
        event.preventDefault();
        var id = $(this).data('id');
        var shortDesc = $('#mid_col').find('span.shortDesc').html();
        var qty = Number($('#qty').val());
        var price = $('#mid_col').find('span.price').html().replace('$', '');
        price = Number(price);
        var added = false;
        $(cartItems).each(function (i, obj) {
            if (obj.partID == id) {
                obj.qty += qty;
                obj.price = obj.qty * price;
                added = true;
            }
        });
        if (!added) {
            cartItems.push(new CartItem(id, qty, price, shortDesc, false))
        }
        updateCart()
        showCart();
        startCartTimer();
        $.getJSON('/Cart/AddAjax', { 'id': id, 'qty': $('#qty').val(), 'ts': getTimestamp() }, function () {
            populateCart();
        });
    });

    getTimestamp = (function () {
        var ts = new Date();
        return ts.getDay().toString() + ts.getMonth().toString() + ts.getYear().toString() + ts.getHours().toString() + ts.getMinutes().toString() + ts.getSeconds().toString() + ts.getMilliseconds().toString() + ts.getTimezoneOffset().toString();
    });

    // handles hover states for cart
    $('#cartlink, #cartbox').hover(function () { cartHovered = true; showCart(); }, function () { cartHovered = false; startCartTimer(300); });

    // populates cart on page load
    populateCart();

    /** Modernize the shit out of the site **/
    /*if (Modernizr.boxshadow) {
    $('.simple-form select').wrap('<div class="shadow" />').after('<div class="clearfix" />').parent().width(function () {
    return $(this).outerWidth()
    }).height(function () {
    return $(this).outerHeight()
    });
    }
    */
    /**** 
    *  USE JQUERY-BBQ to handle hashing and back button nav 
    *******/

    // Bind an event to window.onhashchange that, when the history state changes,
    // gets the url from the hash and displays either our cached content or fetches
    // new content to be displayed.
    $(window).bind('hashchange', function (e) {

        // Get the hash (fragment) as a string, with any leading # removed. Note that
        var param = $.param;
        var url = param.fragment();

        // Fire click on the tab that is being referenced
        changeTab(url);

    });

    // Since the event is only triggered when the hash changes, we need to trigger
    // the event now, to handle the hash the page may have loaded with.
    $(window).trigger('hashchange');

    /**** 
    *  END JQUERY-BBQ
    *******/

    /*$('a[rel*=Shadowbox]').live('click', function (e) {
    e.stopPropagation();
    e.preventDefault();
    return false;
    });*/
    //jsErrLog.debugMode = true;

    /**** Preload Large Loader ******/
    var img = new Image();
    img.src = '/Content/img/large-loader.gif';
});
