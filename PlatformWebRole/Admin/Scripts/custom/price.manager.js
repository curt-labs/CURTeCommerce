var priceTable, setList, setPrice, getSale, saleHtml, submitSale, removeSale;
saleHtml = '<div class="admin_form"><input type="hidden" id="part_id" value="{{part_id}}" /><label for="price">Price<input type="text" id="price" placeholder="Enter your sale price" /></label><label for="sale_start">Sale Start<input type="text" id="sale_start" placeholder="Enter your start date for the sale" /></label><label for="sale_end">Sale End<input type="text" id="sale_end" placeholder="Enter your end date for the sale" /></label><input type="submit" id="submitSale" value="Set Sale Price" /><a href="javascript:$.modal.close()" class="modalClose" title="Close">Close</a></div>';

$(function () {
    priceTable = $('.priceTable').dataTable({
        'bJQueryUI': true,
        'iDisplayLength': 50
    });

    setList = (function (partID, row) {
        if (partID === undefined || partID === 0) {
            showMessage('Invalid reference to part.');
        } else if (confirm('Are you sure you want to set Part #' + partID + ' to list price?')) {
            $.getJSON('/Admin/Pricing/SetList', { 'id': partID }, function (resp) {
                if (resp.length !== 0) {
                    var index = priceTable.fnGetPosition(row);
                    priceTable.fnUpdate('$' + parseFloat(resp.price).toFixed(2), index, 1);
                } else {
                    showMessage('There was an error while processing your request.');
                }
            }).error(function (req, status, error) {
                showMessage(error);
            });
        }

    });

    setPrice = (function (partID, row) {
        var price, index;
        price = prompt('Please enter the new price.');
        if (price === undefined || price === 0) {
            showMessage('Invalid price.');
        } else if (partID === undefined || partID === 0) {
            showMessage('Invalid reference to part.');
        } else {
            $.getJSON('/Admin/Pricing/SetPrice', { 'partID': partID, 'price': price }, function (resp) {
                if (resp.length !== 0) {
                    index = priceTable.fnGetPosition(row);
                    priceTable.fnUpdate('$' + parseFloat(resp.price).toFixed(2), index, 1);
                } else {
                    showMessage('There was an error while processing your request.');
                }
            }).error(function (req, status, error) {
                showMessage(error);
            });
        }
    });

    getSale = (function (partID, row) {
        var price, index, view;
        view = { part_id: partID };
        $.modal(Mustache.to_html(saleHtml, view), {
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
        $('#sale_start,#sale_end').datepicker();
    });

    submitSale = (function (partID, price, start, end) {
        try {
            if (partID === undefined || partID === 0 || partID.length === 0) {
                throw 'Invalid part reference.';
            } else if (price === undefined || price === 0 || price.length === 0) {
                throw 'Price was no valid.';
            } else if (start === undefined || start.length === 0) {
                throw 'Start date was not valid.';
            } else if (start === undefined || start.length === 0) {
                throw 'End date was not valid.';
            }

            var opts = {
                partID: partID,
                price: price,
                sale_start: start,
                sale_end: end
            };
            $.getJSON('/Admin/Pricing/SetSale', opts, function (resp) {
                if (resp.cust_id === undefined || resp.cust_id === null || resp.cust_id === 0) {
                    showMessage('Failed to set price point');
                }
                priceTable.fnAddData([
                    resp.partID,
                    '$' + parseFloat(resp.price).toFixed(2),
                    'Yes',
                    resp.sale_start,
                    resp.sale_end,
                    '<a href="javascript:void(0)" title="Remove Sale" data-partID="' + resp.partID + '" data-price="' + resp.price + '" class="removeSale">Remove Sale Price</a>'
                ]);
            }).error(function (req, status, error) {
                showMessage('Failed to set price point: ' + error);
            });
            $.modal.close();
        } catch (err) {
            $.modal.close();
            showMessage(err);
        }
    });

    removeSale = (function (partID, price, row) {
        try {
            if (partID === undefined || partID === 0 || partID.length === 0) {
                throw 'Invalid part reference.';
            } else if (price === undefined || price === 0 || price.length === 0) {
                throw 'Price was no valid.';
            }

            if (!confirm('Are you sure you want to remove the sale for Part #' + partID + '?')) {
                return false;
            }

            var opts = {
                partID: partID,
                price: price
            };
            $.getJSON('/Admin/Pricing/RemoveSale', opts, function (resp) {
                priceTable.fnDeleteRow(row);
            }).error(function (req, status, error) {
                showMessage('Failed to remove sale: ' + error);
            });
            $.modal.close();
        } catch (err) {
            $.modal.close();
            showMessage(err);
        }
    });

    $(document).on('click', '.setList', function () {
        var row, partID;
        partID = $(this).attr('data-partID');
        row = $(this).closest('tr').get()[0];
        setList(partID, row);
    });

    $(document).on('click', '.setPrice', function () {
        var row, partID;
        row = $(this).closest('tr').get()[0];
        partID = $(this).attr('data-partID');
        setPrice(partID, row);
    });

    $(document).on('click', '.setSale', function () {
        var row, partID;
        row = $(this).closest('tr').get()[0];
        partID = $(this).attr('data-partID');
        getSale(partID, row);
    });

    $(document).on('click', '#submitSale', function () {
        var price, start, end, part_id;
        part_id = $('#part_id').val();
        price = $('#price').val();
        start = $('#sale_start').val();
        end = $('#sale_end').val();
        submitSale(part_id, price, start, end);
    });

    $(document).on('click', '.removeSale', function () {
        var price, partID, row;
        price = $(this).attr('data-price');
        partID = $(this).attr('data-partID');
        row = $(this).parent().parent().get()[0];
        removeSale(partID, price, row);
    });
});