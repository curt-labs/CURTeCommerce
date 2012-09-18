function trim(str) {
    str = str.replace(/^\s+/, '');
    for (var i = str.length - 1; i >= 0; i--) {
        if (/\S/.test(str.charAt(i))) {
            str = str.substring(0, i + 1);
            break;
        }
    }
    return str;
}

jQuery.fn.dataTableExt.oSort['date-asc'] = function (a, b) {
    if (trim(a) != '') {
        var x = new Date(a).getTime();
    } else {
        var x = new Date().getTime(); // = l'an 1000 ...
    }

    if (trim(b) != '') {
        var y = new Date(b).getTime();
    } else {
        var y = new Date().getTime();
    }
    var z = ((x < y) ? -1 : ((x > y) ? 1 : 0));
    return z;
};

jQuery.fn.dataTableExt.oSort['date-desc'] = function (a, b) {
    if (trim(a) != '') {
        var x = new Date(a).getTime();
    } else {
        var x = new Date().getTime();
    }

    if (trim(b) != '') {
        var y = new Date(b).getTime();
    } else {
        var y = new Date().getTime();
    }
    var z = ((x < y) ? 1 : ((x > y) ? -1 : 0));
    return z;
}; 